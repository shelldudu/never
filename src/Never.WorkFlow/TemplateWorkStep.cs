using Never.WorkFlow.Attributes;
using Never.WorkFlow.WorkSteps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Never.WorkFlow
{
    /// <summary>
    /// 步骤执行的元素
    /// </summary>
    internal class TemplateWorkStep
    {
        /// <summary>
        /// 工作步骤
        /// </summary>
        public static readonly List<KeyValuePair<WorkStepAttribute, Type>> Steps = null;

        /// <summary>
        /// 
        /// </summary>
        static TemplateWorkStep()
        {
            Steps = new List<KeyValuePair<WorkStepAttribute, Type>>()
            {
                new KeyValuePair<WorkStepAttribute, Type>(new WorkStepAttribute("1"),typeof(BeginWorkStep)),
                new KeyValuePair<WorkStepAttribute, Type>(new WorkStepAttribute("2147483647"),typeof(EndWorkStep)),
            };

        }

        public static TemplateWorkStepElement New(Type[] workSteps, CoordinationMode mode)
        {
            var attributes = new List<WorkStepAttribute>();
            foreach (var step in workSteps)
            {
                AddStep(step, out var attribute);
                attributes.Add(attribute);
            }

            return new TemplateWorkStepElement()
            {
                Mode = mode,
                Steps = attributes.Select(ta => ta.UniqueId).ToArray(),
            };
        }

        public static void AddStep(Type workstepType, out WorkStepAttribute attribute)
        {
            attribute = workstepType.GetAttribute<WorkStepAttribute>();
            if (attribute == null)
            {
                throw new ArgumentNullException(string.Format("type {0} must had WorkStepAttribute attribute", workstepType.FullName));
            }

            var uniqueId = attribute.UniqueId;
            var filters = Steps.Where(ta => ta.Key.UniqueId == uniqueId);
            if (filters.IsNullOrEmpty())
            {
                Steps.Add(new KeyValuePair<WorkStepAttribute, Type>(attribute, workstepType));
                return;
            }

            if (filters.Any(ta => ta.Value != workstepType))
            {
                throw new ArgumentException("duplicate workstep attribute uniqueid {0}", attribute.UniqueId);
            }
        }

        public static void AddStep(Type workstepType)
        {
            AddStep(workstepType, out var attribute);
        }

        public static Type Find(WorkStepAttribute attribute)
        {
            return Find(attribute.UniqueId);
        }

        public static Type Find(string uniqueId)
        {
            var filters = Steps.Where(ta => ta.Key.UniqueId == uniqueId);
            if (filters.Any())
            {
                return filters.FirstOrDefault().Value;
            }

            return null;
        }
    }
}