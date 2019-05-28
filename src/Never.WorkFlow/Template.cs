using Never.Serialization;
using Never.WorkFlow.Attributes;
using Never.WorkFlow.WorkSteps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Never.WorkFlow
{
    /// <summary>
    /// 模板
    /// </summary>
    public class Template : IEquatable<Template>
    {
        #region ctor

        private Template()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class.
        /// </summary>
        /// <param name="templateName">模板名字</param>
        public Template(string templateName)
        {
            this.Name = templateName;
            this.workSteps = new List<TemplateWorkStepElement>(10)
            {
                new TemplateWorkStepElement() { Order = 0, Steps = new[] { "aa1b00b7cf4e" } }
            };
        }

        #endregion ctor

        #region field

        /// <summary>
        /// 所有的工作步骤集合
        /// </summary>
        private readonly List<TemplateWorkStepElement> workSteps = null;

        #endregion field

        #region prop

        /// <summary>
        /// 是否结束了
        /// </summary>
        public bool Ending { get; private set; } = false;

        /// <summary>
        /// 模板名字，要唯一
        /// </summary>
        public string Name { get; private set; } = null;

        /// <summary>
        /// 该模板的所有步骤
        /// </summary>
        public TemplateWorkStepElement[] Steps
        {
            get
            {
                return this.workSteps.ToArray();
            }
        }

        /// <summary>
        /// 该模板的所有步骤个数
        /// </summary>
        public int StepCount
        {
            get
            {
                return this.workSteps.Count;
            }
        }

        #endregion prop

        #region next

        /// <summary>
        /// 移动到下一工作步骤
        /// </summary>
        /// <typeparam name="TWorkStep">工作步骤</typeparam>
        /// <returns></returns>
        public Template Next<TWorkStep>() where TWorkStep : IWorkStep
        {
            return this.Next(new[] { typeof(TWorkStep) }, CoordinationMode.Any);
        }

        /// <summary>
        /// 移动到下一容器中，这个容器可以有多个工作单位，并且只有所有的工作步骤完成后才能移动到下一节点
        /// </summary>
        /// <param name="workSteps">The work steps.</param>
        /// <param name="mode">是否为同时完成才算完成当前步骤</param>
        /// <returns></returns>
        public Template Next(Type[] workSteps, CoordinationMode mode = CoordinationMode.Any)
        {
            if (workSteps == null)
            {
                return this;
            }

            if (workSteps.All(o => o == null))
            {
                return this;
            }

            if (this.Ending)
            {
                throw new ArgumentException("该模板已经结束了");
            }

            var step = TemplateWorkStep.New(workSteps, mode);
            step.Order = this.workSteps.Count;
            this.workSteps.Add(step);

            return this;
        }

        /// <summary>
        /// 移动到下一容器中，这个容器可以有多个工作单位，并且只有所有的工作步骤完成后才能移动到下一节点
        /// </summary>
        /// <typeparam name="TWorkStep1">工作步骤</typeparam>
        /// <typeparam name="TWorkStep2">工作步骤</typeparam>
        /// <param name="mode">是否为同时完成才算完成当前步骤</param>
        /// <returns></returns>
        public Template Next<TWorkStep1, TWorkStep2>(CoordinationMode mode = CoordinationMode.Any) where TWorkStep1 : IWorkStep where TWorkStep2 : IWorkStep
        {
            return this.Next(new[] { typeof(TWorkStep1), typeof(TWorkStep2) }, mode);
        }

        /// <summary>
        /// 移动到下一容器中，这个容器可以有多个工作单位，并且只有所有的工作步骤完成后才能移动到下一节点
        /// </summary>
        /// <typeparam name="TWorkStep1">工作步骤</typeparam>
        /// <typeparam name="TWorkStep2">工作步骤</typeparam>
        /// <typeparam name="TWorkStep3">工作步骤</typeparam>
        /// <param name="mode">是否为同时完成才算完成当前步骤</param>
        /// <returns></returns>
        public Template Next<TWorkStep1, TWorkStep2, TWorkStep3>(CoordinationMode mode = CoordinationMode.Any) where TWorkStep1 : IWorkStep where TWorkStep2 : IWorkStep where TWorkStep3 : IWorkStep
        {
            return this.Next(new[] { typeof(TWorkStep1), typeof(TWorkStep2), typeof(TWorkStep3) }, mode);
        }

        /// <summary>
        /// 移动到下一容器中，这个容器可以有多个工作单位，并且只有所有的工作步骤完成后才能移动到下一节点
        /// </summary>
        /// <typeparam name="TWorkStep1">工作步骤</typeparam>
        /// <typeparam name="TWorkStep2">工作步骤</typeparam>
        /// <typeparam name="TWorkStep3">工作步骤</typeparam>
        /// <typeparam name="TWorkStep4">工作步骤</typeparam>
        /// <param name="mode">是否为同时完成才算完成当前步骤</param>
        /// <returns></returns>
        public Template Next<TWorkStep1, TWorkStep2, TWorkStep3, TWorkStep4>(CoordinationMode mode = CoordinationMode.Any) where TWorkStep1 : IWorkStep where TWorkStep2 : IWorkStep where TWorkStep3 : IWorkStep where TWorkStep4 : IWorkStep
        {
            return this.Next(new[] { typeof(TWorkStep1), typeof(TWorkStep2), typeof(TWorkStep3), typeof(TWorkStep4) }, mode);
        }

        /// <summary>
        /// 移动到下一容器中，这个容器可以有多个工作单位，并且只有所有的工作步骤完成后才能移动到下一节点
        /// </summary>
        /// <typeparam name="TWorkStep1">工作步骤</typeparam>
        /// <typeparam name="TWorkStep2">工作步骤</typeparam>
        /// <typeparam name="TWorkStep3">工作步骤</typeparam>
        /// <typeparam name="TWorkStep4">工作步骤</typeparam>
        /// <typeparam name="TWorkStep5">工作步骤</typeparam>
        /// <param name="mode">是否为同时完成才算完成当前步骤</param>
        /// <returns></returns>
        public Template Next<TWorkStep1, TWorkStep2, TWorkStep3, TWorkStep4, TWorkStep5>(CoordinationMode mode = CoordinationMode.Any) where TWorkStep1 : IWorkStep where TWorkStep2 : IWorkStep where TWorkStep3 : IWorkStep where TWorkStep4 : IWorkStep
            where TWorkStep5 : IWorkStep
        {
            return this.Next(new[] { typeof(TWorkStep1), typeof(TWorkStep2), typeof(TWorkStep3), typeof(TWorkStep4), typeof(TWorkStep5) }, mode);
        }

        /// <summary>
        /// 移动到下一容器中，这个容器可以有多个工作单位，并且只有所有的工作步骤完成后才能移动到下一节点
        /// </summary>
        /// <typeparam name="TWorkStep1">工作步骤</typeparam>
        /// <typeparam name="TWorkStep2">工作步骤</typeparam>
        /// <typeparam name="TWorkStep3">工作步骤</typeparam>
        /// <typeparam name="TWorkStep4">工作步骤</typeparam>
        /// <typeparam name="TWorkStep5">工作步骤</typeparam>
        /// <typeparam name="TWorkStep6">工作步骤</typeparam>
        /// <param name="mode">是否为同时完成才算完成当前步骤</param>
        /// <returns></returns>
        public Template Next<TWorkStep1, TWorkStep2, TWorkStep3, TWorkStep4, TWorkStep5, TWorkStep6>(CoordinationMode mode = CoordinationMode.Any) where TWorkStep1 : IWorkStep where TWorkStep2 : IWorkStep where TWorkStep3 : IWorkStep where TWorkStep4 : IWorkStep
            where TWorkStep5 : IWorkStep where TWorkStep6 : IWorkStep
        {
            return this.Next(new[] { typeof(TWorkStep1), typeof(TWorkStep2), typeof(TWorkStep3), typeof(TWorkStep4), typeof(TWorkStep5), typeof(TWorkStep6) }, mode);
        }

        /// <summary>
        /// 移动到下一容器中，这个容器可以有多个工作单位，并且只有所有的工作步骤完成后才能移动到下一节点
        /// </summary>
        /// <typeparam name="TWorkStep1">工作步骤</typeparam>
        /// <typeparam name="TWorkStep2">工作步骤</typeparam>
        /// <typeparam name="TWorkStep3">工作步骤</typeparam>
        /// <typeparam name="TWorkStep4">工作步骤</typeparam>
        /// <typeparam name="TWorkStep5">工作步骤</typeparam>
        /// <typeparam name="TWorkStep6">工作步骤</typeparam>
        /// <typeparam name="TWorkStep7">工作步骤</typeparam>
        /// <param name="mode">是否为同时完成才算完成当前步骤</param>
        /// <returns></returns>
        public Template Next<TWorkStep1, TWorkStep2, TWorkStep3, TWorkStep4, TWorkStep5, TWorkStep6, TWorkStep7>(CoordinationMode mode = CoordinationMode.Any) where TWorkStep1 : IWorkStep where TWorkStep2 : IWorkStep where TWorkStep3 : IWorkStep where TWorkStep4 : IWorkStep
            where TWorkStep5 : IWorkStep where TWorkStep6 : IWorkStep where TWorkStep7 : IWorkStep
        {
            return this.Next(new[] { typeof(TWorkStep1), typeof(TWorkStep2), typeof(TWorkStep3), typeof(TWorkStep4), typeof(TWorkStep5), typeof(TWorkStep6), typeof(TWorkStep7) }, mode);
        }

        /// <summary>
        /// 移动到下一容器中，这个容器可以有多个工作单位，并且只有所有的工作步骤完成后才能移动到下一节点
        /// </summary>
        /// <typeparam name="TWorkStep1">工作步骤</typeparam>
        /// <typeparam name="TWorkStep2">工作步骤</typeparam>
        /// <typeparam name="TWorkStep3">工作步骤</typeparam>
        /// <typeparam name="TWorkStep4">工作步骤</typeparam>
        /// <typeparam name="TWorkStep5">工作步骤</typeparam>
        /// <typeparam name="TWorkStep6">工作步骤</typeparam>
        /// <typeparam name="TWorkStep7">工作步骤</typeparam>
        /// <typeparam name="TWorkStep8">工作步骤</typeparam>
        /// <param name="mode">是否为同时完成才算完成当前步骤</param>
        /// <returns></returns>
        public Template Next<TWorkStep1, TWorkStep2, TWorkStep3, TWorkStep4, TWorkStep5, TWorkStep6, TWorkStep7, TWorkStep8>(CoordinationMode mode = CoordinationMode.Any) where TWorkStep1 : IWorkStep where TWorkStep2 : IWorkStep where TWorkStep3 : IWorkStep where TWorkStep4 : IWorkStep
            where TWorkStep5 : IWorkStep where TWorkStep6 : IWorkStep where TWorkStep7 : IWorkStep where TWorkStep8 : IWorkStep
        {
            return this.Next(new[] { typeof(TWorkStep1), typeof(TWorkStep2), typeof(TWorkStep3), typeof(TWorkStep4), typeof(TWorkStep5), typeof(TWorkStep6), typeof(TWorkStep7), typeof(TWorkStep8) }, mode);
        }

        /// <summary>
        /// 移动到完成步骤
        /// </summary>
        public Template End()
        {
            if (this.Ending)
            {
                return this;
            }

            this.workSteps.Add(new TemplateWorkStepElement() { Order = this.workSteps.Count, Steps = new[] { "aa1b00b7d77f" } });
            this.Ending = true;
            return this;
        }

        #endregion next

        #region tostring

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(new EasyJsonSerializer());
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        public string ToString(IJsonSerializer jsonSerializer)
        {
            return ToJson(jsonSerializer, this);
        }

        #endregion tostring

        #region equal

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Template other)
        {
            return other != null && this.Name.IsEquals(other.Name) && this.ToString().IsEquals(other.ToString());
        }

        #endregion equal

        #region element

        private class TemplateJson
        {
            public string Name { get; set; }
            public TemplateWorkStepElement[] Collection { get; set; }
        }

        #endregion element

        #region json

        /// <summary>
        /// 将模板转换为json内容
        /// </summary>
        /// <param name="jsonSerializer"></param>
        /// <param name="template">模板</param>
        /// <returns></returns>
        public static string ToJson(IJsonSerializer jsonSerializer, Template template)
        {
            var temp = new TemplateJson() { Name = template.Name };
            var list = new List<TemplateWorkStepElement>(template.StepCount);
            foreach (var i in template.Steps)
            {
                if (i.Order == 0)
                {
                    continue;
                }

                if (i.Order == template.StepCount - 1)
                {
                    continue;
                }

                list.Add(i);
            }

            temp.Collection = list.ToArray();
            return jsonSerializer.Serialize(temp);
        }

        /// <summary>
        /// 从json内容中转换为模板对象
        /// </summary>
        /// <param name="jsonSerializer"></param>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static Template FromJson(IJsonSerializer jsonSerializer, string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var temp = jsonSerializer.Deserialize<TemplateJson>(json);
            if (temp == null)
            {
                return null;
            }

            var template = new Template(temp.Name);
            foreach (var ele in temp.Collection)
            {
                template.Next(ele.Steps.Select(ta => TemplateWorkStep.Find(ta)).ToArray(), ele.Mode);
            }

            return template.End();
        }

        #endregion json
    }
}