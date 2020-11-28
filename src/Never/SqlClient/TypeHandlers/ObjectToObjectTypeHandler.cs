using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.SqlClient.TypeHandlers
{
    /// <summary>
    /// long to string
    /// </summary>
    public class LongStringTypeHandler : ICastingValueToParameterTypeHandler<string>, IReadingFromDataRecordToValueTypeHandler<long>
    {
        string ICastingValueToParameterTypeHandler<string>.ToParameter(object value)
        {
            return value.ToString();
        }

        long IReadingFromDataRecordToValueTypeHandler<long>.ToValue(IDataRecord dataRecord, int ordinal, string columnName)
        {
            var value = dataRecord.GetString(ordinal);
            return value == null ? 0 : value.AsLong();
        }
    }

    /// <summary>
    /// integer to string
    /// </summary>
    public class IntegerStringTypeHandler : ICastingValueToParameterTypeHandler<string>, IReadingFromDataRecordToValueTypeHandler<long>
    {
        string ICastingValueToParameterTypeHandler<string>.ToParameter(object value)
        {
            return value.ToString();
        }

        long IReadingFromDataRecordToValueTypeHandler<long>.ToValue(IDataRecord dataRecord, int ordinal, string columnName)
        {
            var value = dataRecord.GetString(ordinal);
            return value == null ? 0 : value.AsLong();
        }
    }

    /// <summary>
    /// long to int
    /// </summary>
    public class LongIntegerTypeHandler : ICastingValueToParameterTypeHandler<int>, IReadingFromDataRecordToValueTypeHandler<long>
    {
        int ICastingValueToParameterTypeHandler<int>.ToParameter(object value)
        {
            return (int)value;
        }

        long IReadingFromDataRecordToValueTypeHandler<long>.ToValue(IDataRecord dataRecord, int ordinal, string columnName)
        {
            var value = dataRecord.GetInt32(ordinal);
            return value;
        }
    }

    /// <summary>
    /// char[] to string
    /// </summary>
    public class CharArrayStringTypeHandler : ICastingValueToParameterTypeHandler<string>, IReadingFromDataRecordToValueTypeHandler<char[]>
    {
        string ICastingValueToParameterTypeHandler<string>.ToParameter(object value)
        {
            return new string((char[])value);
        }

        char[] IReadingFromDataRecordToValueTypeHandler<char[]>.ToValue(IDataRecord dataRecord, int ordinal, string columnName)
        {
            var value = dataRecord.GetString(ordinal);
            return value == null ? new char[0] : value.ToCharArray();
        }
    }
}
