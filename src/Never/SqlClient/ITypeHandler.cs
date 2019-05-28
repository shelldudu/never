using System.Data;

namespace Never.SqlClient
{
    /// <summary>
    /// 类型处理者
    /// </summary>
    public interface ITypeHandler
    {
    }

    /// <summary>
    /// 类型处理者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICastingValueToParameterTypeHandler<out T> : ITypeHandler
    {
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="value">集合</param>
        /// <returns></returns>
        T ToParameter(object value);
    }

    /// <summary>
    /// 类型处理者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadingFromDataRecordToValueTypeHandler<out T> : ITypeHandler
    {
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="dataRecord">读取器</param>
        /// <param name="ordinal">column的位置，如果未-1表示没有找到这个值</param>
        /// <param name="columnName">行名字</param>
        /// <returns></returns>
        T ToValue(IDataRecord dataRecord, int ordinal, string columnName);
    }
}