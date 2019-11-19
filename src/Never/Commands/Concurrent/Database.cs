using System;
using System.Collections.Generic;

namespace Never.Commands.Concurrent
{
    /// <summary>
    /// 类似数据库的管理
    /// </summary>
    internal class Database
    {
        #region dict

        /// <summary>
        /// 存储每一个命令的表行记录对象
        /// </summary>
        private static readonly IDictionary<Type, Table> collections = new System.Collections.Concurrent.ConcurrentDictionary<Type, Table>();

        #endregion dict

        /// <summary>
        /// 创建一行TableRow
        /// </summary>
        /// <param name="serialeCommandType"></param>
        public static Table Create(Type serialeCommandType)
        {
            var table = new Table();
            collections[serialeCommandType] = table;
            return table;
        }

        /// <summary>
        /// 查询某一Table对象
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static Table Use(ISerialCommand command)
        {
            if (collections.Count == 0)
                return Create(command.GetType());

            return collections[command.GetType()];
        }

        /// <summary>
        /// 获取命令个数
        /// </summary>
        /// <returns></returns>
        public static int CommandCount()
        {
            var count = 0;
            foreach (var i in collections)
            {
                count += i.Value.CommandCount;
            }
            return count;
        }
    }
}