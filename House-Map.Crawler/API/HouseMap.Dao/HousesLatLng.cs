namespace HouseMap.Dao
{

    public class HousesLatLng
    {
        /// <summary>
        /// 房源Id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string longitude { get; set; }

        /// <summary>
        /// 维度
        /// </summary>
        public string latitude { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 房源URL
        /// </summary>
        public string onlineURL { get; set; }

    }
}