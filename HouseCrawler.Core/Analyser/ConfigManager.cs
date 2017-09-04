using System.IO;
using System;

namespace JiebaNet.Analyser
{
    public class ConfigManager
    {
        // TODO: duplicate codes.
        public static string ConfigFileBaseDir => Path.Combine(AppContext.BaseDirectory,"Resources");

        public static string IdfFile => Path.Combine(ConfigFileBaseDir, "idf.txt");

        public static string StopWordsFile => Path.Combine(ConfigFileBaseDir, "stopwords.txt");
    }
}