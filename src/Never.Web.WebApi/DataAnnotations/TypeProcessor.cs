using Never.Aop;
using Never.DataAnnotations;
using Never.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.Web.WebApi.DataAnnotations
{
    internal class TypeProcessor : Never.Startups.ITypeProcessor
    {
        #region field 

        /// <summary>
        /// All
        /// </summary>
        static readonly IDictionary<Type, Tuple<ValidatorAttribute, Func<IValidator>>> all = null;

        #endregion

        #region ctor        
        /// <summary>
        /// Initializes the <see cref="TypeProcessor"/> class.
        /// </summary>
        static TypeProcessor()
        {
            all = new Dictionary<Type, Tuple<ValidatorAttribute, Func<IValidator>>>(20);
        }

        /// <summary>
        ///
        /// </summary>
        public TypeProcessor()
        {
        }

        #endregion ctor

        #region i am validator

        private struct MyValidator : IValidator
        {
            public ValidationResult Validate(object target)
            {
                var validator = target as IAmValidator;
                return validator == null ? ValidationResult.Success : validator.Validate();
            }
        }

        #endregion


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
                    if (attribute.ValidatorType.IsAssignableFromType(typeof(IAmValidator)) == true)
                    {
                        all[type] = new Tuple<ValidatorAttribute, Func<IValidator>>(attribute, () => new MyValidator());
                        return;
                    }

                    if (attribute.ValidatorType.IsAssignableFromType(typeof(IValidator)) == false)
                    {
                        continue;
                    }

                    if (attribute.ValidatorType.IsValueType)
                    {
                        var emit = Never.Reflection.EasyEmitBuilder<Func<IValidator>>.NewDynamicMethod();
                        var loal = emit.DeclareLocal(attribute.ValidatorType);
                        emit.LoadLocalAddress(loal);
                        emit.InitializeObject(attribute.ValidatorType);
                        emit.LoadLocal(loal);
                        emit.Box(attribute.ValidatorType);
                        emit.CastClass(typeof(IValidator));
                        emit.Return();
                        var @delegate = emit.CreateDelegate();
                        all[type] = new Tuple<ValidatorAttribute, Func<IValidator>>(attribute, @delegate);
                        //all[type] = new Tuple<ValidatorAttribute, Func<IValidator>>(attribute, () => System.Activator.CreateInstance(attribute.ValidatorType) as IValidator);
                        return;
                    }

                    var ctors = attribute.ValidatorType.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var ctor = ctors.FirstOrDefault(t => t.GetParameters().Length == 0);
                    if (ctor != null)
                    {
                        var emit = Never.Reflection.EasyEmitBuilder<Func<IValidator>>.NewDynamicMethod();
                        emit.NewObject(ctor);
                        emit.CastClass(typeof(IValidator));
                        emit.Return();
                        var @delegate = emit.CreateDelegate();
                        all[type] = new Tuple<ValidatorAttribute, Func<IValidator>>(attribute, @delegate);
                        //all[type] = new Tuple<ValidatorAttribute, Func<IValidator>>(attribute, () => System.Activator.CreateInstance(attribute.ValidatorType) as IValidator);
                        return;
                    }

                    throw new Exception($"{((ValidatorAttribute)attributes.FirstOrDefault()).ValidatorType} must has no parameters on ctor");
                }
            }
        }

        #endregion ITypeProcessor

        #region contains

        /// <summary>
        /// 尝试获取特性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static bool TryGetAttribute(Type type, out ValidatorAttribute attribute)
        {
            if (all.TryGetValue(type, out var value))
            {
                attribute = value?.Item1;
                return true;
            }

            attribute = null;
            return false;
        }

        /// <summary>
        /// 尝试获取对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        public static bool TryGetValidator(Type type, out IValidator validator)
        {
            if (all.TryGetValue(type, out var value))
            {
                validator = value?.Item2();
                return true;
            }

            validator = null;
            return false;
        }

        #endregion
    }
}