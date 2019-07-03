using System;

namespace Never.Mappers
{
    /// <summary>
    /// 自动映射
    /// </summary>
    public class EasyMapper : IMapper
    {
        #region ctor and field

        private MapperSetting setting;

        /// <summary>
        ///
        /// </summary>
        public EasyMapper() : this(new MapperSetting(string.Empty))
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="setting"></param>
        public EasyMapper(MapperSetting setting)
        {
            this.setting = setting;
        }

        #endregion ctor and field

        #region imapper

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <returns></returns>
        To IMapper.Map<From, To>(From from)
        {
            return Map<From, To>(from, null, this.setting);
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="callBack">当完成映射后的回调</param>
        /// <returns></returns>
        To IMapper.Map<From, To>(From from, Action<From, To> callBack)
        {
            return Map(from, callBack, this.setting);
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="target">目标对象，请实例化该对象</param>
        /// <returns></returns>
        To IMapper.Map<From, To>(From from, To target)
        {
            return Map(from, target, null, this.setting);
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="target">目标对象，请实例化该对象</param>
        /// <param name="callBack">当完成映射后的回调</param>
        /// <returns></returns>
        To IMapper.Map<From, To>(From from, To target, Action<From, To> callBack)
        {
            return Map(from, target, callBack, this.setting);
        }

        #endregion imapper

        #region map

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <returns></returns>
        public static To Map<From, To>(From from)
        {
            return Map<From, To>(from, null);
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="callBack">当完成映射后的回调</param>
        /// <returns></returns>
        public static To Map<From, To>(From from, Action<From, To> callBack)
        {
            return Map(from, callBack, new MapperSetting(string.Empty));
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="callBack">当完成映射后的回调</param>
        /// <param name="setting">配置项目</param>
        /// <returns></returns>
        internal static To Map<From, To>(From from, Action<From, To> callBack, MapperSetting setting)
        {
            var func = MapperBuilder<From, To>.FuncBuild(setting);
            var to = func(from, new MapperContext() { Setting = setting });
            callBack?.Invoke(from, to);
            return to;
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="callBack">当完成映射后的回调</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        internal static To Map<From, To>(From from, Action<From, To> callBack, MapperContext context)
        {
            var func = MapperBuilder<From, To>.FuncBuild(context.Setting);
            var to = func(from, context);
            callBack?.Invoke(from, to);
            return to;
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="target">目标对象，请实例化该对象</param>
        /// <returns></returns>
        public static To Map<From, To>(From from, To target)
        {
            return Map(from, target, null);
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="target">目标对象，请实例化该对象</param>
        /// <param name="callBack">当完成映射后的回调</param>
        /// <returns></returns>
        public static To Map<From, To>(From from, To target, Action<From, To> callBack)
        {
            return Map(from, target, callBack, new MapperSetting(string.Empty) { AlwaysNewTraget = true });
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="target">目标对象，请实例化该对象</param>
        /// <param name="callBack">当完成映射后的回调</param>
        /// <param name="setting">配置项目</param>
        /// <returns></returns>
        internal static To Map<From, To>(From from, To target, Action<From, To> callBack, MapperSetting setting)
        {
            var action = MapperBuilder<From, To>.ActionBuild(setting);
            action(from, new MapperContext() { Setting = setting }, target);
            callBack?.Invoke(from, target);
            return target;
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="target">目标对象，请实例化该对象</param>
        /// <param name="callBack">当完成映射后的回调</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        internal static To Map<From, To>(From from, To target, Action<From, To> callBack, MapperContext context)
        {
            var action = MapperBuilder<From, To>.ActionBuild(context.Setting);
            action(from, context, target);
            callBack?.Invoke(from, target);
            return target;
        }

        #endregion map
    }
}