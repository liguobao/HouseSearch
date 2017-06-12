using System;
using System.Collections.Generic;
using System.Linq;
using JiebaNet.Segmenter;
using JiebaNet.Segmenter.Common;
using JiebaNet.Segmenter.PosSeg;

namespace JiebaNet.Analyser
{
    public class TfidfExtractor : KeywordExtractor
    {
        private static readonly string DefaultIdfFile = ConfigManager.IdfFile;
        private static readonly int DefaultWordCount = 20;

        private JiebaSegmenter Segmenter { get; set; }
        private PosSegmenter PosSegmenter { get; set; }
        private IdfLoader Loader { get; set; }

        private IDictionary<string, double> IdfFreq { get; set; }
        private double MedianIdf { get; set; }

        public TfidfExtractor(JiebaSegmenter segmenter = null)
        {
            if (segmenter.IsNull())
            {
                Segmenter = new JiebaSegmenter();
            }
            else
            {
                Segmenter = segmenter;
            }
            PosSegmenter = new PosSegmenter(Segmenter);
            SetStopWords(ConfigManager.StopWordsFile);
            if (StopWords.IsEmpty())
            {
                StopWords.UnionWith(DefaultStopWords);
            }

            Loader = new IdfLoader(DefaultIdfFile);

            IdfFreq = Loader.IdfFreq;
            MedianIdf = Loader.MedianIdf;
        }

        public void SetIdfPath(string idfPath)
        {
            Loader.SetNewPath(idfPath);
            IdfFreq = Loader.IdfFreq;
            MedianIdf = Loader.MedianIdf;
        }

        private IEnumerable<string> FilterCutByPos(string text, IEnumerable<string> allowPos)
        {
            var posTags = PosSegmenter.Cut(text).Where(p => allowPos.Contains(p.Flag));
            return posTags.Select(p => p.Word);
        }

        private IDictionary<string, double> GetWordIfidf(string text, IEnumerable<string> allowPos)
        {
            IEnumerable<string> words = null;
            if (allowPos.IsNotEmpty())
            {
                words = FilterCutByPos(text, allowPos);
            }
            else
            {
                words = Segmenter.Cut(text);
            }

            // Calculate TF
            var freq = new Dictionary<string, double>();
            foreach (var word in words)
            {
                var w = word;
                if (string.IsNullOrEmpty(w) || w.Trim().Length < 2 || StopWords.Contains(w.ToLower()))
                {
                    continue;
                }
                freq[w] = freq.GetDefault(w, 0.0) + 1.0;
            }
            var total = freq.Values.Sum();
            foreach (var k in freq.Keys.ToList())
            {
                freq[k] *= IdfFreq.GetDefault(k, MedianIdf) / total;
            }

            return freq;
        }

        public override IEnumerable<string> ExtractTags(string text, int count = 20, IEnumerable<string> allowPos = null)
        {
            if (count <= 0) { count = DefaultWordCount; }

            var freq = GetWordIfidf(text, allowPos);
            return freq.OrderByDescending(p => p.Value).Select(p => p.Key).Take(count);
        }

        public override IEnumerable<WordWeightPair> ExtractTagsWithWeight(string text, int count = 20, IEnumerable<string> allowPos = null)
        {
            if (count <= 0) { count = DefaultWordCount; }

            var freq = GetWordIfidf(text, allowPos);
            return freq.OrderByDescending(p => p.Value).Select(p => new WordWeightPair()
            {
                Word = p.Key, Weight = p.Value
            }).Take(count);
        }
    }

    public class WordWeightPair
    {
        public string Word { get; set; }
        public double Weight { get; set; }
    }
}
