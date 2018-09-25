
namespace HouseMap.Common
{
    /// <summary>
    /// 颜色枚举
    /// </summary>
    public static class LocationMarkBGType
    {
        /// <summary>
        /// 蓝色
        /// </summary>
        public static string Blue { get; } = "Blue.png";

        /// <summary>
        /// 浅绿
        /// </summary>
        public static string PaleGreen { get; } = "PaleGreen.png";

        /// <summary>
        /// 明绿
        /// </summary>
        public static string LightGreen { get; } = "LightGreen.png";

        /// <summary>
        /// 浅黄
        /// </summary>
        public static string PaleYellow { get; } = "PaleYellow.png";

        /// <summary>
        /// 橙黄
        /// </summary>
        public static string OrangeYellow { get; } = "OrangeYellow.png";

        /// <summary>
        /// 
        /// </summary>
        public static string PaleRed { get; } = "PaleRed.png";

        /// <summary>
        /// 红色
        /// </summary>
        public static string Red { get; } = "Red.png";

        /// <summary>
        /// 粉红
        /// </summary>
        public static string Pink { get; } = "Pink.png";

        /// <summary>
        /// 紫色
        /// </summary>
        public static string Violet { get; } = "Violet.png";

        /// <summary>
        /// 黑色
        /// </summary>
        public static string Black { get; } = "Black.png";

        public static string SelectColor(int num)
        {
            switch(num)
            {
                case 0: return Blue;
                case 1:return PaleGreen;
                case 2:return LightGreen;
                case 3:return PaleYellow;
                case 4:return OrangeYellow;
                case 5:return PaleRed;
                case 6:return Red;
                case 7:return Pink;
                case 8:return Violet;
                default:
                    return Black;
            }
        }
    }
}