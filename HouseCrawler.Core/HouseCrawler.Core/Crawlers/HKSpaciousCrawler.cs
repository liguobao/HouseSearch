using AngleSharp.Parser.Html;
using HouseCrawler.Core.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HouseCrawler.Core
{
    public class HKSpaciousCrawler
    {
        private HouseDapper houseDapper;
        public HKSpaciousCrawler(HouseDapper houseDapper)
        {
            this.houseDapper = houseDapper;
        }


        private static HtmlParser htmlParser = new HtmlParser();

        public void Run()
        {
            LogHelper.Info("HKSpaciousCrawler start.");
            DateTime startTime = DateTime.Now;
            var captrueHouseCount = CaptureHouse();
            LogHelper.Info($"HKSpaciousCrawler finish.本次共爬取到{captrueHouseCount}条数据，耗时{ (DateTime.Now - startTime).TotalSeconds}秒。");
        }

        private int CaptureHouse()
        {
            int captrueHouseCount = 0;
            List<BaseHouseInfo> houses = new List<BaseHouseInfo>();
            for (var pageNum = 1; pageNum < 20; pageNum++)
            {
                var result = GetHTML(pageNum);
                houses.AddRange(GetHouseDataFromHTML(result));
            }
            captrueHouseCount = captrueHouseCount + houses.Count;
            houseDapper.BulkInsertHouses(houses);
            return captrueHouseCount;
        }

        private static List<BaseHouseInfo> GetHouseDataFromHTML(string result)
        {
            var houseList = new List<BaseHouseInfo>();
            if (string.IsNullOrEmpty(result))
            {
                return houseList;
            }
            var htmlDoc = htmlParser.Parse(result);
            var list = htmlDoc.QuerySelectorAll("div").Where(element => element.ClassList.Contains("cell-listings-rows-for-cards--row"));
            if (list == null || list.Count() == 0)
            {
                return houseList;
            }
            foreach (var element in list)
            {

                string houseLocation = element.QuerySelectorAll("div")
                .Where(div => div.ClassList.Contains("carousel-inner"))
                .FirstOrDefault()?
                .QuerySelector("img")?
                .GetAttribute("alt");
                string houseTitle = "";
                decimal housePrice = 0;
                string disPlayPrice = "";
                var titleItem = element.QuerySelectorAll("div")
                .Where(div => div.ClassList.Contains("cell-listing-carousel--infobox-text")).FirstOrDefault();
                if (titleItem != null)
                {
                    houseTitle = houseLocation + titleItem.TextContent;
                    disPlayPrice = titleItem.FirstElementChild.TextContent;
                    var textPrice = titleItem.FirstElementChild.TextContent.Replace("HKD$", "").Replace("萬", "");
                    decimal.TryParse(textPrice, out housePrice);
                    //为了显示效果,此处乘以1000即可,本意应该是10000的
                    housePrice = housePrice * 1000;
                }
                var timeText = element.QuerySelectorAll("div")
                 .Where(div => div.ClassList.Contains("cell--listings--row--card__info-contact-response_posted_time"))
                 .FirstOrDefault()?.FirstElementChild.GetAttribute("datetime");
                var pubTime = DateTime.Now;
                DateTime.TryParse(timeText, out pubTime);

                var onlineUrl = "https://www.spacious.hk" + element.QuerySelectorAll("a")
                .Where(item => item.ClassList.Contains("GTM-tracking-full-listing"))
                .FirstOrDefault()?.GetAttribute("href");
                var houseInfo = new BaseHouseInfo
                {
                    HouseTitle = houseTitle,
                    HouseOnlineURL = onlineUrl,
                    DisPlayPrice = disPlayPrice,
                    HouseLocation = houseLocation,
                    Source = ConstConfigName.HKSpacious,
                    HousePrice = housePrice,
                    HouseText = element.InnerHtml,
                    LocationCityName = "香港",
                    PubTime = pubTime
                };
                houseList.Add(houseInfo);
            }

            return houseList;
        }

        private static string GetHTML(int pageNum)
        {
            var client = new RestClient("https://www.spacious.hk/zh-TW/partials/visible_listings");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            request.AddHeader("authority", "www.spacious.hk");
            request.AddHeader("referer", "https://www.spacious.hk/zh-TW/hong-kong/for-rent?disable_last_save_search_notice=true&hide_listing_search_questions=true");
            request.AddHeader("accept", "text/html, */*; q=0.01");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("origin", "https://www.spacious.hk");
            request.AddHeader("cookie", "__uzma=5404437318385277536; __uzmb=1526951424; _spacious_production_sessions_v2017_01_04_1426_=322656b1a04e8e8efa4fff606a9602d1; __atuvc=2%7C21; listing_search_view_mode=wide; Hm_lvt_af48f9f74d445fb8a44bb716e56cf6e4=1526951426,1527081492; Hm_lpvt_af48f9f74d445fb8a44bb716e56cf6e4=1527081492; App.Pages.Cities.RentalListingSearch.Data.PageVisitCount.value=1; App.Map.Session.%2Fzh-TW%2Fhong-kong%2Ffor-rent.lastZoom=11; App.Map.Session.%2Fzh-TW%2Fhong-kong%2Ffor-rent.lastLatitude=22.327094; App.Map.Session.%2Fzh-TW%2Fhong-kong%2Ffor-rent.lastLongitude=114.166725; __uzmc=362547097517; __uzmd=1527081580");
            request.AddParameter("application/json",
             "{\"neighbourhood_ids\":[\"1\",\"3\",\"4\",\"5\",\"6\",\"7\",\"8\",\"9\",\"10\",\"11\",\"12\","
             + "\"13\",\"14\",\"15\",\"16\",\"17\",\"18\",\"19\",\"20\",\"21\",\"22\",\"23\",\"24\",\"25\",\"26\","
             + "\"27\",\"28\",\"29\",\"30\",\"31\",\"32\",\"33\",\"34\",\"35\",\"36\",\"37\",\"38\",\"39\",\"40\","
             + "\"41\",\"42\",\"43\",\"44\",\"45\",\"46\",\"47\",\"48\",\"49\",\"50\",\"51\",\"52\",\"53\",\"54\","
             + "\"55\",\"56\",\"57\",\"58\",\"59\",\"60\",\"61\",\"62\",\"63\",\"64\",\"65\",\"66\",\"67\",\"68\",\"69\",\"70\",\"71\",\"72\",\"73\",\"74\",\"75\",\"76\",\"77\",\"78\",\"79\",\"80\",\"81\",\"82\",\"83\",\"84\",\"86\",\"87\",\"88\",\"89\",\"90\",\"92\",\"94\",\"95\",\"100\",\"104\",\"105\",\"107\",\"108\",\"109\",\"110\",\"111\",\"112\",\"113\",\"114\",\"115\",\"116\",\"117\",\"118\",\"119\",\"120\",\"121\",\"122\",\"123\",\"124\",\"125\",\"126\",\"127\",\"128\",\"132\",\"133\",\"134\",\"136\",\"137\",\"138\",\"139\",\"141\",\"142\",\"143\",\"144\",\"145\",\"146\",\"147\",\"148\",\"149\",\"151\",\"152\",\"154\",\"155\",\"156\",\"157\",\"158\",\"159\",\"160\",\"161\",\"162\",\"164\",\"166\",\"167\",\"275\",\"284\",\"367\",\"377\"],"
             + "\"listing_search\":{\"is_rental\":\"true\",\"is_commercial_listing\":\"false\",\"city_id\":\"1\",\"building_id\":\"\","
             + "\"travel_time_nodes\":\"\",\"place_radius\":\"500\",\"place_position\":\"\",\"place_name\":\"\",\"travel_time_position\":\"\",\"neighbourhood_ids\":[],"
             + "\"travel_time_place_name\":\"\",\"travel_time_duration\":\"10\",\"price_min\":\"\",\"price_max\":\"\",\"gross_min\":\"\","
             + "\"gross_max\":\"\",\"beds\":\"any\",\"number_of_bathrooms_range\":\"any\",\"building_age_range\":\"\",\"has_feature_car_park\":\"0\",\"has_feature_balcony\":\"0\",\"has_feature_roof\":\"0\",\"has_feature_terrace\":\"0\",\"has_feature_maids_quarters\":\"0\",\"has_feature_duplex\":\"0\",\"has_view_open\":\"0\",\"has_view_mountain\":\"0\",\"has_view_city\":\"0\",\"has_view_racecourse\":\"0\",\"has_view_sea\":\"0\",\"has_view_garden\":\"0\",\"has_view_building\":\"0\",\"has_feature_club_house\":\"0\",\"has_feature_gym\":\"0\",\"has_feature_pool\":\"0\",\"has_feature_tennis\":\"0\",\"has_feature_lift\":\"0\",\"has_feature_garden\":\"0\",\"has_feature_pet_friendly\":\"0\",\"age\":\"315569520\",\"lister_type\":\"any\",\"is_village_house\":\"0\",\"has_furnished\":\"0\",\"near_mtr\":\"0\",\"short_rental\":\"0\",\"flat_share_allowed_only\":\"0\",\"has_images\":\"0\",\"has_building_floor_plans\":\"0\",\"shortlisted_only\":\"0\",\"is_serviced_apartment\":\"0\",\"is_haunted_house\":\"0\",\"place_coordinates\":[\"\"],\"travel_time_coordinates\":[\"\"]},"
             + "\"sort\":\"time_desc\",\"zoom_level\":11,\"page\":" + pageNum + "}",
             ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return "";
        }
    }
}
