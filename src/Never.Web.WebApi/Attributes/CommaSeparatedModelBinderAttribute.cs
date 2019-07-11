using System;

namespace Never.Web.WebApi.Attributes
{

#if !NET461
    /// <summary>
    /// 逗号分割绑定类型
    /// </summary>
    public class CommaSeparatedModelBinderAttribute : Microsoft.AspNetCore.Mvc.ModelBinderAttribute
    {
        public CommaSeparatedModelBinderAttribute() : base(typeof(CommaSeparatedModelBinder))
        {

        }
    }
#else
    /// <summary>
    /// 逗号分割绑定类型
    /// </summary>
    public class CommaSeparatedModelBinderAttribute : System.Web.Http.ModelBinding.CustomModelBinderAttribute
    {
        /// <summary>
        /// 返回Model绑定接口对象
        /// </summary>
        /// <returns></returns>
        public override System.Web.Http.ModelBinding.IModelBinder GetBinder()
        {
            return new CommaSeparatedModelBinder();
        }
    }
#endif
}