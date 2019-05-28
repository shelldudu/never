using Never.Aop;
using Never.DataAnnotations;
using Never.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.Web.Mvc.DataAnnotations
{
    /// <summary>
    ///
    /// </summary>
    internal class TypeProcessor : Never.Startups.ITypeProcessor
    {
        /// <summary>
        /// All
        /// </summary>
        internal static readonly IDictionary<Type, ValidatorAttribute> all = null;

        #region ctor
        /// <summary>
        /// Initializes the <see cref="TypeProcessor"/> class.
        /// </summary>
        static TypeProcessor()
        {
            all = new Dictionary<Type, ValidatorAttribute>(20);
        }
        /// <summary>
        ///
        /// </summary>
        public TypeProcessor()
        {
        }

        #endregion ctor

        #region ITypeProcessor

        /// <summary>
        ///
        /// </summary>
        /// <param name="application"></param>
        /// <param name="type"></param>
        public void Processing(IApplicationStartup application, Type type)
        {
            if (type == null)
                return;

            var attributes = type.GetCustomAttributes(typeof(ValidatorAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                foreach (ValidatorAttribute attribute in attributes)
                {
                    var ctors = attribute.ValidatorType.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (ctors.Any(t => t.GetParameters().Length == 0))
                    {
                        all[type] = attribute;
                        return;
                    }
                }

                throw new Exception($"{((ValidatorAttribute)attributes.FirstOrDefault()).ValidatorType} must has no parameters on ctor");
            }

            return;
        }

        #endregion ITypeProcessor
    }
}