using Never.Attributes;
using Never.DataAnnotations;
using Never.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.TestConsole
{
    public class ProxyTest : Program
    {
        public void TestTaskProxy()
        {
            var a10 = Never.Deployment.StartupExtension.StartReport();
            var provider = new ApiRouteProvider(a10);
            a10.Startup(60, new[] { provider });
            var type = Never.Deployment.HttpServiceProxyFactory.CreateProxy(typeof(IAccountService), () => new ApiUriDispatcher<ApiRouteProvider>(provider, a10), null, null);
            var service = (IAccountService)this.ServiceActivator.CreateService(type).Value;
            var task = service.CreateAccount(6);
            var result = task.GetAwaiter().GetResult();

            // var t = result.Result;
            //Console.WriteLine(t);
        }


        public interface IAccountService
        {
            /// <summary>
            /// 创建账户
            /// </summary>
            /// <param name="reqs"></param>
            /// <returns></returns>
            [ApiActionRemark("2578a9b22081", "HttpPost")]
            Task<ApiResult<string>> CreateAccount(long userid);

            [ApiActionRemark("aa04013d823c", "HttpPost")]
            ApiResult<int> CountAllBankCard(BankCardReqs reqs);
        }

        /// <summary>
        /// 资金请求
        /// </summary>
        [Validator(typeof(RequestValidator))]
        public class CapitalReqs : IRoutePrimaryKeySelect
        {
            #region prop

            /// <summary>
            /// 用户Id
            /// </summary>
            public long UserId
            {
                get; set;
            }

            /// <summary>
            /// 路由
            /// </summary>
            string IRoutePrimaryKeySelect.PrimaryKey
            {
                get
                {
                    return this.UserId.ToString();
                }
            }

            #endregion prop

            #region validator

            /// <summary>
            /// 创建用户命令验证
            /// </summary>
            private class RequestValidator : Validator<CapitalReqs>
            {
                public override IEnumerable<KeyValuePair<Expression<Func<CapitalReqs, object>>, string>> RuleFor(CapitalReqs target)
                {
                    if (target.UserId <= 0)
                    {
                        yield return new KeyValuePair<Expression<Func<CapitalReqs, object>>, string>(model => model.UserId, "UserId为空");
                    }
                }
            }

            #endregion validator
        }

        /// <summary>
        /// 绑卡查询
        /// </summary>
        [Validator(typeof(RequestValidator))]
        public class BankCardReqs : IRoutePrimaryKeySelect
        {
            /// <summary>
            /// 用户Id
            /// </summary>
            public long UserId { get; set; }

            /// <summary>
            /// 银行卡号
            /// </summary>
            public string BankCard { get; set; }

            /// <summary>
            /// 是否禁用的
            /// </summary>
            public bool? Disabled { get; set; }

            string IRoutePrimaryKeySelect.PrimaryKey => this.UserId.ToString();

            #region validator

            /// <summary>
            /// 创建用户命令验证
            /// </summary>
            private class RequestValidator : Validator<BankCardReqs>
            {
                public override IEnumerable<KeyValuePair<Expression<Func<BankCardReqs, object>>, string>> RuleFor(BankCardReqs target)
                {
                    if (target.UserId <= 0)
                    {
                        yield return new KeyValuePair<Expression<Func<BankCardReqs, object>>, string>(model => model.UserId, "用户Id小于0");
                    }
                }
            }

            #endregion validator
        }

        /// <summary>
        /// 路由提供者
        /// </summary>
        /// <seealso cref="DefaultApiRouteProvider" />
        [Never.Attributes.Summary(Descn = "读取配置文件中\"CapitalA10\": {\"url\": [ \"http://127.0.0.1/api/\", \"http://127.0.0.1/api/\" ],\"ping\": [ \"http://127.0.0.1/a10\", \"http://127.0.0.1/a10\" ]}")]
        public class ApiRouteProvider : DefaultApiRouteProvider
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ApiRouteProvider"/> class.
            /// </summary>
            public ApiRouteProvider(IA10HealthReport a10HealthReport)
            {
            }

            /// <summary>
            /// 获取A10资源信息
            /// </summary>
            public override IEnumerable<ApiUrlA10Element> ApiUrlA10Elements
            {
                get
                {
                    yield return new ApiUrlA10Element() { A10Url = "http://localhost:59172/a10.html", ApiUrl = "http://localhost:59172/api/" };
                }
            }
        }
    }
}
