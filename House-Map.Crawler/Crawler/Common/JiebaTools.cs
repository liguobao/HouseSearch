using JiebaNet.Analyser;
using JiebaNet.Segmenter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HouseMap.Crawler.Common
{
    public class JiebaTools
    {
        public static int GetHousePrice(string text)
        {
            int housePrice = 0;
            var extractor = new TfidfExtractor();
            var keywords = extractor.ExtractTags(text, 20, new List<string>() { "m" });
            if (keywords != null)
            {
                var prices = keywords.Distinct().Select(p =>
                {
                    var price = 0;
                    int.TryParse(p, out price);
                    return price;
                }).Where(p => p >= 500 && p <= 30000);
                return prices.FirstOrDefault();
            }
            return housePrice;
        }
    }
}
