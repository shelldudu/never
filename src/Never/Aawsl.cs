using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never
{
    /// <summary>
    /// 带返回值的委托
    /// </summary>
    public delegate TResult ResultEventHandler<in TEventArgs, out TResult>(object sender, TEventArgs e) where TEventArgs : EventArgs;

    /// <summary>
    /// 带返回值的委托
    /// </summary>
    public delegate TResult ResultEventHandler<in TEventArgs1, in TEventArgs2, out TResult>(object sender, TEventArgs1 e1, TEventArgs2 e2) where TEventArgs1 : EventArgs where TEventArgs2 : EventArgs;

    /// <summary>
    /// 委托
    /// </summary>
    public delegate void VoidEventHandler<in TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;

    /// <summary>
    /// 委托
    /// </summary>
    public delegate void VoidEventHandler<in TEventArgs1, in TEventArgs2>(object sender, TEventArgs1 e1, TEventArgs2 e2) where TEventArgs1 : EventArgs where TEventArgs2 : EventArgs;
}
