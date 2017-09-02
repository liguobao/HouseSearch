using System.ComponentModel.DataAnnotations.Schema;

namespace Pomelo.AspNetCore.TimedJob.EntityFramework
{
    [Table("AspNetTimedJobs")]
    public class TimedJob : DynamicTimedJob
    {
    }
}
