using Microsoft.AspNetCore.Mvc;
using Never.Attributes;
using Never.Commands;
using Never.Configuration;
using Never.TestWebApi.Models;
using Never.Web.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Never.TestWebApi.Controllers
{
    [Route("api/")]
    public class ValuesController : Never.Web.WebApi.Controllers.BasicController, IDisposable
    {
        public ValuesController(ICommandBus commandBus)
        {
        }

        // GET api/values
        [ApiActionRemark("a914012f291b", "HttpGet"), HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [ApiActionRemark("contacts", "HttpPost"), HttpPost]
        public JsonResult contacts()
        {
            return this.Json(new
            {
                id = "cont_00000000000001",
                name = "Gaurav Kumar",
                entity = "contact",
                contact = "9123456789",
                email = "gaurav.kumar@example.com",
                type = "employee",
                reference_id = "Acme Contact ID 12345",
                batch_id = (String)null,
                active = true,
                notes = new Object[0],
                created_at = "1545320320",
            });
        }


        [ApiActionRemark("fund_accounts", "HttpPost"), HttpPost]
        public JsonResult fund_accounts()
        {
            return this.Json(new
            {
                id = "fa_00000000000001",
                entity = "fund_account",
                name = "Gaurav Kumar",
                contact_id = "cont_00000000000001",
                account_type = "bank_account",
                bank_account = new
                {
                    ifsc = "HDFC0000053",
                    bank_name = "HDFC Bank",
                    name = "Gaurav Kumar",
                    account_number = "765432123456789",
                },
                batch_id = (String)null,
                active = true,
                notes = new Object[0],
                created_at = "1545320320",
            });
        }


        [ApiActionRemark("fund_accounts/validations", "HttpPost"), HttpPost]
        public JsonResult fund_accounts_validations(string id)
        {
            return this.Json(new
            {
                id = "fav_E3s8LkXemkJOtm",
                entity = "fund_account.validation",
                name = "Gaurav Kumar",
                contact_id = "cont_00000000000001",
                account_type = "bank_account",
                fund_account = new
                {
                    ifsc = "fa_E3rbMCO9kMJwOV",
                    entity = "fund_account",
                    contact_id = "cont_E3pCZwID2gAKsd",
                    account_type = "bank_account",
                    bank_account = new
                    {
                        ifsc = "HDFC0000053",
                        bank_name = "HDFC Bank",
                        name = "765432123456789",
                        notes = new Object[0],
                        account_number = "765432123456789"
                    },
                    details = new
                    {
                        ifsc = "HDFC0000053",
                        bank_name = "HDFC Bank",
                        name = "765432123456789",
                        notes = new Object[0],
                        account_number = "765432123456789"
                    }
                },
                batch_id = (String)null,
                active = true,
                notes = new Object[0],
                created_at = "1545320320",
                status = id.IsNullOrEmpty() ? "created" : "completed",
                amount = "100",
                currency = "INR",
                results = new
                {
                    account_status = id.IsNullOrEmpty() ? (String)null : "active",
                    registered_name = id.IsNullOrEmpty() ? (String)null : "1545320320",
                },
            });
        }


        [ApiActionRemark("payouts", "HttpPost"), HttpPost]
        public JsonResult payouts()
        {
            return this.Json(new
            {
                id = "pout_00000000000001",
                entity = "payout",
                fund_account_id = "fa_00000000000001",
                amount = "1000000",
                currency = "INR",
                fees = "0",
                tax = "0",
                batch_id = (String)null,
                utr = (String)null,
                mode = "NEFT",
                purpose = "refund",
                reference_id = "Acme Transaction ID 12345",
                narration = "Acme Corp Fund Transfer",
                failure_reason = (String)null,
                active = true,
                notes = new Object[0],
                created_at = "1545383037",
                status = "queued",
            });
        }


        [ApiActionRemark("orders", "HttpPost"), HttpPost]
        public JsonResult orders()
        {
            return this.Json(new
            {
                id = "order_DaZlswtdcn9UNV",
                entity = "order",
                amount = "50000",
                currency = "INR",
                amount_paid = "0",
                amount_due = "50000",
                batch_id = (String)null,
                receipt = "Receipt #20",
                attempts = "0",
                active = true,
                notes = new Object[0],
                created_at = "1572502745",
                status = "created",
            });
        }

        [ApiActionRemark("eee", "HttpGet"), HttpGet]
        public IHttpActionResult MyAgrememt()
        {
            //using (var stream = System.IO.File.OpenRead("c:\\eee.pdf"))
            {
                return new Never.Web.WebApi.Results.MyPDFResult(System.IO.File.OpenRead("c:\\eee.pdf"), this);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }


    }
}