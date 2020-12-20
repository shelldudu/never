using Never.Exceptions;
using Never.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Never.EasySql
{
    /// <summary>
    /// 嵌入资源方式的Sql
    /// </summary>
    [Obsolete("当前可以根据xml内容构建，也可以直接写sql，还可以写linq方式实现sql执行，该对象有歧义，可以选择DynamicDaoBuilder或XmlContentDaoBuilder.XmlEmbeddedDaoBuilder或XmlContentDaoBuilder.XmlFileDaoBuilder")]
    public abstract class EmbeddedDaoBuilder : XmlContentDaoBuilder.XmlEmbeddedDaoBuilder
    {
    }
}