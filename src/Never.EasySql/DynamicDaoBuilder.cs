using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 动态写sql语句，并不像<see cref="XmlContentDaoBuilder"/>那样先构建好再查询<see cref="SqlTag"/>去执行
    /// </summary>
    public abstract class DynamicDaoBuilder : BaseDaoBuilder
    {
        /// <summary>
        /// 初始化sqlTag提供者
        /// </summary>
        /// <returns></returns>
        public override ISqlTagProvider InitSqlTagProviderOnStart()
        {
            var provider = new SqlTagProvider();
            return provider;
        }
    }
}
