using Never.Serialization;
using Never.Serialization.Json;
using Never.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Never.TestConsole.Serialization
{
    public class SerializerTest
    {
        [Xunit.Fact]
        public void TestWriteObject()
        {
            var a = new TeseDemo1
            {
                C = '\u0022',
                Empty = " ",
                Id = int.MinValue + 1,
                Byte = 233,
                Short = -6530,
                Long = -9676546546L,
                UShort = 6530,
                ULong = 9676546546L,
                UId = 6546545,
                Name = "lxl",
                ABC = ABC.B,
                DDD = "}",
                DDD2 = "ddd",
                M = "dg",
                DDD3 = "ddd",
                DDD4 = "ddd",
                DDD5 = "ddd2",
                DDD6 = "ddd2",
                Guid = Guid.NewGuid(),
                Amount = -79228162514264337593543950335m,
                Time = DateTime.Now,
                Object = new TwoLevel
                {
                    Name = 234
                },
                Array = new TwoLevel[][]
                {
                    new TwoLevel[]
                     {
                         new TwoLevel
                         {
                             Name = 234
                         },
                         new TwoLevel
                         {
                             Name = 234
                         }
                     },
                    null,
                    new TwoLevel[]
                     {
                         new TwoLevel
                         {
                             Name = 234
                         },
                         new TwoLevel
                         {
                             Name = 234
                         }
                     },
                },
                Enumables = new[] { new object[] { new { Id = 56456 }, new { EF = "dgd" } } },
                EnumArray = new ABC[]
                {
                     ABC.A,
                     ABC.B,
                     ABC.C
                },
            };

            var text = EasyJsonSerializer.Serialize(a);
            var aa = EasyJsonSerializer.Deserialize<TeseDemo1>(text);
            var reader = ThunderReader.Load(text);
            var @obj = EasyJsonSerializer.Deserialize<TwoLevel[][]>(reader.Parse(reader.Read("Array")));
            return;
        }

        [Xunit.Fact]
        public void TestDemo999()
        {
            var a = new TeseDemo999()
            {
                Array = new TwoLevel[][]
                 {
                    new TwoLevel[]
                     {
                         new TwoLevel
                         {
                             Name = 234
                         },
                         new TwoLevel
                         {
                             Name = 234
                         }
                     },
                    null,
                    new TwoLevel[]
                     {
                         new TwoLevel
                         {
                             Name = 234
                         },
                         new TwoLevel
                         {
                             Name = 234
                         }
                     },
                 },
            };

            var text = EasyJsonSerializer.Serialize(a);
            Console.WriteLine(text);
            a = EasyJsonSerializer.Deserialize<TeseDemo999>(text);
            text = EasyJsonSerializer.Serialize(a);
            Console.WriteLine(text);
        }

        [Xunit.Fact]
        public void TestIntPrimitiveType()
        {
            var reader = ThunderReader.Load("{\n\n}");
            var abc = EasyJsonSerializer.Deserialize<AccessTokenModel>(reader);
            var test = EasyJsonSerializer.Serialize(new AccessTokenModel());
            var target = 0;
            Console.WriteLine("target:" + target.ToString());
            var text = EasyJsonSerializer.Serialize(target);
            target = EasyJsonSerializer.Deserialize<int>(text);
            var target2 = EasyJsonSerializer.Deserialize<int[]>("  ");
            Console.WriteLine("target:" + target.ToString());
            Console.WriteLine("-----------------------");

            target = 11;
            Console.WriteLine("target:" + target.ToString());
            text = EasyJsonSerializer.Serialize(target);
            target = EasyJsonSerializer.Deserialize<int>(text);
            Console.WriteLine("target:" + target.ToString());
            Console.WriteLine("-----------------------");

            target = 192;
            Console.WriteLine("target:" + target.ToString());
            text = EasyJsonSerializer.Serialize(target);
            target = EasyJsonSerializer.Deserialize<int>(text);
            Console.WriteLine("target:" + target.ToString());
            Console.WriteLine("-----------------------");
        }

        [Xunit.Fact]
        public void TestGuidPrimitiveType()
        {
            var target = Guid.NewGuid();
            Console.WriteLine("target:" + target.ToString());
            var text = EasyJsonSerializer.Serialize(target);
            target = EasyJsonSerializer.Deserialize<Guid>(text);
            Console.WriteLine("target:" + target.ToString());
            Console.WriteLine("-----------------------");

            target = Guid.Empty;
            Console.WriteLine("target:" + target.ToString());
            text = EasyJsonSerializer.Serialize(target);
            target = EasyJsonSerializer.Deserialize<Guid>(text);
            Console.WriteLine("target:" + target.ToString());
            Console.WriteLine("-----------------------");
        }

        [Xunit.Fact]
        public void TestEnumType()
        {
            var tet = EasyJsonSerializer.Deserialize<EnumClass>(EasyJsonSerializer.Serialize(new EnumClass() { }));
            var target = ABC.A | ABC.B;

            //GlobalJsonCompileSetting.Config(new JsonCompileSetting() { UseNumberInEnum = true });
            Console.WriteLine("target:" + target.ToString());
            var text = EasyJsonSerializer.Serialize(target);
            Console.WriteLine("target:" + text);
            target = EasyJsonSerializer.Deserialize<ABC>(text);
            Console.WriteLine("target:" + target.ToString());
            Console.WriteLine("-----------------------");

            target = ABC.C;
            Console.WriteLine("target:" + target.ToString());
            text = EasyJsonSerializer.Serialize(target);
            Console.WriteLine("target:" + text);
            target = EasyJsonSerializer.Deserialize<ABC>("null");
            Console.WriteLine("target:" + target.ToString());
            Console.WriteLine("-----------------------");
        }

        [Xunit.Fact]
        public void TestArrayIntPrimitiveType()
        {
            var target = new[] { 1M, 2M, 3M };
            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            var text = EasyJsonSerializer.Serialize(target);
            target = EasyJsonSerializer.Deserialize<decimal[]>(text);
            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");

            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            text = EasyJsonSerializer.Serialize(target);
            var target2 = EasyJsonSerializer.Deserialize<List<decimal>>(text);
            Console.WriteLine("target:");
            foreach (var i in target2)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");

            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }

            text = EasyJsonSerializer.Serialize(target);
            var target3 = EasyJsonSerializer.Deserialize<ICollection<decimal>>(text);
            Console.WriteLine("target:");
            foreach (var i in target3)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");

            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }

            text = EasyJsonSerializer.Serialize(target);
            var target4 = EasyJsonSerializer.Deserialize<IList<decimal>>(text);
            Console.WriteLine("target:");
            foreach (var i in target4)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");

            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }

            text = EasyJsonSerializer.Serialize(target);
            var target5 = EasyJsonSerializer.Deserialize<IEnumerable<decimal>>(text);
            Console.WriteLine("target:");
            foreach (var i in target5)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");
        }

        [Xunit.Fact]
        public void TestArrayEnumPrimitiveType()
        {
            //GlobalJsonCompileSetting.Config(new JsonCompileSetting() { UseNumberInEnum = true });
            var target = new[] { ABC.A, ABC.A, ABC.C, ABC.A | ABC.C };
            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            var text = EasyJsonSerializer.Serialize(target);
            target = EasyJsonSerializer.Deserialize<ABC[]>(text);
            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");

            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            text = EasyJsonSerializer.Serialize(target);
            var target2 = EasyJsonSerializer.Deserialize<ICollection<ABC>>(text);
            Console.WriteLine("target:");
            foreach (var i in target2)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");
        }

        [Xunit.Fact]
        public void TestArrayObjectType()
        {
            var target = new TwoLevel[]
                     {
                         new TwoLevel
                         {
                             Name = 234,
                             Three = new ThreeLevel() { }
                         },
                         null,
                         new TwoLevel
                         {
                             Name = 5465
                         }
                     };

            Console.WriteLine("target:");
            foreach (var i in target)
            {
                if (i == null)
                {
                    continue;
                }

                Console.WriteLine(i.ToString() + ",");
            }
            var text = EasyJsonSerializer.Serialize(target);
            target = EasyJsonSerializer.Deserialize<TwoLevel[]>(text);
            Console.WriteLine("target:");
            foreach (var i in target)
            {
                if (i == null)
                {
                    continue;
                }

                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");

            Console.WriteLine("target:");
            foreach (var i in target)
            {
                if (i == null)
                {
                    continue;
                }

                Console.WriteLine(i.ToString() + ",");
            }
            text = EasyJsonSerializer.Serialize(target);
            var target2 = EasyJsonSerializer.Deserialize<List<TwoLevel>>(text);
            Console.WriteLine("target:");
            foreach (var i in target2)
            {
                if (i == null)
                {
                    continue;
                }

                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");
        }

        [Xunit.Fact]
        public void TestArrayArrayIntType()
        {
            var target = new TwoLevel[][]
                {
                    new TwoLevel[]
                     {
                         new TwoLevel
                         {
                             Name = 234,
                             U = "E",
                         },
                         new TwoLevel
                         {
                             Name = 54687,
                             U = "EET"
                         }
                     }
                };

            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            var text = EasyJsonSerializer.Serialize(target);
            target = EasyJsonSerializer.Deserialize<TwoLevel[][]>(text);
            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");

            Console.WriteLine("target:");
            foreach (var i in target)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            text = EasyJsonSerializer.Serialize(target);
            var target2 = EasyJsonSerializer.Deserialize<ICollection<TwoLevel[]>>(text);
            Console.WriteLine("target:");
            foreach (var i in target2)
            {
                Console.WriteLine(i.ToString() + ",");
            }
            Console.WriteLine("-----------------------");
        }

        [Xunit.Fact]
        public void TestDictionaryType()
        {
            var target = new Hashtable(2);
            target["A"] = "A";
            target["B"] = new TwoLevel() { Name = 1122, U = "First" };
            //GlobalJsonCompileSetting.Config(new JsonCompileSetting() { UseNumberInEnum = true });
            Console.WriteLine("target:" + target.ToString());
            var text = EasyJsonSerializer.Serialize(target);
            Console.WriteLine("target:" + text);
            target = EasyJsonSerializer.Deserialize<Hashtable>(text);
            Console.WriteLine("target:" + target.ToString());
            Console.WriteLine("-----------------------");

            target = new Hashtable(2);
            target["A"] = "A";
            target["B"] = new TwoLevel() { Name = 1122, U = "First" };
            Console.WriteLine("target:" + target.ToString());
            text = EasyJsonSerializer.Serialize(target);
            Console.WriteLine("target:" + text);
            var target2 = EasyJsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, string>>(text);
            Console.WriteLine("target:" + target.ToString());
            Console.WriteLine("-----------------------");
        }

        [Xunit.Fact]
        public void TestGenericDictionaryType()
        {
            var dict = new Dictionary<int, TwoLevel>(2);
            dict[1] = new TwoLevel() { Name = 1122, U = "First" };
            dict[2] = new TwoLevel() { Name = 3344, U = "Secode" };
            var text = EasyJsonSerializer.Serialize(dict);
            Console.WriteLine(text);
            var target2 = EasyJsonSerializer.Deserialize<System.Collections.Generic.Dictionary<int, TwoLevel>>(text);
        }

        [Xunit.Fact]
        public void TestDemo()
        {
            var demo = new TeseDemo()
            {
                Name = "Lxl",
                UId = 365,
                ULong = 54654
            };

            var text = EasyJsonSerializer.Serialize(demo);
            var list = ThunderReader.Load(text);
            var demo2 = EasyJsonSerializer.Deserialize<TeseDemo>(text);
        }

        [Xunit.Fact]
        public void TestDemo2()
        {
            var demo = new TestDemo2()
            {
                Name = "Lxl",
                UId = 365,
                C = '\ufff2',
                ULong = 54654,
                Demo3 = new TestDemo3()
                {
                    Name = "Lxl33",
                    UId = 3665,
                    ULong = 154654,
                }
            };

            var text = EasyJsonSerializer.Serialize(demo);
            var list = ThunderReader.Load(text);
            var demo2 = EasyJsonSerializer.Deserialize<TestDemo2>(text);
        }

        [Xunit.Fact]
        public void TestDemo3()
        {
        }

        [Xunit.Fact]
        public void TestDemo4()
        {
            var a = new TeseDemo4
            {
                Object = new TwoLevel
                {
                    Name = 234,
                    U = "UU",
                    Three = new ThreeLevel() { ABC = "LXL" }
                },
            };

            var text = EasyJsonSerializer.Serialize(Anonymous.NewApiResult(ApiStatus.Success, (TwoLevel)null));
            Console.WriteLine(text);
            var ta = EasyJsonSerializer.Deserialize<ApiResult<TwoLevel>>(text);
            return;
        }

        [Xunit.Fact]
        public void TestException()
        {
            var ex = new Exception("etet", null);
            //var text = Newtonsoft.Json.JsonConvert.SerializeObject(ex);// EasyJsonSerializer.Default.Serialize<Exception>(ex);
            //Never.Serialization.Json.GlobalJsonCompileSetting.Config(new JsonCompileSetting() { WriteNullStringWhenObjectIsNull = true, });

            var text = EasyJsonSerializer.Serialize<Exception>(ex);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(ex));
            //Console.WriteLine(EasyJsonSerializer.Default.Serialize<Exception>(ex));
            var ex2 = EasyJsonSerializer.Deserialize<Exception>(text);
            // ex2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Exception>(text);// EasyJsonSerializer.Default.Deserialize<Exception>(text);
        }

        [Xunit.Fact]
        public void TestWriteType()
        {
            var type = typeof(int);
            var text = EasyJsonSerializer.Serialize<Type>(type);
            type = EasyJsonSerializer.Deserialize<Type>(text);
        }

        [Xunit.Fact]
        public void TestWriteTime()
        {
            var time = DateTime.Now;
            var text = EasyJsonSerializer.Serialize(time, new JsonSerializeSetting() { DateTimeFormat = DateTimeFormat.MicrosoftStyle });
            var time2 = EasyJsonSerializer.Deserialize<DateTime>(text);
            text = EasyJsonSerializer.Serialize(time, new JsonSerializeSetting() { DateTimeFormat = DateTimeFormat.ChineseStyle });
            var time3 = EasyJsonSerializer.Deserialize<DateTime>(text, new JsonDeserializeSetting() { DateTimeFormat = DateTimeFormat.ChineseStyle });

            text = EasyJsonSerializer.Serialize(time, new JsonSerializeSetting() { DateTimeFormat = DateTimeFormat.ISO8601Style });
            var time4 = EasyJsonSerializer.Deserialize<DateTime>(text, new JsonDeserializeSetting() { DateTimeFormat = DateTimeFormat.ISO8601Style });

            text = EasyJsonSerializer.Serialize(time, new JsonSerializeSetting() { DateTimeFormat = DateTimeFormat.RFC1123Style });
            var time5 = EasyJsonSerializer.Deserialize<DateTime>(text, new JsonDeserializeSetting() { DateTimeFormat = DateTimeFormat.RFC1123Style });
        }

        [Xunit.Fact]
        public void TestSerializerSpeedTest()
        {
            var timeSpan = TimeSpan.FromHours(2);
            var text = EasyJsonSerializer.Serialize(timeSpan);
            var spsn = EasyJsonSerializer.Deserialize<TimeSpan>(text, new JsonDeserializeSetting() { DateTimeFormat = DateTimeFormat.ChineseStyle });

        }

        [Xunit.Fact]
        public void TestDemo5()
        {
            var target = new TestDemo5()
            {
                Table = new Hashtable(),
                Name = "TET",
            };
            target.Table["A"] = "A";
            target.Table["B"] = new TwoLevel() { Name = 1122, U = "First" };
            //GlobalJsonCompileSetting.Config(new JsonCompileSetting() { UseNumberInEnum = true });
            Console.WriteLine("target:" + target.ToString());
            var text = EasyJsonSerializer.Serialize(target);
            Console.WriteLine("target:" + text);
            var target2 = EasyJsonSerializer.Deserialize<TestDemo5>(text);
            Console.WriteLine("target:" + target.ToString());
        }

        [Xunit.Fact]
        public void TestDemo6()
        {
            //GlobalJsonCompileSetting.Config(new JsonCompileSetting() { WriteNullStringWhenObjectIsNull = false });

            var target = new TestDemo6()
            {
                Name = "\\\\LTET\"0",
                //Name = "\r",
                //Demo3 = new Serialization.TestDemo3()
                //{
                //    Name = "E",
                //    UId = 1265,
                //    ULong = 56465
                //}
            };

            var text = EasyJsonSerializer.Serialize(target);
            var tet = EasyJsonSerializer.Serialize(text);
            Console.WriteLine("target:" + EasyJsonSerializer.Serialize(text));
            Console.WriteLine("target:" + Newtonsoft.Json.JsonConvert.SerializeObject(text));
            var t2 = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(EasyJsonSerializer.Serialize(text));
            //var t3 = Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>("2015-06-23");
            var target2 = EasyJsonSerializer.Deserialize<TestDemo6>(t2);
            var targe3 = Newtonsoft.Json.JsonConvert.DeserializeObject<TestDemo6>(t2);
        }

        [Xunit.Fact]
        public void TestParse()
        {
            var text = File.ReadAllText(@"c:\json.txt");
            var reader = Never.Serialization.Json.ThunderReader.Load(text);
            var obj = Never.Serialization.EasyJsonSerializer.Deserialize<MyyyTest>(reader);
            // var arrar = Never.Serialization.EasyJsonSerializer.Default.Deserialize<ReinvestArrivedReqs[]>(reader);
        }

        public class MyyyTest
        {
            public int Error { get; set; }
            public string Msg { get; set; }

            public string Method { get; set; }

            //public Newtonsoft.Json.Linq.JObject Data { get; set; }

            public System.Collections.Generic.ISet<string> Data { get; set; }
        }

        public class MyJobj<T> : List<T>
        {
        }

        public class MyJobj222 : List<string>
        {
        }

        public class MyyyTestDate
        {
            public MyyyTestDateISPNUM ISPNUM { get; set; }

            public MyyyTestDateRSL[] RSL { get; set; }

            public int[] ECL { get; set; }
        }

        public class MyyyTestDateISPNUM
        {
            public string province { get; set; }
            public string city { get; set; }
            public string isp { get; set; }
        }

        public class MyyyTestDateRSL
        {
            public MyyyTestDateRSLRS RS { get; set; }

            public string IFT { get; set; }
        }

        public class MyyyTestDateRSLRS
        {
            public string code { get; set; }
            public string desc { get; set; }
        }

        /// <summary>
        /// 复投到账请求
        /// </summary>
        public class ReinvestArrivedReqs
        {
            #region prop

            /// <summary>
            /// 用户Id
            /// </summary>
            public long UserId { get; set; }

            /// <summary>
            /// 被打包的项目id
            /// </summary>
            public Guid PackPackageId { get; set; }

            /// <summary>
            /// 被打包的项目名
            /// </summary>
            public string PackPackageName { get; set; }

            /// <summary>
            /// 项目项目
            /// </summary>
            public Guid PackageId { get; set; }

            /// <summary>
            /// 项目名字
            /// </summary>
            public string PackageName { get; set; }

            /// <summary>
            /// 项目利率
            /// </summary>
            public decimal PackageRate { get; set; }

            /// <summary>
            /// 项目奖励利率
            /// </summary>
            public decimal PackageRewardRate { get; set; }

            /// <summary>
            /// 还款期数
            /// </summary>
            public int Period { get; set; }

            /// <summary>
            /// 还款日
            /// </summary>
            public DateTime PayDate { get; set; }

            /// <summary>
            /// 资产代号
            /// </summary>
            public string AssetCode { get; set; }

            /// <summary>
            /// 债权人Id
            /// </summary>
            public long ObligorId { get; set; }

            /// <summary>
            /// 还款本金
            /// </summary>
            public decimal Principal { get; set; }

            /// <summary>
            /// 利息
            /// </summary>
            public decimal Interest { get; set; }

            /// <summary>
            /// 还款服务费
            /// </summary>
            public decimal ServiceFee { get; set; }

            /// <summary>
            /// 交易号
            /// </summary>
            public string TransNo { get; set; }

            #endregion prop
        }

        public class JoinCompanyModel
        {
            /// <summary>
            /// 公司全称
            /// </summary>
            public string CompanyAllName { get; set; }

            /// <summary>
            /// 公司简介
            /// </summary>
            public string CompanyProfile { get; set; }

            /// <summary>
            /// 公司官网
            /// </summary>
            public string CompanyWebSite { get; set; }

            /// <summary>
            /// 附件数据
            /// </summary>
            public string FileData { get; set; }
        }

        public class PersoanlLoanResp
        {
            #region property

            public int Id { get; set; }

            /// <summary>
            /// 唯一标识
            /// </summary>
            public Guid AggregateId { get; set; }

            /// <summary>
            /// 借款人Id
            /// </summary>
            public long UserId { get; set; }

            /// <summary>
            /// 借款人名称
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// 公司Id
            /// </summary>
            public Guid CompanyId { get; set; }

            /// <summary>
            /// 借款期限（天）
            /// </summary>
            public int Duration { get; set; }

            /// <summary>
            /// 期数
            /// </summary>
            public int Period { get; set; }

            /// <summary>
            /// 借款金额
            /// </summary>
            public decimal Amount { get; set; }

            /// <summary>
            /// 借款利率
            /// </summary>
            public decimal Rate { get; set; }

            /// <summary>
            /// 期望到账时间
            /// </summary>
            public DateTime ExpectRepayDate { get; set; }

            /// <summary>
            /// 服务费率
            /// </summary>
            public decimal ServiceFeeRate { get; set; }

            /// <summary>
            /// 放款时间
            /// </summary>
            public DateTime? GrantLoanDate { get; set; }

            /// <summary>
            /// 保证人
            /// </summary>
            public string Guarantor { get; set; }

            /// <summary>
            /// 保证人关系
            /// </summary>
            public string GuarantorRelationship { get; set; }

            /// <summary>
            /// 保证人联系方式
            /// </summary>
            public string GuarantorPhone { get; set; }

            /// <summary>
            /// 个人征信报告
            /// </summary>
            public string PersonalCreditReport { get; set; }

            /// <summary>
            /// 保证人工资条
            /// </summary>
            public string GuarantorSalaryBill { get; set; }

            /// <summary>
            /// 风险保证金利率
            /// </summary>
            public decimal SecurityDepositRate { get; set; }

            /// <summary>
            /// 借款日期
            /// </summary>
            public DateTime CreateDate { get; set; }

            /// <summary>
            /// 资产编号
            /// </summary>
            public string AssetCode { get; set; }

            /// <summary>
            /// 资产名字
            /// </summary>
            public string AssetName { get; set; }

            /// <summary>
            /// 赠券Id
            /// </summary>
            public Guid CouponId { get; set; }

            /// <summary>
            /// 赠券金额
            /// </summary>
            public decimal CouponAmount { get; set; }

            /// <summary>
            /// 现金券活动来源
            /// </summary>
            public string CouponSource { get; set; }

            /// <summary>
            /// 个人补贴利率
            /// </summary>
            public decimal SubsidyRate { get; set; }

            #endregion property
        }

        public class ContainInvestModel
        {
            public string Name { get; set; }
            public InvestModel Invest { get; set; }
        }

        public class InvestModel
        {
            /// <summary>
            /// 投资金额
            /// </summary>
            public decimal Amount { get; set; }

            /// <summary>
            /// 优惠券Id
            /// </summary>
            public Guid CouponId { get; set; }

            /// <summary>
            /// 项目ID
            /// </summary>
            public Guid PackageId { get; set; }

            /// <summary>
            /// 加密的项目信息
            /// </summary>
            public string PackageToken { get; set; }

            public string PageSize { get; set; }

            public string ExchangeCode { get; set; }

            public long UserId { get; set; }

            public string UserName { get; set; }
        }

        public class InvestModelSerialier : ISerialierBuilder<InvestModel>, IDeserialierBuilder<InvestModel>
        {
            public void Write(ISerializerWriter writer, JsonSerializeSetting setting, InvestModel model, byte level)
            {
                writer.Write("{");
                writer.Write("Amount:");
                writer.Write(model.Amount.ToString());
                writer.Write("}");
            }

            public InvestModel Parse(IDeserializerReader reader, JsonDeserializeSetting setting, int level)
            {
                return new InvestModel();
            }

            Action<ISerializerWriter, JsonSerializeSetting, InvestModel, byte> ISerialierBuilder<InvestModel>.Build(JsonSerializeSetting setting)
            {
                return this.Write;
            }

            Func<IDeserializerReader, JsonDeserializeSetting, int, InvestModel> IDeserialierBuilder<InvestModel>.Build(JsonDeserializeSetting setting)
            {
                return this.Parse;
            }
        }

        public class AssetAdditionalReqs
        {
            /// <summary>
            /// 用户Id
            /// </summary>
            public long UserId { get; set; }

            /// <summary>
            /// 路径
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// 文件
            /// </summary>
            public string FileData { get; set; }
        }

        [Xunit.Fact]
        public void TestTwoLevel()
        {
            var two = new TwoLevel()
            {
                U = "36}",
            };

            var text = EasyJsonSerializer.Serialize(two);
            var twoc = EasyJsonSerializer.Deserialize<TwoLevelCopy>(text);
        }

        [Xunit.Fact]
        public void TestStringSer()
        {
            System.Convert.ToBase64String(UTF8Encoding.Default.GetBytes("刘侠"));
            var text = "{userid:11,FileData:''}";
            var t = Never.Serialization.EasyJsonSerializer.Deserialize<AssetAdditionalReqs>(text);
            t = Newtonsoft.Json.JsonConvert.DeserializeObject<AssetAdditionalReqs>(text);
            // Newtonsoft.Json.JsonSerializer.Create()
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(null));

            new System.Diagnostics.StackTrace().GetFrames();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(""));

            Console.WriteLine(Never.Serialization.EasyJsonSerializer.Serialize((string)null));

            Console.WriteLine(Never.Serialization.EasyJsonSerializer.Serialize(""));

            var st = new MemoryStream();
            var writer = new StreamWriter(st);

            Never.Serialization.EasyJsonSerializer.Serialize((string)null, writer);
        }

        public class MyString
        {
            public string Text { get; set; }
        }

        [Xunit.Fact]
        public void TestOTo2Array()
        {
            var v = new TwoLevel[][]
                {
                    new TwoLevel[]
                     {
                         new TwoLevel
                         {
                             Name = 234
                         },
                         new TwoLevel
                         {
                             Name = 234
                         }
                     },
                    null,
                    new TwoLevel[]
                     {
                         new TwoLevel
                         {
                             Name = 234
                         },
                         new TwoLevel
                         {
                             Name = 234
                         }
                     },
                };

            var text = Never.Serialization.EasyJsonSerializer.Serialize(v);
            text = Newtonsoft.Json.JsonConvert.SerializeObject(v);
            var list = Never.Serialization.Json.ThunderReader.Load(text);
            v = Never.Serialization.EasyJsonSerializer.Deserialize<TwoLevel[][]>(text);
            v = Newtonsoft.Json.JsonConvert.DeserializeObject<TwoLevel[][]>(text);
        }

        [Xunit.Fact]
        public void TestArray()
        {
            var txt = Never.Serialization.EasyJsonSerializer.Serialize(new ArrayDeci());
        }

        [Xunit.Fact]
        public void TestMyDict()
        {
            var dict = new MyDict();
            dict["A"] = "A";
            dict["B"] = "B";

            var a = new
            {
                DICT = dict,
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(a);
            json = EasyJsonSerializer.Serialize(a);
            json = Jil.JSON.Serialize(a);
        }

        [Xunit.Fact]
        public void TestMyList()
        {
            //var list = new MyList();
            //list.Add(1);
            //list.Add(2);

            //var a = new
            //{
            //    List = list,
            //};
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(a);
            //json = EasyJsonSerializer.Default.Serialize(a);
            //json = Jil.JSON.Serialize(a);

            Nullable<int> a = 2;
            //GlobalJsonCompileSetting.Config(new JsonCompileSetting() { WriteQuotationMarkWhenObjectIsNumber = true });

            var txt = EasyJsonSerializer.Serialize(a);
        }

        [Xunit.Fact]
        public void TestTET()
        {
            var command = new MyStringCommand(NewId.GenerateString()) { Version = 2 };
            var text = EasyJsonSerializer.Serialize(command);

            var cmd1 = EasyJsonSerializer.Deserialize<MyStringCommand>(text);
            var cmd2 = Newtonsoft.Json.JsonConvert.DeserializeObject<MyStringCommand>(text);
        }

        public class MyStringCommand : Never.Domains.StringAggregateCommand
        {
            public MyStringCommand() : this(string.Empty)
            {
            }

            public MyStringCommand(string agg) : base(agg)
            {
            }
        }

        [Xunit.Fact]
        public void TestMachineClass()
        {
            /*json序列化配置*/
            //Never.Serialization.Json.GlobalJsonCompileSetting.Config(new Never.Serialization.Json.JsonCompileSetting()
            //{
            //    UseNumberInEnum = false,
            //    WriteNullStringWhenObjectIsNull = true,GlobalJsonCompileSetting
            //    WriteQuotationMarkWhenObjectIsNumber = false,
            //    WriteQuotationMarkWhenObjectIsBoolean = false,
            //    DateTimeFormat = Never.Serialization.Json.DateTimeFormat.ChineseStyle,
            //});

            var target = new MachineClass() { Machine = Machine.mobile };
            var text = EasyJsonSerializer.Serialize(target);
            Console.WriteLine(text);
            //var source = EasyJsonSerializer.Default.Deserialize<MachineClass>(text);

            //text = "{\"Machine\":\"3\"}";
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public void TestRef(int a, ref int b, out int c)
        {
            c = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public void TestRef2(int a, ref int b, out IMock c)
        {
            c = default(IMock);
        }

        [Xunit.Fact]
        public void TestArrayLevel()
        {
            var text = "[[[a,b],[c,d]]]";
            var list = ThunderReader.Load(text);
        }

        [Xunit.Fact]
        public void TestArrayLevel2()
        {
            var text = "[[[a,b],[c,d]]]";
            var list = ThunderReader.Load(text);
        }

        [Xunit.Fact]
        public void TestArrayLevel3()
        {
            var text = System.IO.File.ReadAllText(@"c:\json.txt");
            var list = ThunderReader.Load(text);
            var cmd = EasyJsonSerializer.Deserialize<MyStringCommand>(list);
            text = System.IO.File.ReadAllText(@"c:\json.txt");
            list = ThunderReader.Load(text);

            text = System.IO.File.ReadAllText(@"c:\json.txt");
            list = ThunderReader.Load(text);
        }

        [Xunit.Fact]
        public void TestTewt()
        {
            var text = "{a:1 }";
            var reader = ThunderReader.Load(text);

            text = " 1";
            reader = ThunderReader.Load(text);
            var it = Never.Serialization.EasyJsonSerializer.Deserialize<int>(reader);
        }

        [Xunit.Fact]
        public void TestMyString()
        {
            var obj = new
            {
                A = new { B = 1, C = "EFG" }
            };

            var text = EasyJsonSerializer.Serialize(obj);
            var ta = EasyJsonSerializer.Deserialize<Dictionary<string, string>>(text);
        }


        [Xunit.Fact]
        public void TestIser3()
        {
            var text = System.IO.File.ReadAllText(@"d:\json.txt");
            var list = ThunderReader.Load(text);
            var a = EasyJsonSerializer.Deserialize<Dictionary<string, string>>(list);
            list = ThunderReader.Load(text);
            var b = EasyJsonSerializer.Deserialize<MyUser>(list);
        }

        public struct MyUser
        {
            public long UserId { get; set; }

            public string Name { get; set; }
        }
    }
}