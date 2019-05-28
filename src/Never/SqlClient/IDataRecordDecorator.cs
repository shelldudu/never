using System;
using System.Collections.Generic;
using System.Data;

namespace Never.SqlClient
{
    /// <summary>
    /// 因为GetOrdinal去获取过程中没有相应的Name,会出异常，但是因为reader已经被emit创建好
    /// </summary>
    internal class IDataRecordDecorator : IDataRecord
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private IDataRecord dataRecord;

        /// <summary>
        ///
        /// </summary>
        private IDictionary<string, int> field;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        public IDataRecordDecorator(IDataReader reader)
        {
            this.dataRecord = null;
            if (reader != null && reader.FieldCount >= 0)
            {
                this.field = new Dictionary<string, int>(reader.FieldCount);
                for (var i = 0; i < reader.FieldCount; i++)
                    field[reader.GetName(i)] = i;
            }
            else
            {
                this.field = new Dictionary<string, int>(0);
            }
        }

        #endregion ctor

        #region load

        internal IDataRecordDecorator Load(IDataRecord dataRecord)
        {
            this.dataRecord = dataRecord;
            return this;
        }

        #endregion load

        #region IDataRecord

        public object this[string name]
        {
            get
            {
                return dataRecord[name];
            }
        }

        public object this[int i]
        {
            get
            {
                return dataRecord[i];
            }
        }

        public int FieldCount
        {
            get
            {
                return dataRecord.FieldCount;
            }
        }

        public IDataReader GetData(int i)
        {
            return dataRecord.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return dataRecord.GetDataTypeName(i);
        }

        public int GetOrdinal(string name)
        {
            int ordinal = -1;
            if (this.field.TryGetValue(name, out ordinal))
                return ordinal;

            return -1;
        }

        public int GetValues(object[] values)
        {
            return dataRecord.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return dataRecord.IsDBNull(i);
        }

        public Type GetFieldType(int i)
        {
            return dataRecord.GetFieldType(i);
        }

        public string GetName(int i)
        {
            return dataRecord.GetName(i);
        }

        #endregion IDataRecord

        #region type

        public object GetValue(int i)
        {
            return dataRecord.GetValue(i);
        }

        public byte GetByte(int i)
        {
            return dataRecord.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return dataRecord.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public bool GetBoolean(int i)
        {
            return dataRecord.GetBoolean(i);
        }

        public char GetChar(int i)
        {
            return dataRecord.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return dataRecord.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public DateTime GetDateTime(int i)
        {
            return dataRecord.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return dataRecord.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return dataRecord.GetDouble(i);
        }

        public float GetFloat(int i)
        {
            return dataRecord.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return dataRecord.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return dataRecord.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return dataRecord.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return dataRecord.GetInt64(i);
        }

        public string GetString(int i)
        {
            return dataRecord.GetString(i);
        }

        #endregion type
    }
}