using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Never.Attributes;

namespace Never.TestWebApi.Controllers.v1._1
{
    [Route("api/")]
    [ApiAreaRemark("v1.1")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        [ApiActionRemark("a914012f291b", "HttpGet")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value5", "value6" };
        }
    }
}