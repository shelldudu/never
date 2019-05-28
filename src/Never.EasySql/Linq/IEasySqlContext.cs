using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    public interface IEasySqlContext
    {
        string SqlId { get; }

        void Build();
    }
}