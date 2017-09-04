using System;
using System.IO;

namespace JiebaNet.Segmenter
{
    public class ConfigManager
    {
        public static string ConfigFileBaseDir
        {
            get
            {
                var configFileDir = "Resources";
                if (!Path.IsPathRooted(configFileDir))
                {
                    var domainDir = AppContext.BaseDirectory;
                    configFileDir = Path.GetFullPath(Path.Combine(domainDir, configFileDir));
                }
                return configFileDir;
            }
        }

        public static string MainDictFile => Path.Combine(ConfigFileBaseDir, "dict.txt");

        public static string ProbTransFile => Path.Combine(ConfigFileBaseDir, "prob_trans.json");

        public static string ProbEmitFile => Path.Combine(ConfigFileBaseDir, "prob_emit.json");

        public static string PosProbStartFile => Path.Combine(ConfigFileBaseDir, "pos_prob_start.json");

        public static string PosProbTransFile => Path.Combine(ConfigFileBaseDir, "pos_prob_trans.json");

        public static string PosProbEmitFile => Path.Combine(ConfigFileBaseDir, "pos_prob_emit.json");

        public static string CharStateTabFile => Path.Combine(ConfigFileBaseDir, "char_state_tab.json");
    }
}