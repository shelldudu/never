using Never.Attributes;

namespace Never.Web
{
    /// <summary>
    /// 水印地址
    /// </summary>
    public enum WatermarkType
    {
        /// <summary>
        /// 左上
        /// </summary>
        [Remark(Name = "左上")]
        左上 = 1,

        /// <summary>
        /// 中上
        /// </summary>
        [Remark(Name = "左中")]
        中上 = 2,

        /// <summary>
        /// 右上
        /// </summary>
        [Remark(Name = "右上角")]
        右上 = 3,

        /// <summary>
        /// 左中
        /// </summary>
        [Remark(Name = "左上")]
        左中 = 4,

        /// <summary>
        /// 中中
        /// </summary>
        [Remark(Name = "中中")]
        中中 = 5,

        /// <summary>
        /// 右中
        /// </summary>
        [Remark(Name = "右中")]
        右中 = 6,

        /// <summary>
        /// 左上角
        /// </summary>
        [Remark(Name = "左下")]
        左下 = 7,

        /// <summary>
        /// 右下
        /// </summary>
        [Remark(Name = "中下")]
        中下 = 8,

        /// <summary>
        /// 右下
        /// </summary>
        [Remark(Name = "右下")]
        右下 = 9
    }
}