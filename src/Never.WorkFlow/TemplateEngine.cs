using Never.WorkFlow.Attributes;
using Never.WorkFlow.Exceptions;
using Never.WorkFlow.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Never.WorkFlow
{
    /// <summary>
    /// 模板引擎
    /// </summary>
    public class TemplateEngine : ITemplateEngine
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="workflowengine"></param>
        /// <param name="templateRepository"></param>
        public TemplateEngine(IWorkFlowEngine workflowengine, ITemplateRepository templateRepository)
        {
            this.workflowengine = workflowengine;
            this.templateRepository = templateRepository;
            this.cache = new Dictionary<string, Template>();
            this.addlist = new List<Template>(10);
            this.changeList = new List<Template>(10);
        }

        #endregion ctor

        #region field

        /// <summary>
        /// 引擎
        /// </summary>
        private readonly IWorkFlowEngine workflowengine = null;

        /// <summary>
        /// 模板仓库
        /// </summary>
        private readonly ITemplateRepository templateRepository = null;

        /// <summary>
        /// 用于对模板进行缓存
        /// </summary>
        private readonly Dictionary<string, Template> cache = null;

        /// <summary>
        /// 已经准备好了
        /// </summary>
        private bool ready = false;

        /// <summary>
        /// 新加的模板
        /// </summary>
        private List<Template> addlist = null;

        /// <summary>
        /// 更新模板
        /// </summary>
        private List<Template> changeList = null;

        /// <summary>
        /// 是否已经加载所有模板
        /// </summary>
        private bool loaded = false;

        #endregion field

        #region select

        /// <summary>
        /// 登记模板
        /// </summary>
        /// <param name="template">模板</param>
        public void Register(Template template)
        {
            if (!this.loaded)
            {
                lock (this)
                {
                    if (!this.loaded)
                    {
                        var templates = this.templateRepository.GetAll(this.workflowengine.JsonSerializer);
                        foreach (var te in templates)
                        {
                            if (te.Steps != null)
                            {
                                foreach (var ct in te.Steps)
                                {
                                    if (ct.Steps != null)
                                    {
                                        foreach (var t in ct.Steps)
                                        {
                                            if (t != null)
                                            {
                                                this.cache[te.Name] = te;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        this.loaded = true;
                    }
                }
            }

            template.End();

            var exists = this.cache.ContainsKey(template.Name);
            if (exists)
            {
                //if (!cache[template.Name].Equals(template))
                //{
                //    this.changeList.Add(template);
                //    cache[template.Name] = template;
                //}

                this.changeList.Add(template);
                this.cache[template.Name] = template;
            }
            else
            {
                this.addlist.Add(template);
                this.cache[template.Name] = template;
            }
        }

        /// <summary>
        /// 查询登记模板
        /// </summary>
        /// <param name="templateName">模板名字</param>
        /// <returns></returns>
        public Template Select(string templateName)
        {
            var template = this.templateRepository.Get(this.workflowengine.JsonSerializer, templateName);
            if (template != null)
            {
                this.cache[template.Name] = template;
            }

            return template;
        }

        #endregion select

        #region test comliant

        /// <summary>
        /// 兼容性测试事件
        /// </summary>
        public event EventHandler<CompliantEventArgs> TestCompliant;

        /// <summary>
        /// 准备好了
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public void Startup()
        {
            if (this.ready)
            {
                return;
            }

            this.ready = true;
            var eventArgs = new CompliantEventArgs();
            this.TestCompliant?.Invoke(this, eventArgs);

            /*有注册兼容性检查的才检查*/
            if (eventArgs.collections.Any())
            {
                foreach (var tmpl in this.addlist)
                {
                    var steps = tmpl.Steps.ToArray();
                    if (steps.Length <= 3)
                    {
                        continue;
                    }

                    /*从第三个步骤开始，因为第二个步骤的消息参数是从begin那里得来的*/
                    for (var i = 2; i < steps.Length - 1; i++)
                    {
                        foreach (var step in steps[i].Steps)
                        {
                            var type = TemplateWorkStep.Find(step);
                            if (!eventArgs.collections.ContainsKey(type))
                            {
                                throw new TemplateNotCompliantException("步骤{0}所需要上一步骤的消息结果没有注册，无法确定是否与当前步骤兼容", type.FullName);
                            }

                            var elements = steps[i - 1];
                            foreach (var ele in elements.Steps)
                            {
                                if (!eventArgs.collections.ContainsKey(TemplateWorkStep.Find(ele)))
                                {
                                    throw new TemplateNotCompliantException("步骤{0}所需要上一步骤的消息结果没有注册，无法确定是否与当前步骤兼容", type.FullName);
                                }

                                var preAction = eventArgs.collections[TemplateWorkStep.Find(ele)];
                                var curAction = eventArgs.collections[type];
                                if (curAction.Value.Invoke(type, preAction.Key))
                                {
                                    break;
                                }

                                throw new TemplateNotCompliantException("步骤{0}所需要上一步骤的消息结果与当前步骤不兼容", type.FullName);
                            }
                        }
                    }
                }
            }

            this.templateRepository.SaveAndChange(this.workflowengine.JsonSerializer, this.addlist.ToArray(), this.changeList.ToArray());

            return;
        }

        #endregion test comliant
    }
}