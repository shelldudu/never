using Never;
using Never.IoC;
using Never.SqlClient;
using Never.WorkFlow;
using Never.WorkFlow.Coordinations;
using Never.WorkFlow.Repository;
using Never.WorkFlow.Test.Messages;
using Never.WorkFlow.Test.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.WorkFlow.Test
{
    internal class MyMain
    {
        public static void Main222(string[] args)
        {
            var workflowEngine = TypeCreate();
            var query = workflowEngine.CreateQueue();
            var tasks = query.GetAll("小狗与大狗20170320的请假", new[] { TaskschedStatus.Working });
            var task = new TaskschedNode();
            if (tasks != null && tasks.Any())
            {
                task = tasks.FirstOrDefault();
            }
            else
            {
                task = query.Push("请病假申请", "小狗与大狗20170320的请假", "qingjia", new 请假申请消息()
                {
                    CreateDate = DateTime.Now,
                    BeginTime = DateTime.Now,
                    EndTime = DateTime.Now.AddDays(2),
                    Reason = "生病了",
                    UserName = "小狗与大狗"
                });
            }

            workflowEngine.CreateEngine().Execute(task);
        }

        public static IWorkFlowEngine TypeCreate()
        {
            var workflowEngine = new WorkFlowEngine(new IoCMySqlExecutingRepository(), new IoCMySqlExecutingRepository(), new Never.Serialization.EasyJsonSerializer());
            workflowEngine.TemplateEngine.Register(new Template("请病假申请").Next<请假申请>().Next<组长审批, 主管审批>(CoordinationMode.Meanwhile).Next<人事审批>().End());
            workflowEngine.TemplateEngine.Register(new Template("请节日假申请").Next<请假申请>().Next<组长审批>().Next<人事审批>().End());
            workflowEngine.TemplateEngine.Register(new Template("请婚姻假申请").Next<请假申请>().Next<组长审批, 主管审批>().Next<人事审批>().End());
            workflowEngine.TemplateEngine.TestCompliant += (o, e) =>
            {
                e.Register<请假申请>(new 请假申请消息(), (x, y) => { return y is 请假申请消息; });
                e.Register<主管审批>(new 主管审批意见结果(), (x, y) => { return y is 请假申请消息; });
                e.Register<组长审批>(new 组长审批意见结果(), (x, y) => { return y is 请假申请消息; });
                e.Register<人事审批>(new 人事审批请假意见结果(), (x, y) => { return y is 组长审批意见结果 || y is 主管审批意见结果; });
            };

            workflowEngine.Startup();

            return workflowEngine;
        }

        public static IWorkFlowEngine IoCCreate()
        {
            var app = new Never.ApplicationStartup(IoC.Providers.AppDomainAssemblyProvider.Default)
                  .RegisterAssemblyFilter("Never".CreateAssemblyFilter())
                  .UsePublishSubscribeBus()
                  .UseEasyIoC((x, y, z) =>
                  {
                      x.RegisterCallBack("eee", ComponentLifeStyle.Singleton, () => new IoCMySqlTemplateRepository());
                  })
                  .UseWorlFlow(new IoCMySqlTemplateRepository(), new IoCMySqlExecutingRepository(), new Never.Serialization.EasyJsonSerializer(), (w) =>
                   {
                       w.TemplateEngine.Register(new Template("请病假申请").Next<请假申请>().Next<组长审批, 主管审批>(CoordinationMode.Meanwhile).Next<人事审批>().End());
                       w.TemplateEngine.Register(new Template("请节日假申请").Next<请假申请>().Next<组长审批>().Next<人事审批>().End());
                       w.TemplateEngine.Register(new Template("请婚姻假申请").Next<请假申请>().Next<组长审批, 主管审批>().Next<人事审批>().End());
                       w.TemplateEngine.TestCompliant += (o, e) =>
                       {
                           e.Register<请假申请>(new 请假申请消息(), (x, y) => { return y is 请假申请消息; });
                           e.Register<主管审批>(new 主管审批意见结果(), (x, y) => { return y is 请假申请消息; });
                           e.Register<组长审批>(new 组长审批意见结果(), (x, y) => { return y is 请假申请消息; });
                           e.Register<人事审批>(new 人事审批请假意见结果(), (x, y) => { return y is 组长审批意见结果 || y is 主管审批意见结果; });
                       };
                   })
                   .Startup(x =>
                   {

                   });

            var eee = app.ServiceLocator.Resolve<IoCMySqlTemplateRepository>("eee");
            eee = app.ServiceLocator.Resolve<IoCMySqlTemplateRepository>();
            return app.ServiceLocator.Resolve<IWorkFlowEngine>("abc");
        }

        private class IoCMySqlTemplateRepository : SingleTableMySqlExecutingRepository
        {
            protected override string TablePrefixName
            {
                get
                {
                    return "single_";
                }
            }

            protected override ISqlExecuter Open()
            {
                var conn = @"server=127.0.0.1;port=3306;sslmode=none;uid=sa;pwd=123456;database=b2c_admin";
                return SqlExecuterFactory.MySql(conn);
            }
        }

        private class IoCMySqlExecutingRepository : SingleTableSqlServerExecutingRepository
        {
            protected override string TablePrefixName
            {
                get
                {
                    return "single_";
                }
            }

            protected override ISqlExecuter Open()
            {
                var conn = @"server=127.0.0.1;uid=sa;pwd=123456;database=b2c_admin";
                return SqlExecuterFactory.SqlServer(conn);
            }
        }
    }
}