﻿using Never.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Never.Mappers
{
    /// <summary>
    /// 自动映射创建
    /// </summary>
    public partial class MapperBuilder<From, To> : MapperBuilder
    {
        #region field and ctor

        private static ConcurrentDictionary<MapperSetting, Func<From, To>> funcCaching = null;

        private static ConcurrentDictionary<MapperSetting, Action<From, To>> actionCaching = null;

        static MapperBuilder()
        {
            funcCaching = new ConcurrentDictionary<MapperSetting, Func<From, To>>(new MapperSetting());
            actionCaching = new ConcurrentDictionary<MapperSetting, Action<From, To>>(new MapperSetting());
        }

        #endregion field and ctor
    }
}