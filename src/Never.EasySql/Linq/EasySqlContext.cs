using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    public class EasySqlContext
    {
        #region prop

        public string SqlId { get; protected set; }

        #endregion prop

        #region build

        /// <summary>
        ///
        /// </summary>
        protected void BeginBuilding()
        {
            if (this.SqlId.IsNullOrEmpty())
                this.SqlId = NewId.GenerateString(NewId.StringLength.L24);
        }

        #endregion build
    }
}