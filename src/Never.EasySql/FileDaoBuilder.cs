using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// file资源方式的Sql
    /// </summary>
    [Obsolete("当前可以根据xml内容构建，也可以直接写sql，还可以写linq方式实现sql执行，该对象有奇异，可以选择DynamicDaoBuilder或XmlContentDaoBuilder")]
    public abstract class FileDaoBuilder : XmlContentDaoBuilder.XmlFileDaoBuilder
    {
    }
}