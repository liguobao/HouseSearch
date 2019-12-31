using JiebaNet.Analyser;
using JiebaNet.Segmenter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HouseCrawler.Core.Common
{
    public class JiebaTools
    {
        public static decimal GetHousePrice(string text)
        {

            //var seg = new JiebaSegmenter();
            //var li = seg.Cut(text).ToList();

            decimal housePrice = 0;
            var extractor = new TfidfExtractor();
            var keywords = extractor.ExtractTags(text, 20, new List<string>() { "m" });
            if (keywords != null)
            {

                var lstProce = keywords.Distinct().Where(s => s.Length <= 5 && s.Length >= 3).OrderByDescending(s => s.Length);
                var price = lstProce.FirstOrDefault();
                decimal.TryParse(price, out housePrice);
            }
            return housePrice;
        }
    }
}
