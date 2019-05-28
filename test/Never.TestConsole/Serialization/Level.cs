using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Never.TestConsole.Serialization
{
    public class OneLevel
    {
        public int Id { get; set; }
    }

    public class MachineClass
    {
        public Machine Machine { get; set; }
    }

    public class TwoLevel
    {
        public int Name { get; set; }

        public string U { get; set; }

        public ThreeLevel? Three { get; set; }
    }

    public class GetBankCardResp
    {
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string CardMobile { get; set; }
        public string CardNumber { get; set; }
        public string IDCard { get; set; }
        public bool IsQPCard { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
    }

    public class TwoLevelCopy
    {
        public int Name { get; set; }

        public string U { get; set; }

        public ThreeLevel? Three { get; set; }
    }

    public struct ThreeLevel
    {
        public string ABC
        {
            get;
            set;
        }

        public int Id { get; set; }
    }

    public class EnumClass
    {
        public ABC? Name { get; set; }
    }

    public class ArrayDeci
    {
        public IEnumerable<DeviceInfoModel> DeviceInfoList { get; set; }
    }

    public class DeviceInfoModel
    {
        ///// <summary>
        ///// 唯一标识
        ///// </summary>
        //public Guid AggregateId { get; set; }

        ///// <summary>
        ///// 借款ID
        ///// </summary>
        //public Guid LoanId { get; set; }

        /// <summary>
        /// 机型
        /// </summary>
        public string MobileMode { get; set; }

        /// <summary>
        /// IMEI号码
        /// </summary>
        public string IMEI { get; set; }

        /// <summary>
        /// 网络
        /// </summary>
        public string Network { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; set; }
    }

    public class MyDict : Dictionary<string, object>
    {
    }

    public class MyList : List<int>
    {
    }

    /// <summary>
    /// 获取第三方平台的全局唯一票据结果模型。
    /// </summary>
    public sealed class AccessTokenModel
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的第三方平台的全局唯一票据。
        /// </summary>
        internal AccessTokenModel()
        {
            CreateTime = DateTime.Now;
        }

        #endregion Constructor

        /// <summary>
        /// 第三方平台access_token。
        /// </summary>
        [JsonProperty("component_access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 凭证有效时间（秒）。
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// 是否过期。
        /// </summary>
        /// <returns>如果过期返回true，否则返回false。</returns>
        public bool IsExpired()
        {
            return CreateTime.AddSeconds(ExpiresIn - 20/*不采用最后的期限作为判断，防止在很少的时间内到期导致后续的逻辑无法执行*/) <= DateTime.Now;
        }
    }

    public class PackageListReqs : PagedSearch
    {
        /// <summary>
        /// 是否推荐
        /// </summary>
        public bool? IsRecommand { get; set; }

        /// <summary>
        /// 项目状态
        /// </summary>
        public int[] Status { get; set; }

        /// <summary>
        /// 最低利率（包含）
        /// </summary>
        public decimal? MinRate { get; set; }

        /// <summary>
        /// 最高利率（包含）
        /// </summary>
        public decimal? MaxRate { get; set; }

        /// <summary>
        /// 最低期限（包含）
        /// </summary>
        public decimal? MinDuration { get; set; }

        /// <summary>
        /// 最高期限（包含）
        /// </summary>
        public decimal? MaxDuration { get; set; }

        /// <summary>
        /// 是否新人可投
        /// </summary>
        public bool? OnlyNewMan { get; set; }

        /// <summary>
        /// 排序键值对(key:排序字段，value:升序降序 asc|desc)
        /// </summary>
        public Dictionary<string, string> OrderBy { get; set; }
    }

    public class GuidSource
    {
        public Guid AggregateId { get; set; }
    }

    public class GuidTarget
    {
        public object AggregateId { get; set; }
    }
}