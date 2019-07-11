using Microsoft.AspNetCore.Mvc;
using Never.Attributes;
using Never.Commands;
using Never.Configuration;
using Never.TestWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Never.TestWebApi.Controllers
{
    [Route("api/")]
    [ApiAreaRemark("v1.0")]
    public class ValuesController : Never.Web.WebApi.Controllers.BasicController, IDisposable
    {
        public ValuesController(ICommandBus commandBus)
        {
        }

        // GET api/values
        [ApiActionRemark("a914012f291b", "HttpGet")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [Never.Attributes.ApiActionRemark("a91500c8ca64", "HttpGet")]
        public string Get(int id)
        {
           // var value = AppConfig.StringInAppSettings("ApiLoad");
            return "value";
        }

        [ApiActionRemark("Regiseter", "HttpPost"), HttpPost]
        public string Regiseter(CreateUserReqs reqs)
        {
            if (!this.TryValidateModel(reqs))
                return this.ModelErrorMessage;

            return "ok";
        }

        [ApiActionRemark("ChangePwdReqs", "HttpPost"), HttpPost]
        public string ChangePwd(ChangePwdReqs reqs)
        {
            if (!this.TryValidateModel(reqs))
                return this.ModelErrorMessage;

            return "ok";
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}