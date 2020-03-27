using System;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 对json text读取
    /// </summary>
    public class SequenceStringReader
    {
        #region field

        /// <summary>
        /// 当前索引
        /// </summary>
        private int head;

        /// <summary>
        /// 字符串
        /// </summary>
        private readonly string json;

        /// <summary>
        /// 当前索引标识
        /// </summary>
        private int started;

        /// <summary>
        /// 中断一次
        /// </summary>
        private bool breaking;

        /// <summary>
        /// 转义符中
        /// </summary>
        private bool escaping;

        /// <summary>
        /// 虚拟读取，实际推进器不会前进
        /// </summary>
        private int invented = 0;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="json"></param>
        public SequenceStringReader(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException("json is empty");

            this.json = json;
            this.head = -1;
            this.started = -1;
        }

        #endregion ctor

        #region read

        /// <summary>
        /// 读取当前
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            if (breaking)
            {
                breaking = false;
                return false;
            }

            if (!this.CanRead())
                return false;

            this.head++;
            return true;
        }

        /// <summary>
        /// 读取n次
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public char[] Read(int time)
        {
            if (this.head + time > this.json.Length - 1)
                return null;

            var buffer = new char[time];
            var i = 0;
            do
            {
                buffer[i] = this.Current;
                if (Read())
                    i++;
                else
                    return null;
            } while (i < time);

            return buffer;
        }

        /// <summary>
        /// 是否可以读取当前
        /// </summary>
        public bool CanRead()
        {
            if (breaking)
                return false;

            if (this.head >= json.Length - 1)
                return false;

            return true;
        }

        /// <summary>
        /// 是否可以读取其中某个值
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        public bool CanRead(int head)
        {
            if (breaking)
                return false;

            if (head >= json.Length - 1)
                return false;

            return true;
        }

        /// <summary>
        /// 当前不可读
        /// </summary>
        /// <returns></returns>
        public bool CannotRead()
        {
            if (breaking)
                return true;

            if (this.head < 0)
                return true;

            if (this.head > json.Length - 1)
                return true;

            return false;
        }

        /// <summary>
        /// 当前不可读
        /// </summary>
        /// <returns></returns>
        public bool CannotRead(int head)
        {
            if (breaking)
                return true;

            if (head < 0)
                return true;

            if (head > json.Length - 1)
                return true;

            return false;
        }

        /// <summary>
        /// 当前字符是否匹配
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public bool CurrentMatch(char compare)
        {
            var current = this.Current;
            return current == compare;
        }

        /// <summary>
        /// 下一个字符是否匹配
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public bool NextMatch(char compare)
        {
            var next = this.Next;
            return next.HasValue && next.Value == compare;
        }

        /// <summary>
        /// 暂时中断
        /// </summary>
        public void Break()
        {
            breaking = true;
            return;
        }

        /// <summary>
        /// 移动到下一个节点
        /// </summary>
        /// <returns></returns>
        public SequenceStringReader Move()
        {
            this.Read();
            return this;
        }

        /// <summary>
        /// 回滚到上一个节点
        /// </summary>
        /// <returns></returns>
        public SequenceStringReader Rollback()
        {
            if (this.head > 0)
                this.head--;

            return this;
        }

        /// <summary>
        /// 回滚到上一个节点
        /// </summary>
        /// <returns></returns>
        public SequenceStringReader Rollback(int step)
        {
            while (step > 0)
            {
                step--;
                Rollback();
            }

            return this;
        }

        /// <summary>
        /// 虚拟读取，实际推进器不会前进
        /// </summary>
        /// <returns></returns>
        public SequenceStringReader InventedRead()
        {
            this.invented++;
            return this;
        }

        /// <summary>
        /// 释放虚拟读取
        /// </summary>
        public void DisposeInvented()
        {
            this.invented = 0;
        }

        /// <summary>
        /// 将当前索引标识
        /// </summary>
        public void Start(int head)
        {
            if (this.started > -1)
                return;

            this.started = head <= 0 ? 0 : head;
        }

        /// <summary>
        /// 清空当前标签并且将当前索引标识
        /// </summary>
        public void ClearToStart(int head)
        {
            this.ClearStart();

            if (this.started > -1)
                return;

            this.started = head <= 0 ? 0 : head;
        }

        /// <summary>
        /// 将当前标识索引重置
        /// </summary>
        public void ClearStart()
        {
            this.started = -1;
        }

        /// <summary>
        /// 清空重新读取
        /// </summary>
        public SequenceStringReader Reset()
        {
            this.head = -1;
            this.started = -1;
            this.breaking = false;
            this.escaping = false;
            return this;
        }

        /// <summary>
        /// 转义符
        /// </summary>
        public void Escaped()
        {
            this.escaping = true;
        }

        /// <summary>
        /// 清空转义符
        /// </summary>
        public void ClearEscaping()
        {
            this.escaping = false;
        }

        #endregion read

        #region prop

        /// <summary>
        /// 当前头部
        /// </summary>
        public int Head
        {
            get
            {
                return this.head;
            }
        }

        /// <summary>
        /// 总长度
        /// </summary>
        public int Length
        {
            get
            {
                return this.json.Length;
            }
        }

        /// <summary>
        /// 当前标识索引
        /// </summary>
        public int Started
        {
            get
            {
                return this.started;
            }
        }

        /// <summary>
        /// 获取当前字符
        /// </summary>
        public char Current
        {
            get
            {
                return this.json[this.head];
            }
        }

        /// <summary>
        /// 获取下一个字符但不移动位置
        /// </summary>
        public char? Next
        {
            get
            {
                if (this.head > this.json.Length - 2)
                    return null;

                return this.json[this.head + 1];
            }
        }

        /// <summary>
        /// 是否已经读取到尽头了
        /// </summary>
        public bool Ending
        {
            get
            {
                return this.head == this.json.Length - 1;
            }
        }

        /// <summary>
        /// 是否正在标识
        /// </summary>
        public bool Starting
        {
            get
            {
                return this.started > -1;
            }
        }

        /// <summary>
        /// 是否转义符中
        /// </summary>
        public bool Escaping
        {
            get
            {
                return this.escaping;
            }
        }

        /// <summary>
        /// 回滚步数
        /// </summary>
        public int RollbackStep { get; set; }

        #endregion prop

        #region string

        /// <summary>
        /// 原始字符串
        /// </summary>
        public string Original
        {
            get
            {
                return this.json;
            }
        }

        #endregion string

        #region tostring

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.json;
        }

        #endregion tostring

        #region util

        /// <summary>
        /// 是否为空格或换行
        /// </summary>
        /// <returns></returns>
        public bool IsWhiteSpaceChangeLine()
        {
            return this.IsWhiteSpaceChangeLine(this.Current);
        }

        /// <summary>
        /// 是否为空格或换行
        /// </summary>
        /// <param name="char">The character.</param>
        /// <returns></returns>
        public bool IsWhiteSpaceChangeLine(int @char)
        {
            //  \u0020  -    - 33  space
            //  \u0009  - \t - 9   tab
            //  \u000A  - \r - 13  new line
            //  \u000D  - \n - 10  carriage return

            return @char < 0x21 && (@char == 0x20 || @char == 0x09 || @char == 0x0A || @char == 0x0D);
        }

        /// <summary>
        /// 是否为换行
        /// </summary>
        /// <returns></returns>
        public bool IsChangeLine()
        {
            return this.IsChangeLine(this.Current);
        }

        /// <summary>
        /// 是否为换行
        /// </summary>
        /// <param name="char">The character.</param>
        /// <returns></returns>
        public bool IsChangeLine(int @char)
        {
            //  \u0020  -    - 33
            //  \u0009  - \t - 9
            //  \u000A  - \r - 13
            //  \u000D  - \n - 10

            return @char < 0x0E && (@char == 0x0A || @char == 0x0D);
        }

        /// <summary>
        /// 是否数字
        /// </summary>
        /// <returns></returns>
        public bool IsDigital()
        {
            return this.IsDigital(this.Current);
        }

        /// <summary>
        /// 是否数字
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public bool IsDigital(int chr)
        {
            /*
             *  \0x30  -    - 0
             *  \0x31  -    - 1
             *  \0x39  -    - 9
             */

            return chr >= 0x30 && chr <= 0x39;
        }


        /// <summary>
        /// 是否为空格
        /// </summary>
        /// <returns></returns>
        public bool IsWhiteSpace()
        {
            return this.IsWhiteSpace(this.Current);
        }

        /// <summary>
        /// 是否为空格
        /// </summary>
        /// <param name="char">The character.</param>
        /// <returns></returns>
        public bool IsWhiteSpace(int @char)
        {
            //  \u0020  -    - 33  space
            //  \u0009  - \t - 9   tab

            return @char < 0x21 && (@char == 0x20 || @char == 0x09);
        }

        #endregion util

        #region remove

        /// <summary>
        /// 忽略回车字符
        /// </summary>
        public bool SkipEnterResult()
        {
            if (this.IsChangeLine(this.Current))
            {
                while (this.Read())
                {
                    if (this.IsChangeLine(this.Current))
                        continue;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 忽略回车字符
        /// </summary>
        public int SkipEnterCount()
        {
            if (this.IsChangeLine(this.Current))
            {
                var i = 1;
                while (this.Read())
                {
                    if (this.IsChangeLine(this.Current))
                    {
                        i++;
                        continue;
                    }

                    return i;
                }

                return 1;
            }

            return 0;
        }

        /// <summary>
        /// 忽略空格回车字符
        /// </summary>
        public bool SkipSpaceEnterResult()
        {
            if (this.IsWhiteSpaceChangeLine(this.Current))
            {
                while (this.Read())
                {
                    if (this.IsWhiteSpaceChangeLine(this.Current))
                        continue;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 忽略空格回车字符
        /// </summary>
        public int SkipSpaceEnterCount()
        {
            if (this.IsWhiteSpaceChangeLine(this.Current))
            {
                var i = 1;
                while (this.Read())
                {
                    if (this.IsWhiteSpaceChangeLine(this.Current))
                    {
                        i++;
                        continue;
                    }

                    return i;
                }

                return 1;
            }

            return 0;
        }

        #endregion remove
    }
}