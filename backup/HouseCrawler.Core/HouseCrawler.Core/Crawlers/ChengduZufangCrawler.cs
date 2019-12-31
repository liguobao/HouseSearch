using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using MySql.Data.MySqlClient;
using Dapper;
using AngleSharp.Dom;

namespace HouseCrawler.Core
{
    public class ChengduZufangCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();

        private HouseDapper houseDapper;

        private ConfigDapper configDapper;

        public ChengduZufangCrawler(HouseDapper houseDapper, ConfigDapper configDapper)
        {
            this.houseDapper = houseDapper;
            this.configDapper = configDapper;
        }


        public void Run()
        {
            int captrueHouseCount = 0;
            DateTime startTime = DateTime.Now;
            foreach (var crawlerConfiguration in configDapper.GetList(ConstConfigName.Chengdufgj).ToList())
            {
                LogHelper.RunActionNotThrowEx(() =>
                {
                    List<BaseHouseInfo> houses = new List<BaseHouseInfo>();
                    var confInfo = JsonConvert.DeserializeObject<dynamic>(crawlerConfiguration.ConfigurationValue);
                    for (var pageNum = 1; pageNum < confInfo.pagecount.Value; pageNum++)
                    {
                        var url = $"http://zf.cdfgj.gov.cn/{confInfo.path.Value}page={pageNum}";
                        var houseHTML = GetHouseHTML(url);
                        houses.AddRange(GetDataFromHMTL(confInfo.cityname.Value, houseHTML));
                    }
                    houseDapper.BulkInsertHouses(houses);
                    captrueHouseCount = captrueHouseCount + houses.Count;
                }, "CapturBaiXing", crawlerConfiguration);
            }
            LogHelper.Info($"ChengduZufangCrawler finish.本次共爬取到{captrueHouseCount}条数据，耗时{ (DateTime.Now - startTime).TotalSeconds}秒。");
        }

        private static List<BaseHouseInfo> GetDataFromHMTL(string cityName, string houseHTML)
        {
            List<BaseHouseInfo> houseList = new List<BaseHouseInfo>();
            if (string.IsNullOrEmpty(houseHTML))
                return houseList;
            var htmlDoc = htmlParser.Parse(houseHTML);
            var houseItems = htmlDoc.QuerySelectorAll("div.pan-item.clearfix");
            if (!houseItems.Any())
                return houseList;
            foreach (var item in houseItems)
            {
                int housePrice = GetHousePrice(item);
                string houseLocation = GetLocation(item);
                var titleItem = item.QuerySelector("h2");
                var pubTime = GetPubTime(item);
                string houseURL = GetHouseURL(titleItem);
                var houseInfo = new BaseHouseInfo
                {
                    HouseTitle = titleItem.TextContent.Replace("\n", "").Trim() + houseLocation,
                    HouseOnlineURL = "http://zf.cdfgj.gov.cn" + houseURL,
                    DisPlayPrice = housePrice + "元/月",
                    HouseLocation = houseLocation,
                    Source = ConstConfigName.Chengdufgj,
                    HousePrice = housePrice,
                    HouseText = item.InnerHtml,
                    LocationCityName = cityName,
                    PubTime = pubTime,
                    PicURLs = GetPhotos(item)
                };
                houseList.Add(houseInfo);
            }
            return houseList;
        }

        private static string GetHouseURL(IElement titleItem)
        {
            var hrefList = titleItem.QuerySelector("a").GetAttribute("href").Split(";");
            var houseURL = "";
            if (hrefList.Count() > 0)
            {
                houseURL = hrefList[0];
            }
            else
            {
                houseURL = titleItem.QuerySelector("a").GetAttribute("href");
            }

            return houseURL;
        }

        private static DateTime GetPubTime(IElement item)
        {
            var pubTimeItems = item.QuerySelectorAll("p.p_gx");
            var pubTimetext = "";
            if (pubTimeItems != null && pubTimeItems.Count() > 0)
            {
                if (pubTimeItems.Count() > 1)
                {
                    pubTimetext = pubTimeItems[1].TextContent.Replace("\n", "").Replace("发布时间：", "").Trim();
                }
                else
                {
                    pubTimetext = pubTimeItems[0].TextContent.Replace("\n", "").Replace("发布时间：", "").Trim();
                }
            }
            var pubTime = DateTime.Now;
            DateTime.TryParse(pubTimetext, out pubTime);
            return pubTime;
        }

        private static string GetLocation(IElement item)
        {
            var houseLocation = "";
            var locationItem = item.QuerySelectorAll("p").Where(p => !p.ClassList.Any()).FirstOrDefault();
            if (locationItem != null)
            {
                houseLocation = locationItem.TextContent.Replace("\n", "").Replace(" ", "").Trim();
            }

            return houseLocation;
        }

        private static int GetHousePrice(IElement item)
        {
            var disPlayPriceItem = item.QuerySelector("strong.rent-price");
            if (disPlayPriceItem == null)
            {
                disPlayPriceItem = item.QuerySelector("strong.current-price");
            }
            var disPlayPrice = disPlayPriceItem.TextContent.Replace("元/月", "").Replace("\n", "").Replace("当前价：", "").Trim();
            int.TryParse(disPlayPrice, out var housePrice);
            return housePrice;
        }

        private static String GetPhotos(IElement element)
        {
            var photos = new List<String>();
            var imageURL = element.QuerySelector("img")?.GetAttribute("src");
            if (imageURL != null)
            {
                photos.Add(imageURL.Replace(".170x130.jpg", ""));
            }
            return JsonConvert.SerializeObject(photos);
        }


        public static string GetHouseHTML(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "keep-alive");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }




        /// <summary>
        /// 初始化配置数据，首次运行项目才需要
        /// </summary>
        public void InitConfiguration()
        {
            #region 
            var cityJsonString = @"[
    {
        'name': '南京',
        'code': 'nanjing'
    },
    {
        'name': '常州',
        'code': 'changzhou'
    },
    {
        'name': '淮安',
        'code': 'huaian'
    },
    {
        'name': '连云港',
        'code': 'lianyungang'
    },
    {
        'name': '南通',
        'code': 'nantong'
    },
    {
        'name': '苏州',
        'code': 'suzhou'
    },
    {
        'name': '泰州',
        'code': 'tz'
    },
    {
        'name': '无锡',
        'code': 'wuxi'
    },
    {
        'name': '宿迁',
        'code': 'suqian'
    },
    {
        'name': '徐州',
        'code': 'xuzhou'
    },
    {
        'name': '盐城',
        'code': 'yancheng'
    },
    {
        'name': '扬州',
        'code': 'yangzhou'
    },
    {
        'name': '镇江',
        'code': 'zhenjiang'
    },
    {
        'name': '杭州',
        'code': 'hangzhou'
    },
    {
        'name': '湖州',
        'code': 'huzhou'
    },
    {
        'name': '嘉兴',
        'code': 'jiaxing'
    },
    {
        'name': '金华',
        'code': 'jinhua'
    },
    {
        'name': '丽水',
        'code': 'lishui'
    },
    {
        'name': '宁波',
        'code': 'ningbo'
    },
    {
        'name': '衢州',
        'code': 'quzhou'
    },
    {
        'name': '绍兴',
        'code': 'shaoxing'
    },
    {
        'name': '台州',
        'code': 'taizhou'
    },
    {
        'name': '温州',
        'code': 'wenzhou'
    },
    {
        'name': '舟山',
        'code': 'zhoushan'
    },
    {
        'name': '福州',
        'code': 'fuzhou'
    },
    {
        'name': '龙岩',
        'code': 'longyan'
    },
    {
        'name': '南平',
        'code': 'nanping'
    },
    {
        'name': '宁德',
        'code': 'ningde'
    },
    {
        'name': '莆田',
        'code': 'putian'
    },
    {
        'name': '泉州',
        'code': 'quanzhou'
    },
    {
        'name': '三明',
        'code': 'sanming'
    },
    {
        'name': '漳州',
        'code': 'zhangzhou'
    },
    {
        'name': '济南',
        'code': 'jinan'
    },
    {
        'name': '滨州',
        'code': 'binzhou'
    },
    {
        'name': '德州',
        'code': 'dezhou'
    },
    {
        'name': '东营',
        'code': 'dongying'
    },
    {
        'name': '菏泽',
        'code': 'heze'
    },
    {
        'name': '济宁',
        'code': 'jining'
    },
    {
        'name': '聊城',
        'code': 'liaocheng'
    },
    {
        'name': '临沂',
        'code': 'linyi'
    },
    {
        'name': '青岛',
        'code': 'qingdao'
    },
    {
        'name': '日照',
        'code': 'rizhao'
    },
    {
        'name': '泰安',
        'code': 'taian'
    },
    {
        'name': '威海',
        'code': 'weihai'
    },
    {
        'name': '潍坊',
        'code': 'weifang'
    },
    {
        'name': '烟台',
        'code': 'yantai'
    },
    {
        'name': '枣庄',
        'code': 'zaozhuang'
    },
    {
        'name': '淄博',
        'code': 'zibo'
    },
    {
        'name': '南昌',
        'code': 'nanchang'
    },
    {
        'name': '抚州',
        'code': 'fz'
    },
    {
        'name': '赣州',
        'code': 'ganzhou'
    },
    {
        'name': '吉安',
        'code': 'jian'
    },
    {
        'name': '景德镇',
        'code': 'jingdezhen'
    },
    {
        'name': '九江',
        'code': 'jiujiang'
    },
    {
        'name': '萍乡',
        'code': 'pingxiang'
    },
    {
        'name': '上饶',
        'code': 'shangrao'
    },
    {
        'name': '新余',
        'code': 'xinyu'
    },
    {
        'name': '宜春',
        'code': 'yc'
    },
    {
        'name': '鹰潭',
        'code': 'yingtan'
    },
    {
        'name': '合肥',
        'code': 'hefei'
    },
    {
        'name': '安庆',
        'code': 'anqing'
    },
    {
        'name': '蚌埠',
        'code': 'bengbu'
    },
    {
        'name': '池州',
        'code': 'chizhou'
    },
    {
        'name': '滁州',
        'code': 'chuzhou'
    },
    {
        'name': '阜阳',
        'code': 'fuyang'
    },
    {
        'name': '淮北',
        'code': 'huaibei'
    },
    {
        'name': '淮南',
        'code': 'huainan'
    },
    {
        'name': '黄山',
        'code': 'huangshan'
    },
    {
        'name': '六安',
        'code': 'luan'
    },
    {
        'name': '马鞍山',
        'code': 'maanshan'
    },
    {
        'name': '芜湖',
        'code': 'wuhu'
    },
    {
        'name': '宿州',
        'code': 'sz'
    },
    {
        'name': '宣城',
        'code': 'xuancheng'
    },
    {
        'name': '广州',
        'code': 'guangzhou'
    },
    {
        'name': '潮州',
        'code': 'chaozhou'
    },
    {
        'name': '佛山',
        'code': 'foshan'
    },
    {
        'name': '河源',
        'code': 'heyuan'
    },
    {
        'name': '惠州',
        'code': 'huizhou'
    },
    {
        'name': '江门',
        'code': 'jiangmen'
    },
    {
        'name': '揭阳',
        'code': 'jieyang'
    },
    {
        'name': '茂名',
        'code': 'maoming'
    },
    {
        'name': '梅州',
        'code': 'meizhou'
    },
    {
        'name': '清远',
        'code': 'qingyuan'
    },
    {
        'name': '汕头',
        'code': 'shantou'
    },
    {
        'name': '汕尾',
        'code': 'shanwei'
    },
    {
        'name': '韶关',
        'code': 'shaoguan'
    },
    {
        'name': '阳江',
        'code': 'yangjiang'
    },
    {
        'name': '云浮',
        'code': 'yunfu'
    },
    {
        'name': '湛江',
        'code': 'zhanjiang'
    },
    {
        'name': '肇庆',
        'code': 'zhaoqing'
    },
    {
        'name': '海口',
        'code': 'haikou'
    },
    {
        'name': '南宁',
        'code': 'nanning'
    },
    {
        'name': '百色',
        'code': 'bose'
    },
    {
        'name': '北海',
        'code': 'beihai'
    },
    {
        'name': '崇左',
        'code': 'chongzuo'
    },
    {
        'name': '防城港',
        'code': 'fangchenggang'
    },
    {
        'name': '贵港',
        'code': 'guigang'
    },
    {
        'name': '桂林',
        'code': 'guilin'
    },
    {
        'name': '河池',
        'code': 'hechi'
    },
    {
        'name': '贺州',
        'code': 'hezhou'
    },
    {
        'name': '来宾',
        'code': 'laibin'
    },
    {
        'name': '柳州',
        'code': 'liuzhou'
    },
    {
        'name': '钦州',
        'code': 'qinzhou'
    },
    {
        'name': '梧州',
        'code': 'wuzhou'
    },
    {
        'name': '玉林',
        'code': 'yl'
    },
    {
        'name': '恩施',
        'code': 'enshi'
    },
    {
        'name': '黄冈',
        'code': 'huanggang'
    },
    {
        'name': '黄石',
        'code': 'huangshi'
    },
    {
        'name': '荆门',
        'code': 'jingmen'
    },
    {
        'name': '荆州',
        'code': 'jingzhou'
    },
    {
        'name': '十堰',
        'code': 'shiyan'
    },
    {
        'name': '随州',
        'code': 'suizhou'
    },
    {
        'name': '咸宁',
        'code': 'xianning'
    },
    {
        'name': '襄阳',
        'code': 'xiangfan'
    },
    {
        'name': '孝感',
        'code': 'xiaogan'
    },
    {
        'name': '宜昌',
        'code': 'yichang'
    },
    {
        'name': '长沙',
        'code': 'changsha'
    },
    {
        'name': '常德',
        'code': 'changde'
    },
    {
        'name': '郴州',
        'code': 'chenzhou'
    },
    {
        'name': '衡阳',
        'code': 'hengyang'
    },
    {
        'name': '怀化',
        'code': 'huaihua'
    },
    {
        'name': '娄底',
        'code': 'loudi'
    },
    {
        'name': '邵阳',
        'code': 'shaoyang'
    },
    {
        'name': '湘潭',
        'code': 'xiangtan'
    },
    {
        'name': '湘西',
        'code': 'xiangxi'
    },
    {
        'name': '益阳',
        'code': 'yiyang'
    },
    {
        'name': '永州',
        'code': 'yongzhou'
    },
    {
        'name': '岳阳',
        'code': 'yueyang'
    },
    {
        'name': '张家界',
        'code': 'zhangjiajie'
    },
    {
        'name': '株洲',
        'code': 'zhuzhou'
    },
    {
        'name': '郑州',
        'code': 'zhengzhou'
    },
    {
        'name': '安阳',
        'code': 'anyang'
    },
    {
        'name': '鹤壁',
        'code': 'hebi'
    },
    {
        'name': '焦作',
        'code': 'jiaozuo'
    },
    {
        'name': '开封',
        'code': 'kaifeng'
    },
    {
        'name': '洛阳',
        'code': 'luoyang'
    },
    {
        'name': '漯河',
        'code': 'luohe'
    },
    {
        'name': '南阳',
        'code': 'nanyang'
    },
    {
        'name': '平顶山',
        'code': 'pingdingshan'
    },
    {
        'name': '濮阳',
        'code': 'puyang'
    },
    {
        'name': '三门峡',
        'code': 'sanmenxia'
    },
    {
        'name': '商丘',
        'code': 'shangqiu'
    },
    {
        'name': '新乡',
        'code': 'xinxiang'
    },
    {
        'name': '信阳',
        'code': 'xinyang'
    },
    {
        'name': '许昌',
        'code': 'xuchang'
    },
    {
        'name': '周口',
        'code': 'zhoukou'
    },
    {
        'name': '驻马店',
        'code': 'zhumadian'
    },
    {
        'name': '阿拉善',
        'code': 'alashan'
    },
    {
        'name': '巴彦淖尔',
        'code': 'bayannaoer'
    },
    {
        'name': '包头',
        'code': 'baotou'
    },
    {
        'name': '赤峰',
        'code': 'chifeng'
    },
    {
        'name': '鄂尔多斯',
        'code': 'eerduosi'
    },
    {
        'name': '呼伦贝尔',
        'code': 'hulunbeier'
    },
    {
        'name': '通辽',
        'code': 'tongliao'
    },
    {
        'name': '乌兰察布',
        'code': 'wulanchabu'
    },
    {
        'name': '锡林郭勒',
        'code': 'xilinguole'
    },
    {
        'name': '兴安',
        'code': 'xingan'
    },
    {
        'name': '石家庄',
        'code': 'shijiazhuang'
    },
    {
        'name': '保定',
        'code': 'baoding'
    },
    {
        'name': '沧州',
        'code': 'cangzhou'
    },
    {
        'name': '承德',
        'code': 'chengde'
    },
    {
        'name': '邯郸',
        'code': 'handan'
    },
    {
        'name': '衡水',
        'code': 'hengshui'
    },
    {
        'name': '廊坊',
        'code': 'langfang'
    },
    {
        'name': '秦皇岛',
        'code': 'qinhuangdao'
    },
    {
        'name': '唐山',
        'code': 'tangshan'
    },
    {
        'name': '邢台',
        'code': 'xingtai'
    },
    {
        'name': '张家口',
        'code': 'zhangjiakou'
    },
    {
        'name': '太原',
        'code': 'taiyuan'
    },
    {
        'name': '大同',
        'code': 'datong'
    },
    {
        'name': '晋城',
        'code': 'jincheng'
    },
    {
        'name': '晋中',
        'code': 'jinzhong'
    },
    {
        'name': '临汾',
        'code': 'linfen'
    },
    {
        'name': '吕梁',
        'code': 'lvliang'
    },
    {
        'name': '朔州',
        'code': 'shuozhou'
    },
    {
        'name': '忻州',
        'code': 'xinzhou'
    },
    {
        'name': '阳泉',
        'code': 'yangquan'
    },
    {
        'name': '运城',
        'code': 'yuncheng'
    },
    {
        'name': '长治',
        'code': 'changzhi'
    },
    {
        'name': '沈阳',
        'code': 'shenyang'
    },
    {
        'name': '鞍山',
        'code': 'anshan'
    },
    {
        'name': '朝阳',
        'code': 'chaoyang'
    },
    {
        'name': '大连',
        'code': 'dalian'
    },
    {
        'name': '丹东',
        'code': 'dandong'
    },
    {
        'name': '阜新',
        'code': 'fuxin'
    },
    {
        'name': '葫芦岛',
        'code': 'huludao'
    },
    {
        'name': '锦州',
        'code': 'jinzhou'
    },
    {
        'name': '辽阳',
        'code': 'liaoyang'
    },
    {
        'name': '盘锦',
        'code': 'panjin'
    },
    {
        'name': '铁岭',
        'code': 'tieling'
    },
    {
        'name': '营口',
        'code': 'yingkou'
    },
    {
        'name': '长春',
        'code': 'changchun'
    },
    {
        'name': '白城',
        'code': 'baicheng'
    },
    {
        'name': '白山',
        'code': 'baishan'
    },
    {
        'name': '吉林',
        'code': 'jilin'
    },
    {
        'name': '辽源',
        'code': 'liaoyuan'
    },
    {
        'name': '四平',
        'code': 'siping'
    },
    {
        'name': '松原',
        'code': 'songyuan'
    },
    {
        'name': '通化',
        'code': 'tonghua'
    },
    {
        'name': '延边',
        'code': 'yanbian'
    },
    {
        'name': '哈尔滨',
        'code': 'haerbin'
    },
    {
        'name': '大庆',
        'code': 'daqing'
    },
    {
        'name': '大兴安岭',
        'code': 'daxinganling'
    },
    {
        'name': '鹤岗',
        'code': 'hegang'
    },
    {
        'name': '黑河',
        'code': 'heihe'
    },
    {
        'name': '鸡西',
        'code': 'jixi'
    },
    {
        'name': '佳木斯',
        'code': 'jiamusi'
    },
    {
        'name': '牡丹江',
        'code': 'mudanjiang'
    },
    {
        'name': '七台河',
        'code': 'qitaihe'
    },
    {
        'name': '齐齐哈尔',
        'code': 'qiqihaer'
    },
    {
        'name': '双鸭山',
        'code': 'shuangyashan'
    },
    {
        'name': '绥化',
        'code': 'suihua'
    },
    {
        'name': '伊春',
        'code': 'yichun'
    },
    {
        'name': '成都',
        'code': 'chengdu'
    },
    {
        'name': '阿坝',
        'code': 'aba'
    },
    {
        'name': '巴中',
        'code': 'bazhong'
    },
    {
        'name': '达州',
        'code': 'dazhou'
    },
    {
        'name': '德阳',
        'code': 'deyang'
    },
    {
        'name': '甘孜',
        'code': 'ganzi'
    },
    {
        'name': '广安',
        'code': 'guangan'
    },
    {
        'name': '广元',
        'code': 'guangyuan'
    },
    {
        'name': '乐山',
        'code': 'leshan'
    },
    {
        'name': '凉山',
        'code': 'liangshan'
    },
    {
        'name': '泸州',
        'code': 'luzhou'
    },
    {
        'name': '眉山',
        'code': 'meishan'
    },
    {
        'name': '绵阳',
        'code': 'mianyang'
    },
    {
        'name': '南充',
        'code': 'nanchong'
    },
    {
        'name': '内江',
        'code': 'neijiang'
    },
    {
        'name': '攀枝花',
        'code': 'panzhihua'
    },
    {
        'name': '遂宁',
        'code': 'suining'
    },
    {
        'name': '雅安',
        'code': 'yaan'
    },
    {
        'name': '宜宾',
        'code': 'yibin'
    },
    {
        'name': '资阳',
        'code': 'ziyang'
    },
    {
        'name': '自贡',
        'code': 'zigong'
    },
    {
        'name': '阿里',
        'code': 'ali'
    },
    {
        'name': '昌都',
        'code': 'changdu'
    },
    {
        'name': '林芝',
        'code': 'linzhi'
    },
    {
        'name': '那曲',
        'code': 'naqu'
    },
    {
        'name': '日喀则',
        'code': 'rikaze'
    },
    {
        'name': '山南',
        'code': 'shannan'
    },
    {
        'name': '昆明',
        'code': 'kunming'
    },
    {
        'name': '保山',
        'code': 'baoshan'
    },
    {
        'name': '楚雄',
        'code': 'chuxiong'
    },
    {
        'name': '大理',
        'code': 'dali'
    },
    {
        'name': '德宏',
        'code': 'dehong'
    },
    {
        'name': '迪庆',
        'code': 'diqing'
    },
    {
        'name': '红河',
        'code': 'honghe'
    },
    {
        'name': '丽江',
        'code': 'lijiang'
    },
    {
        'name': '临沧',
        'code': 'lincang'
    },
    {
        'name': '怒江',
        'code': 'nujiang'
    },
    {
        'name': '普洱',
        'code': 'puer'
    },
    {
        'name': '曲靖',
        'code': 'qujing'
    },
    {
        'name': '文山',
        'code': 'wenshan'
    },
    {
        'name': '西双版纳',
        'code': 'xishuangbanna'
    },
    {
        'name': '玉溪',
        'code': 'yuxi'
    },
    {
        'name': '昭通',
        'code': 'zhaotong'
    },
    {
        'name': '贵阳',
        'code': 'guiyang'
    },
    {
        'name': '安顺',
        'code': 'anshun'
    },
    {
        'name': '毕节',
        'code': 'bijie'
    },
    {
        'name': '六盘水',
        'code': 'liupanshui'
    },
    {
        'name': '黔东南',
        'code': 'qiandongnan'
    },
    {
        'name': '黔南',
        'code': 'qiannan'
    },
    {
        'name': '黔西南',
        'code': 'qianxinan'
    },
    {
        'name': '铜仁',
        'code': 'tongren'
    },
    {
        'name': '遵义',
        'code': 'zunyi'
    },
    {
        'name': '西安',
        'code': 'xian'
    },
    {
        'name': '安康',
        'code': 'ankang'
    },
    {
        'name': '宝鸡',
        'code': 'baoji'
    },
    {
        'name': '汉中',
        'code': 'hanzhong'
    },
    {
        'name': '商洛',
        'code': 'shangluo'
    },
    {
        'name': '铜川',
        'code': 'tongchuan'
    },
    {
        'name': '渭南',
        'code': 'weinan'
    },
    {
        'name': '咸阳',
        'code': 'xianyang'
    },
    {
        'name': '延安',
        'code': 'yanan'
    },
    {
        'name': '榆林',
        'code': 'yulin'
    },
    {
        'name': '乌鲁木齐',
        'code': 'wulumuqi'
    },
    {
        'name': '阿克苏',
        'code': 'akesu'
    },
    {
        'name': '阿勒泰',
        'code': 'aletai'
    },
    {
        'name': '巴音郭楞',
        'code': 'bayinguoleng'
    },
    {
        'name': '博尔塔拉',
        'code': 'boertala'
    },
    {
        'name': '昌吉',
        'code': 'changji'
    },
    {
        'name': '哈密',
        'code': 'hami'
    },
    {
        'name': '和田',
        'code': 'hetian'
    },
    {
        'name': '克孜勒苏',
        'code': 'kezilesu'
    },
    {
        'name': '塔城',
        'code': 'tacheng'
    },
    {
        'name': '吐鲁番',
        'code': 'tulufan'
    },
    {
        'name': '伊犁',
        'code': 'yili'
    },
    {
        'name': '果洛',
        'code': 'guoluo'
    },
    {
        'name': '海北',
        'code': 'haibei'
    },
    {
        'name': '海东',
        'code': 'haidong'
    },
    {
        'name': '海南',
        'code': 'hainan'
    },
    {
        'name': '海西',
        'code': 'haixi'
    },
    {
        'name': '黄南',
        'code': 'huangnan'
    },
    {
        'name': '玉树',
        'code': 'yushu'
    },
    {
        'name': '银川',
        'code': 'yinchuan'
    },
    {
        'name': '固原',
        'code': 'guyuan'
    },
    {
        'name': '石嘴山',
        'code': 'shizuishan'
    },
    {
        'name': '吴忠',
        'code': 'wuzhong'
    },
    {
        'name': '中卫',
        'code': 'zhongwei'
    },
    {
        'name': '白银',
        'code': 'baiyin'
    },
    {
        'name': '定西',
        'code': 'dingxi'
    },
    {
        'name': '甘南',
        'code': 'gannan'
    },
    {
        'name': '金昌',
        'code': 'jinchang'
    },
    {
        'name': '酒泉',
        'code': 'jiuquan'
    },
    {
        'name': '临夏',
        'code': 'linxia'
    },
    {
        'name': '陇南',
        'code': 'longnan'
    },
    {
        'name': '平凉',
        'code': 'pingliang'
    },
    {
        'name': '庆阳',
        'code': 'qingyang'
    },
    {
        'name': '天水',
        'code': 'tianshui'
    },
    {
        'name': '武威',
        'code': 'wuwei'
    },
    {
        'name': '张掖',
        'code': 'zhangye'
    },
    {
        'name': '上海',
        'code': 'shanghai'
    },
    {
        'name': '北京',
        'code': 'beijing'
    },
    {
        'name': '天津',
        'code': 'tianjin'
    },
    {
        'name': '重庆',
        'code': 'chongqing'
    }
]";
            #endregion

            #region 插入配置数据
            dynamic lstCity = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(cityJsonString);
            if (lstCity != null)
            {
                var lst = new List<CrawlerConfiguration>();
                foreach (var cityInfo in lstCity)
                {
                    if (cityInfo?.name?.Value != null && cityInfo.code?.Value != null)
                    {
                        lst.Add(new CrawlerConfiguration()
                        {
                            ConfigurationKey = 0,
                            ConfigurationValue = $"{{'cityname':'{Convert.ToString(cityInfo.name.Value)}','shortcutname':'{Convert.ToString(cityInfo.code.Value)}','pagecount':5}}",
                            ConfigurationName = ConstConfigName.BaiXing,
                            IsEnabled = true,
                        });
                    }
                }

                string sqlText = "INSERT INTO `housecrawler`.`CrawlerConfigurations` (`ConfigurationName`, `ConfigurationValue`) VALUES (@ConfigurationName, @ConfigurationValue);";
                using (IDbConnection dbConnection = new MySqlConnection(""))
                {
                    dbConnection.Open();
                    IDbTransaction transaction = dbConnection.BeginTransaction();
                    var result = dbConnection.Execute(sqlText,
                                         lst, transaction: transaction);
                    transaction.Commit();
                }
            }



            #endregion

        }
    }
}
