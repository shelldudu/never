using Never.Aop.DomainFilters;
using Never.Commands;
using Never.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Never.EventStreams
{
    /// <summary>
    /// 事件分析者
    /// </summary>
    public class DefaultEventStreamAnalyser : IEventStreamAnalyser
    {
        #region helper

        /// <summary>
        /// 事件助手
        /// </summary>
        protected class EventStreamHelper
        {
            /// <summary>
            /// Guid类型的事件
            /// </summary>
            public List<IAggregateRootEvent<Guid>> Guids { get; set; }

            /// <summary>
            /// Int类型的事件
            /// </summary>
            public List<IAggregateRootEvent<int>> Ints { get; set; }

            /// <summary>
            /// String类型的事件
            /// </summary>
            public List<IAggregateRootEvent<string>> Strings { get; set; }

            /// <summary>
            /// Long类型的事件
            /// </summary>
            public List<IAggregateRootEvent<long>> Longs { get; set; }

            /// <summary>
            /// 其他类型
            /// </summary>
            public List<IEvent> Others { get; set; }

            /// <summary>
            ///
            /// </summary>
            public EventStreamHelper(int count)
            {
                this.Guids = new List<IAggregateRootEvent<Guid>>(count);
                this.Ints = new List<IAggregateRootEvent<int>>(count);
                this.Strings = new List<IAggregateRootEvent<string>>(count);
                this.Longs = new List<IAggregateRootEvent<long>>(count);
                this.Others = new List<IEvent>(count);
            }
        }

        #endregion helper

        #region field

        /// <summary>
        ///
        /// </summary>
        private static readonly IDictionary<Type, int> streamTypes = new Dictionary<Type, int>()
        {
            { typeof(Guid), 1 },
            { typeof(Int32), 2 },
            { typeof(long), 3 },
            { typeof(string), 4}
        };

        #endregion field

        #region 分析

        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="context">命令上下文</param>
        /// <param name="events">事件</param>
        public virtual IEnumerable<IOperateEvent> Analyse(ICommandContext context, IEnumerable<KeyValuePair<Type, IEvent[]>> events)
        {
            var source = new List<IOperateEvent>();
            if (events == null)
                return source;

            foreach (var e in events)
            {
                var eventHelper = this.FindEventHelper(e.Value);
                if (eventHelper == null)
                    return source;

                if (eventHelper.Guids.Count > 0)
                {
                    foreach (var ge in eventHelper.Guids)
                    {
                        source.Add(FindOperateEvent(e.Key, ge));
                    }
                }

                if (eventHelper.Longs.Count > 0)
                {
                    foreach (var le in eventHelper.Longs)
                    {
                        source.Add(FindOperateEvent(e.Key, le));
                    }
                }

                if (eventHelper.Ints.Count > 0)
                {
                    foreach (var ie in eventHelper.Ints)
                    {
                        source.Add(FindOperateEvent(e.Key, ie));
                    }
                }

                if (eventHelper.Others.Count > 0)
                    this.Append(source, e.Key, eventHelper.Others);
            }

            return source;
        }

        /// <summary>
        /// Appends the specified events.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="aggregateType"></param>
        /// <param name="events">The events.</param>
        protected void Append(List<IOperateEvent> source, Type aggregateType, List<IEvent> events)
        {
            if (events == null)
                return;

            foreach (var e in events)
            {
                source.Add(FindOperateEvent(aggregateType, e));
            }
        }

        #endregion 分析

        #region utils

        /// <summary>
        /// 查找是聚合根操作事件类型的事件
        /// </summary>
        /// <param name="events">事件列表</param>
        /// <returns></returns>
        protected virtual EventStreamHelper FindEventHelper(ICollection<IEvent> events)
        {
            if (events == null)
                return new EventStreamHelper(0);

            var helper = new EventStreamHelper(events.Count);
            foreach (var e in events)
            {
                /*使用as性能很快*/
                /*基本所有的聚合根都是Guid的，所以先用guid去匹配*/
                var guid = e as IAggregateRootEvent<Guid>;
                if (guid != null)
                {
                    helper.Guids.Add(guid);
                    continue;
                }
                var lng = e as IAggregateRootEvent<long>;
                if (lng != null)
                {
                    helper.Longs.Add(lng);
                    continue;
                }

                var str = e as IAggregateRootEvent<string>;
                if (str != null)
                {
                    helper.Strings.Add(str);
                    continue;
                }

                var it = e as IAggregateRootEvent<int>;
                if (it != null)
                {
                    helper.Ints.Add(it);
                    continue;
                }

                helper.Others.Add(e);
            };

            return helper;
        }

        /// <summary>
        /// 获取事件助手
        /// </summary>
        /// <param name="aggregateType"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual DefaultOperateEvent FindOperateEvent(Type aggregateType, IEvent e)
        {
            var type = e.GetType();
            var attrbute = EventBehaviorStorager.Default.GetAttribute<EventDomainAttribute>(type);
            string domain = attrbute == null ? "" : attrbute.Domain;

            PropertyInfo aggregateRootIdProp = type.GetProperty("AggregateId");
            if (aggregateRootIdProp == null)
                aggregateRootIdProp = type.GetProperty("UniqueId");

            if (aggregateRootIdProp == null || !streamTypes.ContainsKey(aggregateRootIdProp.PropertyType))
            {
                return new DefaultOperateEvent()
                {
                    AggregateType = aggregateType,
                    Event = e,
                    EventType = Regex.Replace(type.AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase),
                    EventTypeFullName = type.FullName,
                    HashCode = e.GetHashCode(),
                    Increment = DefaultOperateEvent.NextIncrement,
                    AppDomain = domain,
                    CreateDate = new DateTime(2000, 1, 1, 0, 0, 0),
                    Creator = "es",
                };
            }

            var uniqueId = aggregateRootIdProp.GetValue(e, null);

            var createDateProp = type.GetProperty("CreateDate");
            var createDate = createDateProp == null ? null : createDateProp.GetValue(e, null);
            var creatorProp = type.GetProperty("Creator");
            var creator = creatorProp == null ? null : creatorProp.GetValue(e, null);

            var result = new DefaultOperateEvent()
            {
                AggregateType = aggregateType,
                Event = e,
                AppDomain = domain,
                EventType = Regex.Replace(type.AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase),
                EventTypeFullName = type.FullName,
                AggregateId = uniqueId == null ? "" : uniqueId.ToString(),
                AggregateIdType = aggregateRootIdProp.PropertyType,
                Version = e.Version,
                CreateDate = new DateTime(2000, 1, 1, 0, 0, 0),
                Creator = "es",
                HashCode = e.GetHashCode(),
                Increment = DefaultOperateEvent.NextIncrement,
            };

            if (creator != null)
                result.Creator = creator as string;
            if (createDate != null)
            {
                var time = DateTime.Now;
                if (DateTime.TryParse(createDate.ToString(), out time))
                    result.CreateDate = time;
            }

            return result;
        }

        #endregion utils
    }
}