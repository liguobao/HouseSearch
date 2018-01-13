using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using HouseCrawler.Core.DBService.DAL;
using HouseCrawler.Core.DataContent;

namespace HouseCrawler.Core
{
    public class PinPaiGongYuHouseCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();

        private static readonly CrawlerDataContent DataContent = new CrawlerDataContent();

        private static CrawlerDAL crawlerDAL = new CrawlerDAL();

        public static void CapturPinPaiHouseInfo()
        {
            foreach (var crawlerConfiguration in DataContent.CrawlerConfigurations.Where(c => c.ConfigurationName
               == ConstConfigurationName.PinPaiGongYu && c.IsEnabled).ToList())
            {

                LogHelper.RunActionNotThrowEx(() => 
                {
                    var confInfo = JsonConvert.DeserializeObject<dynamic>(crawlerConfiguration.ConfigurationValue);

                    for (var index = 0; index < confInfo.pagecount.Value; index++)
                    {
                        var url = $"http://{confInfo.shortcutname.Value}.58.com/pinpaigongyu/pn/{index}";
                        var htmlResult = HTTPHelper.GetHTMLByURL(url);
                        var page = new HtmlParser().Parse(htmlResult);
                        var lstLi = page.QuerySelectorAll("li").Where(element => element.HasAttribute("logr"));
                        if (!lstLi.Any())
                        {
                            continue;
                        }
                        GetDataOnPageDoc(confInfo, page);
                        DataContent.SaveChanges();
                    }

                }, "CapturPinPaiHouseInfo", crawlerConfiguration);

            }
        }

        private static void GetDataOnPageDoc(dynamic confInfo, 
            AngleSharp.Dom.Html.IHtmlDocument page)
        {
            foreach (var element in page.QuerySelectorAll("li").Where(element => element.HasAttribute("logr")))
            {
                var houseTitle = element.QuerySelector("h2").TextContent;
                var houseInfoList = houseTitle.Split(' ');
                int.TryParse(element.QuerySelector("b").TextContent, out var housePrice);
                var onlineUrl = $"http://{confInfo.shortcutname.Value}.58.com" + element.QuerySelector("a").GetAttribute("href");
                if (DataContent.ApartmentHouseInfos.Find(onlineUrl)!=null)
                    continue;
                var houseInfo = new ApartmentHouseInfo
                {
                    HouseTitle = houseTitle,
                    HouseOnlineURL = onlineUrl,
                    DisPlayPrice = element.QuerySelector("b").TextContent,
                    HouseLocation = new[] { "公寓", "青年社区" }.All(s => houseInfoList.Contains(s)) ? houseInfoList[0] : houseInfoList[1],
                    DataCreateTime = DateTime.Now,
                    Source = ConstConfigurationName.PinPaiGongYu,
                    HousePrice = housePrice,
                    HouseText = houseTitle,
                    LocationCityName = confInfo.cityname.Value,
                    PubTime = DateTime.Now
                };
                DataContent.ApartmentHouseInfos.Add(houseInfo);
            }
        }

        /// <summary>
        /// 过滤无效的城市配置
        /// </summary>
        public static void FilterInvalidCityConfig()
        {
            foreach (var doubanConf in DataContent.CrawlerConfigurations.Where(c => c.ConfigurationName
               == ConstConfigurationName.PinPaiGongYu).ToList())
            {
                var confInfo = JsonConvert.DeserializeObject<dynamic>(doubanConf.ConfigurationValue);
                var url = $"http://{confInfo.shortcutname.Value}.58.com/pinpaigongyu/pn/0";
                var htmlResult = HTTPHelper.GetHTMLByURL(url);
                var page = new HtmlParser().Parse(htmlResult);
                var lstLi = page.QuerySelectorAll("li").Where(element => element.HasAttribute("logr"));
                if (!lstLi.Any())
                {
                    doubanConf.IsEnabled = false;
                }
            }
            DataContent.SaveChanges();
        }


        /// <summary>
        /// 初始化配置数据，首次运行项目才需要
        /// </summary>
        public static void InitConfiguration()
        {

            //DomainJS/city.json
            #region  58同城所有的城市简称 cityJsonString 
            string cityJsonString = @"[
    {
                'cityName': '北京',
        'shortCut': 'bj'
    },
    {
                'cityName': '上海',
        'shortCut': 'sh'
    },
    {
                'cityName': '重庆',
        'shortCut': 'cq'
    },
    {
                'cityName': '青岛',
        'shortCut': 'qd'
    },
    {
                'cityName': '济南',
        'shortCut': 'jn'
    },
    {
                'cityName': '烟台',
        'shortCut': 'yt'
    },
    {
                'cityName': '潍坊',
        'shortCut': 'wf'
    },
    {
                'cityName': '临沂',
        'shortCut': 'linyi'
    },
    {
                'cityName': '淄博',
        'shortCut': 'zb'
    },
    {
                'cityName': '济宁',
        'shortCut': 'jining'
    },
    {
                'cityName': '泰安',
        'shortCut': 'ta'
    },
    {
                'cityName': '聊城',
        'shortCut': 'lc'
    },
    {
                'cityName': '威海',
        'shortCut': 'weihai'
    },
    {
                'cityName': '枣庄',
        'shortCut': 'zaozhuang'
    },
    {
                'cityName': '德州',
        'shortCut': 'dz'
    },
    {
                'cityName': '日照',
        'shortCut': 'rizhao'
    },
    {
                'cityName': '东营',
        'shortCut': 'dy'
    },
    {
                'cityName': '菏泽',
        'shortCut': 'heze'
    },
    {
                'cityName': '滨州',
        'shortCut': 'bz'
    },
    {
                'cityName': '莱芜',
        'shortCut': 'lw'
    },
    {
                'cityName': '章丘',
        'shortCut': 'zhangqiu'
    },
    {
                'cityName': '垦利',
        'shortCut': 'kl'
    },
    {
                'cityName': '诸城',
        'shortCut': 'zc'
    },
    {
                'cityName': '寿光',
        'shortCut': 'shouguang'
    },
    {
                'cityName': '苏州',
        'shortCut': 'su'
    },
    {
                'cityName': '南京',
        'shortCut': 'nj'
    },
    {
                'cityName': '无锡',
        'shortCut': 'wx'
    },
    {
                'cityName': '常州',
        'shortCut': 'cz'
    },
    {
                'cityName': '徐州',
        'shortCut': 'xz'
    },
    {
                'cityName': '南通',
        'shortCut': 'nt'
    },
    {
                'cityName': '扬州',
        'shortCut': 'yz'
    },
    {
                'cityName': '盐城',
        'shortCut': 'yancheng'
    },
    {
                'cityName': '淮安',
        'shortCut': 'ha'
    },
    {
                'cityName': '连云港',
        'shortCut': 'lyg'
    },
    {
                'cityName': '泰州',
        'shortCut': 'taizhou'
    },
    {
                'cityName': '宿迁',
        'shortCut': 'suqian'
    },
    {
                'cityName': '镇江',
        'shortCut': 'zj'
    },
    {
                'cityName': '沭阳',
        'shortCut': 'shuyang'
    },
    {
                'cityName': '大丰',
        'shortCut': 'dafeng'
    },
    {
                'cityName': '如皋',
        'shortCut': 'rugao'
    },
    {
                'cityName': '启东',
        'shortCut': 'qidong'
    },
    {
                'cityName': '溧阳',
        'shortCut': 'liyang'
    },
    {
                'cityName': '海门',
        'shortCut': 'haimen'
    },
    {
                'cityName': '东海',
        'shortCut': 'donghai'
    },
    {
                'cityName': '扬中',
        'shortCut': 'yangzhong'
    },
    {
                'cityName': '兴化',
        'shortCut': 'xinghuashi'
    },
    {
                'cityName': '新沂',
        'shortCut': 'xinyishi'
    },
    {
                'cityName': '泰兴',
        'shortCut': 'taixing'
    },
    {
                'cityName': '如东',
        'shortCut': 'rudong'
    },
    {
                'cityName': '邳州',
        'shortCut': 'pizhou'
    },
    {
                'cityName': '沛县',
        'shortCut': 'xzpeixian'
    },
    {
                'cityName': '靖江',
        'shortCut': 'jingjiang'
    },
    {
                'cityName': '建湖',
        'shortCut': 'jianhu'
    },
    {
                'cityName': '海安',
        'shortCut': 'haian'
    },
    {
                'cityName': '东台',
        'shortCut': 'dongtai'
    },
    {
                'cityName': '丹阳',
        'shortCut': 'danyang'
    },
    {
                'cityName': '杭州',
        'shortCut': 'hz'
    },
    {
                'cityName': '宁波',
        'shortCut': 'nb'
    },
    {
                'cityName': '温州',
        'shortCut': 'wz'
    },
    {
                'cityName': '金华',
        'shortCut': 'jh'
    },
    {
                'cityName': '嘉兴',
        'shortCut': 'jx'
    },
    {
                'cityName': '台州',
        'shortCut': 'tz'
    },
    {
                'cityName': '绍兴',
        'shortCut': 'sx'
    },
    {
                'cityName': '湖州',
        'shortCut': 'huzhou'
    },
    {
                'cityName': '丽水',
        'shortCut': 'lishui'
    },
    {
                'cityName': '衢州',
        'shortCut': 'quzhou'
    },
    {
                'cityName': '舟山',
        'shortCut': 'zhoushan'
    },
    {
                'cityName': '乐清',
        'shortCut': 'yueqingcity'
    },
    {
                'cityName': '瑞安',
        'shortCut': 'ruiancity'
    },
    {
                'cityName': '义乌',
        'shortCut': 'yiwu'
    },
    {
                'cityName': '余姚',
        'shortCut': 'yuyao'
    },
    {
                'cityName': '诸暨',
        'shortCut': 'zhuji'
    },
    {
                'cityName': '象山',
        'shortCut': 'xiangshanxian'
    },
    {
                'cityName': '温岭',
        'shortCut': 'wenling'
    },
    {
                'cityName': '桐乡',
        'shortCut': 'tongxiang'
    },
    {
                'cityName': '慈溪',
        'shortCut': 'cixi'
    },
    {
                'cityName': '长兴',
        'shortCut': 'changxing'
    },
    {
                'cityName': '嘉善',
        'shortCut': 'jiashanx'
    },
    {
                'cityName': '海宁',
        'shortCut': 'haining'
    },
    {
                'cityName': '德清',
        'shortCut': 'deqing'
    },
    {
                'cityName': '合肥',
        'shortCut': 'hf'
    },
    {
                'cityName': '芜湖',
        'shortCut': 'wuhu'
    },
    {
                'cityName': '蚌埠',
        'shortCut': 'bengbu'
    },
    {
                'cityName': '阜阳',
        'shortCut': 'fy'
    },
    {
                'cityName': '淮南',
        'shortCut': 'hn'
    },
    {
                'cityName': '安庆',
        'shortCut': 'anqing'
    },
    {
                'cityName': '宿州',
        'shortCut': 'suzhou'
    },
    {
                'cityName': '六安',
        'shortCut': 'la'
    },
    {
                'cityName': '淮北',
        'shortCut': 'huaibei'
    },
    {
                'cityName': '滁州',
        'shortCut': 'chuzhou'
    },
    {
                'cityName': '马鞍山',
        'shortCut': 'mas'
    },
    {
                'cityName': '铜陵',
        'shortCut': 'tongling'
    },
    {
                'cityName': '宣城',
        'shortCut': 'xuancheng'
    },
    {
                'cityName': '亳州',
        'shortCut': 'bozhou'
    },
    {
                'cityName': '黄山',
        'shortCut': 'huangshan'
    },
    {
                'cityName': '池州',
        'shortCut': 'chizhou'
    },
    {
                'cityName': '巢湖',
        'shortCut': 'ch'
    },
    {
                'cityName': '和县',
        'shortCut': 'hexian'
    },
    {
                'cityName': '霍邱',
        'shortCut': 'hq'
    },
    {
                'cityName': '桐城',
        'shortCut': 'tongcheng'
    },
    {
                'cityName': '宁国',
        'shortCut': 'ningguo'
    },
    {
                'cityName': '天长',
        'shortCut': 'tianchang'
    },
    {
                'cityName': '深圳',
        'shortCut': 'sz'
    },
    {
                'cityName': '广州',
        'shortCut': 'gz'
    },
    {
                'cityName': '东莞',
        'shortCut': 'dg'
    },
    {
                'cityName': '佛山',
        'shortCut': 'fs'
    },
    {
                'cityName': '中山',
        'shortCut': 'zs'
    },
    {
                'cityName': '珠海',
        'shortCut': 'zh'
    },
    {
                'cityName': '惠州',
        'shortCut': 'huizhou'
    },
    {
                'cityName': '江门',
        'shortCut': 'jm'
    },
    {
                'cityName': '汕头',
        'shortCut': 'st'
    },
    {
                'cityName': '湛江',
        'shortCut': 'zhanjiang'
    },
    {
                'cityName': '肇庆',
        'shortCut': 'zq'
    },
    {
                'cityName': '茂名',
        'shortCut': 'mm'
    },
    {
                'cityName': '揭阳',
        'shortCut': 'jy'
    },
    {
                'cityName': '梅州',
        'shortCut': 'mz'
    },
    {
                'cityName': '清远',
        'shortCut': 'qingyuan'
    },
    {
                'cityName': '阳江',
        'shortCut': 'yj'
    },
    {
                'cityName': '韶关',
        'shortCut': 'sg'
    },
    {
                'cityName': '河源',
        'shortCut': 'heyuan'
    },
    {
                'cityName': '云浮',
        'shortCut': 'yf'
    },
    {
                'cityName': '汕尾',
        'shortCut': 'sw'
    },
    {
                'cityName': '潮州',
        'shortCut': 'chaozhou'
    },
    {
                'cityName': '台山',
        'shortCut': 'taishan'
    },
    {
                'cityName': '阳春',
        'shortCut': 'yangchun'
    },
    {
                'cityName': '顺德',
        'shortCut': 'sd'
    },
    {
                'cityName': '惠东',
        'shortCut': 'huidong'
    },
    {
                'cityName': '博罗',
        'shortCut': 'boluo'
    },
    {
                'cityName': '福州',
        'shortCut': 'fz'
    },
    {
                'cityName': '厦门',
        'shortCut': 'xm'
    },
    {
                'cityName': '泉州',
        'shortCut': 'qz'
    },
    {
                'cityName': '莆田',
        'shortCut': 'pt'
    },
    {
                'cityName': '漳州',
        'shortCut': 'zhangzhou'
    },
    {
                'cityName': '宁德',
        'shortCut': 'nd'
    },
    {
                'cityName': '三明',
        'shortCut': 'sm'
    },
    {
                'cityName': '南平',
        'shortCut': 'np'
    },
    {
                'cityName': '龙岩',
        'shortCut': 'ly'
    },
    {
                'cityName': '武夷山',
        'shortCut': 'wuyishan'
    },
    {
                'cityName': '石狮',
        'shortCut': 'shishi'
    },
    {
                'cityName': '晋江',
        'shortCut': 'jinjiangshi'
    },
    {
                'cityName': '南安',
        'shortCut': 'nananshi'
    },
    {
                'cityName': '南宁',
        'shortCut': 'nn'
    },
    {
                'cityName': '柳州',
        'shortCut': 'liuzhou'
    },
    {
                'cityName': '桂林',
        'shortCut': 'gl'
    },
    {
                'cityName': '玉林',
        'shortCut': 'yulin'
    },
    {
                'cityName': '梧州',
        'shortCut': 'wuzhou'
    },
    {
                'cityName': '北海',
        'shortCut': 'bh'
    },
    {
                'cityName': '贵港',
        'shortCut': 'gg'
    },
    {
                'cityName': '钦州',
        'shortCut': 'qinzhou'
    },
    {
                'cityName': '百色',
        'shortCut': 'baise'
    },
    {
                'cityName': '河池',
        'shortCut': 'hc'
    },
    {
                'cityName': '来宾',
        'shortCut': 'lb'
    },
    {
                'cityName': '贺州',
        'shortCut': 'hezhou'
    },
    {
                'cityName': '防城港',
        'shortCut': 'fcg'
    },
    {
                'cityName': '崇左',
        'shortCut': 'chongzuo'
    },
    {
                'cityName': '海口',
        'shortCut': 'haikou'
    },
    {
                'cityName': '三亚',
        'shortCut': 'sanya'
    },
    {
                'cityName': '五指山',
        'shortCut': 'wzs'
    },
    {
                'cityName': '三沙',
        'shortCut': 'sansha'
    },
    {
                'cityName': '琼海',
        'shortCut': 'qionghai'
    },
    {
                'cityName': '文昌',
        'shortCut': 'wenchang'
    },
    {
                'cityName': '万宁',
        'shortCut': 'wanning'
    },
    {
                'cityName': '屯昌',
        'shortCut': 'tunchang'
    },
    {
                'cityName': '琼中',
        'shortCut': 'qiongzhong'
    },
    {
                'cityName': '陵水',
        'shortCut': 'lingshui'
    },
    {
                'cityName': '东方',
        'shortCut': 'df'
    },
    {
                'cityName': '定安',
        'shortCut': 'da'
    },
    {
                'cityName': '澄迈',
        'shortCut': 'cm'
    },
    {
                'cityName': '保亭',
        'shortCut': 'baoting'
    },
    {
                'cityName': '白沙',
        'shortCut': 'baish'
    },
    {
                'cityName': '儋州',
        'shortCut': 'danzhou'
    },
    {
                'cityName': '郑州',
        'shortCut': 'zz'
    },
    {
                'cityName': '洛阳',
        'shortCut': 'luoyang'
    },
    {
                'cityName': '新乡',
        'shortCut': 'xx'
    },
    {
                'cityName': '南阳',
        'shortCut': 'ny'
    },
    {
                'cityName': '许昌',
        'shortCut': 'xc'
    },
    {
                'cityName': '平顶山',
        'shortCut': 'pds'
    },
    {
                'cityName': '安阳',
        'shortCut': 'ay'
    },
    {
                'cityName': '焦作',
        'shortCut': 'jiaozuo'
    },
    {
                'cityName': '商丘',
        'shortCut': 'sq'
    },
    {
                'cityName': '开封',
        'shortCut': 'kaifeng'
    },
    {
                'cityName': '濮阳',
        'shortCut': 'puyang'
    },
    {
                'cityName': '周口',
        'shortCut': 'zk'
    },
    {
                'cityName': '信阳',
        'shortCut': 'xy'
    },
    {
                'cityName': '驻马店',
        'shortCut': 'zmd'
    },
    {
                'cityName': '漯河',
        'shortCut': 'luohe'
    },
    {
                'cityName': '三门峡',
        'shortCut': 'smx'
    },
    {
                'cityName': '鹤壁',
        'shortCut': 'hb'
    },
    {
                'cityName': '济源',
        'shortCut': 'jiyuan'
    },
    {
                'cityName': '明港',
        'shortCut': 'mg'
    },
    {
                'cityName': '鄢陵',
        'shortCut': 'yanling'
    },
    {
                'cityName': '禹州',
        'shortCut': 'yuzhou'
    },
    {
                'cityName': '长葛',
        'shortCut': 'changge'
    },
    {
                'cityName': '武汉',
        'shortCut': 'wh'
    },
    {
                'cityName': '宜昌',
        'shortCut': 'yc'
    },
    {
                'cityName': '襄阳',
        'shortCut': 'xf'
    },
    {
                'cityName': '荆州',
        'shortCut': 'jingzhou'
    },
    {
                'cityName': '十堰',
        'shortCut': 'shiyan'
    },
    {
                'cityName': '黄石',
        'shortCut': 'hshi'
    },
    {
                'cityName': '孝感',
        'shortCut': 'xiaogan'
    },
    {
                'cityName': '黄冈',
        'shortCut': 'hg'
    },
    {
                'cityName': '恩施',
        'shortCut': 'es'
    },
    {
                'cityName': '荆门',
        'shortCut': 'jingmen'
    },
    {
                'cityName': '咸宁',
        'shortCut': 'xianning'
    },
    {
                'cityName': '鄂州',
        'shortCut': 'ez'
    },
    {
                'cityName': '随州',
        'shortCut': 'suizhou'
    },
    {
                'cityName': '潜江',
        'shortCut': 'qianjiang'
    },
    {
                'cityName': '天门',
        'shortCut': 'tm'
    },
    {
                'cityName': '仙桃',
        'shortCut': 'xiantao'
    },
    {
                'cityName': '神农架',
        'shortCut': 'snj'
    },
    {
                'cityName': '宜都',
        'shortCut': 'yidou'
    },
    {
                'cityName': '长沙',
        'shortCut': 'cs'
    },
    {
                'cityName': '株洲',
        'shortCut': 'zhuzhou'
    },
    {
                'cityName': '益阳',
        'shortCut': 'yiyang'
    },
    {
                'cityName': '常德',
        'shortCut': 'changde'
    },
    {
                'cityName': '衡阳',
        'shortCut': 'hy'
    },
    {
                'cityName': '湘潭',
        'shortCut': 'xiangtan'
    },
    {
                'cityName': '岳阳',
        'shortCut': 'yy'
    },
    {
                'cityName': '郴州',
        'shortCut': 'chenzhou'
    },
    {
                'cityName': '邵阳',
        'shortCut': 'shaoyang'
    },
    {
                'cityName': '怀化',
        'shortCut': 'hh'
    },
    {
                'cityName': '永州',
        'shortCut': 'yongzhou'
    },
    {
                'cityName': '娄底',
        'shortCut': 'ld'
    },
    {
                'cityName': '湘西',
        'shortCut': 'xiangxi'
    },
    {
                'cityName': '张家界',
        'shortCut': 'zjj'
    },
    {
                'cityName': '南昌',
        'shortCut': 'nc'
    },
    {
                'cityName': '赣州',
        'shortCut': 'ganzhou'
    },
    {
                'cityName': '九江',
        'shortCut': 'jj'
    },
    {
                'cityName': '宜春',
        'shortCut': 'yichun'
    },
    {
                'cityName': '吉安',
        'shortCut': 'ja'
    },
    {
                'cityName': '上饶',
        'shortCut': 'sr'
    },
    {
                'cityName': '萍乡',
        'shortCut': 'px'
    },
    {
                'cityName': '抚州',
        'shortCut': 'fuzhou'
    },
    {
                'cityName': '景德镇',
        'shortCut': 'jdz'
    },
    {
                'cityName': '新余',
        'shortCut': 'xinyu'
    },
    {
                'cityName': '鹰潭',
        'shortCut': 'yingtan'
    },
    {
                'cityName': '永新',
        'shortCut': 'yxx'
    },
    {
                'cityName': '沈阳',
        'shortCut': 'sy'
    },
    {
                'cityName': '大连',
        'shortCut': 'dl'
    },
    {
                'cityName': '鞍山',
        'shortCut': 'as'
    },
    {
                'cityName': '锦州',
        'shortCut': 'jinzhou'
    },
    {
                'cityName': '抚顺',
        'shortCut': 'fushun'
    },
    {
                'cityName': '营口',
        'shortCut': 'yk'
    },
    {
                'cityName': '盘锦',
        'shortCut': 'pj'
    },
    {
                'cityName': '朝阳',
        'shortCut': 'cy'
    },
    {
                'cityName': '丹东',
        'shortCut': 'dandong'
    },
    {
                'cityName': '辽阳',
        'shortCut': 'liaoyang'
    },
    {
                'cityName': '本溪',
        'shortCut': 'benxi'
    },
    {
                'cityName': '葫芦岛',
        'shortCut': 'hld'
    },
    {
                'cityName': '铁岭',
        'shortCut': 'tl'
    },
    {
                'cityName': '阜新',
        'shortCut': 'fx'
    },
    {
                'cityName': '庄河',
        'shortCut': 'pld'
    },
    {
                'cityName': '瓦房店',
        'shortCut': 'wfd'
    },
    {
                'cityName': '哈尔滨',
        'shortCut': 'hrb'
    },
    {
                'cityName': '大庆',
        'shortCut': 'dq'
    },
    {
                'cityName': '齐齐哈尔',
        'shortCut': 'qqhr'
    },
    {
                'cityName': '牡丹江',
        'shortCut': 'mdj'
    },
    {
                'cityName': '绥化',
        'shortCut': 'suihua'
    },
    {
                'cityName': '佳木斯',
        'shortCut': 'jms'
    },
    {
                'cityName': '鸡西',
        'shortCut': 'jixi'
    },
    {
                'cityName': '双鸭山',
        'shortCut': 'sys'
    },
    {
                'cityName': '鹤岗',
        'shortCut': 'hegang'
    },
    {
                'cityName': '黑河',
        'shortCut': 'heihe'
    },
    {
                'cityName': '伊春',
        'shortCut': 'yich'
    },
    {
                'cityName': '七台河',
        'shortCut': 'qth'
    },
    {
                'cityName': '大兴安岭',
        'shortCut': 'dxal'
    },
    {
                'cityName': '长春',
        'shortCut': 'cc'
    },
    {
                'cityName': '吉林',
        'shortCut': 'jl'
    },
    {
                'cityName': '四平',
        'shortCut': 'sp'
    },
    {
                'cityName': '延边',
        'shortCut': 'yanbian'
    },
    {
                'cityName': '松原',
        'shortCut': 'songyuan'
    },
    {
                'cityName': '白城',
        'shortCut': 'bc'
    },
    {
                'cityName': '通化',
        'shortCut': 'th'
    },
    {
                'cityName': '白山',
        'shortCut': 'baishan'
    },
    {
                'cityName': '辽源',
        'shortCut': 'liaoyuan'
    },
    {
                'cityName': '成都',
        'shortCut': 'cd'
    },
    {
                'cityName': '绵阳',
        'shortCut': 'mianyang'
    },
    {
                'cityName': '德阳',
        'shortCut': 'deyang'
    },
    {
                'cityName': '南充',
        'shortCut': 'nanchong'
    },
    {
                'cityName': '宜宾',
        'shortCut': 'yb'
    },
    {
                'cityName': '自贡',
        'shortCut': 'zg'
    },
    {
                'cityName': '乐山',
        'shortCut': 'ls'
    },
    {
                'cityName': '泸州',
        'shortCut': 'luzhou'
    },
    {
                'cityName': '达州',
        'shortCut': 'dazhou'
    },
    {
                'cityName': '内江',
        'shortCut': 'scnj'
    },
    {
                'cityName': '遂宁',
        'shortCut': 'suining'
    },
    {
                'cityName': '攀枝花',
        'shortCut': 'panzhihua'
    },
    {
                'cityName': '眉山',
        'shortCut': 'ms'
    },
    {
                'cityName': '广安',
        'shortCut': 'ga'
    },
    {
                'cityName': '资阳',
        'shortCut': 'zy'
    },
    {
                'cityName': '凉山',
        'shortCut': 'liangshan'
    },
    {
                'cityName': '广元',
        'shortCut': 'guangyuan'
    },
    {
                'cityName': '雅安',
        'shortCut': 'ya'
    },
    {
                'cityName': '巴中',
        'shortCut': 'bazhong'
    },
    {
                'cityName': '阿坝',
        'shortCut': 'ab'
    },
    {
                'cityName': '甘孜',
        'shortCut': 'ganzi'
    },
    {
                'cityName': '昆明',
        'shortCut': 'km'
    },
    {
                'cityName': '曲靖',
        'shortCut': 'qj'
    },
    {
                'cityName': '大理',
        'shortCut': 'dali'
    },
    {
                'cityName': '红河',
        'shortCut': 'honghe'
    },
    {
                'cityName': '玉溪',
        'shortCut': 'yx'
    },
    {
                'cityName': '丽江',
        'shortCut': 'lj'
    },
    {
                'cityName': '文山',
        'shortCut': 'ws'
    },
    {
                'cityName': '楚雄',
        'shortCut': 'cx'
    },
    {
                'cityName': '西双版纳',
        'shortCut': 'bn'
    },
    {
                'cityName': '昭通',
        'shortCut': 'zt'
    },
    {
                'cityName': '德宏',
        'shortCut': 'dh'
    },
    {
                'cityName': '普洱',
        'shortCut': 'pe'
    },
    {
                'cityName': '保山',
        'shortCut': 'bs'
    },
    {
                'cityName': '临沧',
        'shortCut': 'lincang'
    },
    {
                'cityName': '迪庆',
        'shortCut': 'diqing'
    },
    {
                'cityName': '怒江',
        'shortCut': 'nujiang'
    },
    {
                'cityName': '贵阳',
        'shortCut': 'gy'
    },
    {
                'cityName': '遵义',
        'shortCut': 'zunyi'
    },
    {
                'cityName': '黔东南',
        'shortCut': 'qdn'
    },
    {
                'cityName': '黔南',
        'shortCut': 'qn'
    },
    {
                'cityName': '六盘水',
        'shortCut': 'lps'
    },
    {
                'cityName': '毕节',
        'shortCut': 'bijie'
    },
    {
                'cityName': '铜仁',
        'shortCut': 'tr'
    },
    {
                'cityName': '安顺',
        'shortCut': 'anshun'
    },
    {
                'cityName': '黔西南',
        'shortCut': 'qxn'
    },
    {
                'cityName': '拉萨',
        'shortCut': 'lasa'
    },
    {
                'cityName': '日喀则',
        'shortCut': 'rkz'
    },
    {
                'cityName': '山南',
        'shortCut': 'sn'
    },
    {
                'cityName': '林芝',
        'shortCut': 'linzhi'
    },
    {
                'cityName': '昌都',
        'shortCut': 'changdu'
    },
    {
                'cityName': '那曲',
        'shortCut': 'nq'
    },
    {
                'cityName': '阿里',
        'shortCut': 'al'
    },
    {
                'cityName': '日土',
        'shortCut': 'rituxian'
    },
    {
                'cityName': '改则',
        'shortCut': 'gaizexian'
    },
    {
                'cityName': '石家庄',
        'shortCut': 'sjz'
    },
    {
                'cityName': '保定',
        'shortCut': 'bd'
    },
    {
                'cityName': '唐山',
        'shortCut': 'ts'
    },
    {
                'cityName': '廊坊',
        'shortCut': 'lf'
    },
    {
                'cityName': '邯郸',
        'shortCut': 'hd'
    },
    {
                'cityName': '秦皇岛',
        'shortCut': 'qhd'
    },
    {
                'cityName': '沧州',
        'shortCut': 'cangzhou'
    },
    {
                'cityName': '邢台',
        'shortCut': 'xt'
    },
    {
                'cityName': '衡水',
        'shortCut': 'hs'
    },
    {
                'cityName': '张家口',
        'shortCut': 'zjk'
    },
    {
                'cityName': '承德',
        'shortCut': 'chengde'
    },
    {
                'cityName': '定州',
        'shortCut': 'dingzhou'
    },
    {
                'cityName': '馆陶',
        'shortCut': 'gt'
    },
    {
                'cityName': '张北',
        'shortCut': 'zhangbei'
    },
    {
                'cityName': '赵县',
        'shortCut': 'zx'
    },
    {
                'cityName': '正定',
        'shortCut': 'zd'
    },
    {
                'cityName': '太原',
        'shortCut': 'ty'
    },
    {
                'cityName': '临汾',
        'shortCut': 'linfen'
    },
    {
                'cityName': '大同',
        'shortCut': 'dt'
    },
    {
                'cityName': '运城',
        'shortCut': 'yuncheng'
    },
    {
                'cityName': '晋中',
        'shortCut': 'jz'
    },
    {
                'cityName': '长治',
        'shortCut': 'changzhi'
    },
    {
                'cityName': '晋城',
        'shortCut': 'jincheng'
    },
    {
                'cityName': '阳泉',
        'shortCut': 'yq'
    },
    {
                'cityName': '吕梁',
        'shortCut': 'lvliang'
    },
    {
                'cityName': '忻州',
        'shortCut': 'xinzhou'
    },
    {
                'cityName': '朔州',
        'shortCut': 'shuozhou'
    },
    {
                'cityName': '临猗',
        'shortCut': 'linyixian'
    },
    {
                'cityName': '清徐',
        'shortCut': 'qingxu'
    },
    {
                'cityName': '呼和浩特',
        'shortCut': 'hu'
    },
    {
                'cityName': '包头',
        'shortCut': 'bt'
    },
    {
                'cityName': '赤峰',
        'shortCut': 'chifeng'
    },
    {
                'cityName': '鄂尔多斯',
        'shortCut': 'erds'
    },
    {
                'cityName': '通辽',
        'shortCut': 'tongliao'
    },
    {
                'cityName': '呼伦贝尔',
        'shortCut': 'hlbe'
    },
    {
                'cityName': '巴彦淖尔市',
        'shortCut': 'bycem'
    },
    {
                'cityName': '乌兰察布',
        'shortCut': 'wlcb'
    },
    {
                'cityName': '锡林郭勒',
        'shortCut': 'xl'
    },
    {
                'cityName': '兴安盟',
        'shortCut': 'xam'
    },
    {
                'cityName': '乌海',
        'shortCut': 'wuhai'
    },
    {
                'cityName': '阿拉善盟',
        'shortCut': 'alsm'
    },
    {
                'cityName': '海拉尔',
        'shortCut': 'hlr'
    },
    {
                'cityName': '西安',
        'shortCut': 'xa'
    },
    {
                'cityName': '咸阳',
        'shortCut': 'xianyang'
    },
    {
                'cityName': '宝鸡',
        'shortCut': 'baoji'
    },
    {
                'cityName': '渭南',
        'shortCut': 'wn'
    },
    {
                'cityName': '汉中',
        'shortCut': 'hanzhong'
    },
    {
                'cityName': '榆林',
        'shortCut': 'yl'
    },
    {
                'cityName': '延安',
        'shortCut': 'yanan'
    },
    {
                'cityName': '安康',
        'shortCut': 'ankang'
    },
    {
                'cityName': '商洛',
        'shortCut': 'sl'
    },
    {
                'cityName': '铜川',
        'shortCut': 'tc'
    },
    {
                'cityName': '乌鲁木齐',
        'shortCut': 'xj'
    },
    {
                'cityName': '昌吉',
        'shortCut': 'changji'
    },
    {
                'cityName': '巴音郭楞',
        'shortCut': 'bygl'
    },
    {
                'cityName': '伊犁',
        'shortCut': 'yili'
    },
    {
                'cityName': '阿克苏',
        'shortCut': 'aks'
    },
    {
                'cityName': '喀什',
        'shortCut': 'ks'
    },
    {
                'cityName': '哈密',
        'shortCut': 'hami'
    },
    {
                'cityName': '克拉玛依',
        'shortCut': 'klmy'
    },
    {
                'cityName': '博尔塔拉',
        'shortCut': 'betl'
    },
    {
                'cityName': '吐鲁番',
        'shortCut': 'tlf'
    },
    {
                'cityName': '和田',
        'shortCut': 'ht'
    },
    {
                'cityName': '石河子',
        'shortCut': 'shz'
    },
    {
                'cityName': '克孜勒苏',
        'shortCut': 'kzls'
    },
    {
                'cityName': '阿拉尔',
        'shortCut': 'ale'
    },
    {
                'cityName': '五家渠',
        'shortCut': 'wjq'
    },
    {
                'cityName': '图木舒克',
        'shortCut': 'tmsk'
    },
    {
                'cityName': '库尔勒',
        'shortCut': 'kel'
    },
    {
                'cityName': '阿勒泰',
        'shortCut': 'alt'
    },
    {
                'cityName': '塔城',
        'shortCut': 'tac'
    },
    {
                'cityName': '兰州',
        'shortCut': 'lz'
    },
    {
                'cityName': '天水',
        'shortCut': 'tianshui'
    },
    {
                'cityName': '白银',
        'shortCut': 'by'
    },
    {
                'cityName': '庆阳',
        'shortCut': 'qingyang'
    },
    {
                'cityName': '平凉',
        'shortCut': 'pl'
    },
    {
                'cityName': '酒泉',
        'shortCut': 'jq'
    },
    {
                'cityName': '张掖',
        'shortCut': 'zhangye'
    },
    {
                'cityName': '武威',
        'shortCut': 'wuwei'
    },
    {
                'cityName': '定西',
        'shortCut': 'dx'
    },
    {
                'cityName': '金昌',
        'shortCut': 'jinchang'
    },
    {
                'cityName': '陇南',
        'shortCut': 'ln'
    },
    {
                'cityName': '临夏',
        'shortCut': 'linxia'
    },
    {
                'cityName': '嘉峪关',
        'shortCut': 'jyg'
    },
    {
                'cityName': '甘南',
        'shortCut': 'gn'
    },
    {
                'cityName': '银川',
        'shortCut': 'yinchuan'
    },
    {
                'cityName': '吴忠',
        'shortCut': 'wuzhong'
    },
    {
                'cityName': '石嘴山',
        'shortCut': 'szs'
    },
    {
                'cityName': '中卫',
        'shortCut': 'zw'
    },
    {
                'cityName': '固原',
        'shortCut': 'guyuan'
    },
    {
                'cityName': '西宁',
        'shortCut': 'xn'
    },
    {
                'cityName': '海西',
        'shortCut': 'hx'
    },
    {
                'cityName': '海北',
        'shortCut': 'haibei'
    },
    {
                'cityName': '果洛',
        'shortCut': 'guoluo'
    },
    {
                'cityName': '海东',
        'shortCut': 'haidong'
    },
    {
                'cityName': '黄南',
        'shortCut': 'huangnan'
    },
    {
                'cityName': '玉树',
        'shortCut': 'ys'
    },
    {
                'cityName': '海南',
        'shortCut': 'hainan'
    }
]";
            #endregion

            #region 插入配置数据
            dynamic lstCity = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(cityJsonString);
            if (lstCity != null)
            {
                var lst = new List<BizCrawlerConfiguration>();
                foreach (var cityInfo in lstCity)
                {
                    if (cityInfo?.cityName?.Value != null &&cityInfo.shortCut?.Value != null)
                    {
                        lst.Add(new BizCrawlerConfiguration()
                        {
                            ConfigurationKey = 0,
                            ConfigurationValue = $"{{'cityname':'{Convert.ToString(cityInfo.cityName.Value)}','shortcutname':'{Convert.ToString(cityInfo.shortCut.Value)}','pagecount':10}}",
                            ConfigurationName = ConstConfigurationName.PinPaiGongYu,
                            DataCreateTime = DateTime.Now,
                            IsEnabled = true,
                        });
                    }
                }
                DataContent.AddRange(lst);
                DataContent.SaveChanges();


            }

            #endregion

        }
    }
}
