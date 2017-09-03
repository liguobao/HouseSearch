using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AspNetCore.TimedJob.Jobs;

namespace Pomelo.AspNetCore.TimedJob
{
    public class TimedJobService
    {
        private IAssemblyLocator Locator { get; }

        private IServiceProvider Services { get; }

        private IDynamicTimedJobProvider DynamicJobs { get; }

        private ILogger Logger { get; }

        public Dictionary<string, bool> JobStatus { get; } = new Dictionary<string, bool>();

        public Dictionary<string, Timer> JobTimers { get; } = new Dictionary<string, Timer>();

        private List<TypeInfo> JobTypeCollection { get; } = new List<TypeInfo>();

        public TimedJobService(IAssemblyLocator locator, IServiceProvider services)
        {
            this.Services = services;
            this.Locator = locator;
            this.Logger = services.GetService<ILogger>();
            this.DynamicJobs = services.GetService<IDynamicTimedJobProvider>();
            var asm = locator.GetAssemblies();
            foreach (var x in asm)
            {
                // 查找基类为Job的类
                var types = x.DefinedTypes.Where(y => y.BaseType == typeof(Job)).ToList();
                foreach (var y in types)
                {
                    JobTypeCollection.Add(y);
                }
            }
            StartHardTimers();
            if (DynamicJobs != null)
                StartDynamicTimers();
        }

        private void StartHardTimers()
        {
            foreach (var x in JobTypeCollection)
            {
                foreach (var y in x.DeclaredMethods)
                {
                    if (y.GetCustomAttribute<NonJobAttribute>() == null)
                    {
                        JobStatus.Add(x.FullName + '.' + y.Name, false);
                        var invoke = y.GetCustomAttribute<InvokeAttribute>();
                        if (invoke != null && invoke.IsEnabled)
                        {
                            long delta = 0;
                            if (invoke.Begin == default(DateTime))
                                invoke.Begin = DateTime.Now;
                            else
                                delta = Convert.ToInt64((invoke.Begin - DateTime.Now).TotalMilliseconds.ToString("0"));
                            if (delta < 0)
                            {
                                delta = delta % Convert.ToInt64(invoke.Interval);
                                if (delta < 0)
                                    delta += Convert.ToInt64(invoke.Interval);
                            }

                            Task.Factory.StartNew(async () =>
                            {
                                if (delta > int.MaxValue)
                                {
                                    for (; delta > Int32.MaxValue; delta = delta - Int32.MaxValue)
                                    {
                                        await Task.Delay(Int32.MaxValue);
                                    }
                                }

                                var timer = new Timer(t => {
                                    Execute(x.FullName + '.' + y.Name);
                                }, null, Convert.ToInt32(delta), invoke.Interval);
                                JobTimers.Add(x.FullName + '.' + y.Name, timer);
                            });
                        }
                    }
                }
            }
        }

        private void StartDynamicTimers()
        {
            var jobs = DynamicJobs.GetJobs();
            foreach (var x in jobs)
            {
                // 如果Hard Timer已经启动则注销实例
                if (JobTimers.ContainsKey(x.Id))
                {
                    JobTimers[x.Id].Dispose();
                    JobStatus[x.Id] = false;
                    JobTimers.Remove(x.Id);
                    JobStatus.Remove(x.Id);
                }
                long delta = Convert.ToInt64((x.Begin - DateTime.Now).TotalMilliseconds);
                if (delta < 0)
                {
                    delta = delta % Convert.ToInt64(x.Interval);
                    if (delta < 0)
                        delta += Convert.ToInt64(x.Interval);
                }
                Task.Factory.StartNew(async () =>
                {
                    if (delta > int.MaxValue)
                    {
                        for (; delta > int.MaxValue; delta = delta - int.MaxValue)
                        {
                            await Task.Delay(int.MaxValue);
                        }
                    }
                    var timer = new Timer(t => {
                        Execute(x.Id);
                    }, null, Convert.ToInt32(delta), x.Interval);
                    JobTimers.Add(x.Id, timer);
                });
            }
        }

        public void RestartDynamicTimers()
        {
            var jobs = DynamicJobs.GetJobs();
            foreach (var x in jobs)
            {
                if (JobTimers.ContainsKey(x.Id))
                {
                    JobTimers[x.Id].Dispose();
                    JobStatus[x.Id] = false;
                    JobTimers.Remove(x.Id);
                    JobStatus.Remove(x.Id);
                }
            }
            StartDynamicTimers();
        }

        public bool Execute(string identifier)
        {
            var typename = identifier.Substring(0, identifier.LastIndexOf('.'));
            var function = identifier.Substring(identifier.LastIndexOf('.') + 1);
            var type = JobTypeCollection.SingleOrDefault(x => x.FullName == typename);
            if (type == null)
            {
                throw new NotImplementedException(typename + "." + function);
            }
            var argtypes = type.GetConstructors()
                .First()
                .GetParameters()
                .Select(x => x.ParameterType == typeof(IServiceProvider) ? Services : Services.GetService(x.ParameterType))
                .ToArray();
            var job = Activator.CreateInstance(type.AsType(), argtypes);
            var method = type.GetMethod(function);
            var paramtypes = method.GetParameters().Select(x => Services.GetService(x.ParameterType)).ToArray();
            var invokeAttr = method.GetCustomAttribute<InvokeAttribute>();
            lock (this)
            {
                if (invokeAttr != null && invokeAttr.SkipWhileExecuting && JobStatus[identifier])
                    return false;
                JobStatus[identifier] = true;
            }
            try
            {
                Logger?.LogInformation("Invoking " + identifier + "...");
                method.Invoke(job, paramtypes);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.ToString());
            }
            JobStatus[identifier] = false;
            return true;
        }

        public List<string> GetJobFunctions()
        {
            return (from x in JobTypeCollection from y in x.DeclaredMethods where y.GetCustomAttribute<NonJobAttribute>() == null select x.FullName + '.' + y.Name).ToList();
        }
    }
}
