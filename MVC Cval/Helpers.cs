using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Globalization;

namespace MVCCval
{
    public static class Helpers
    {
        private static readonly Type _boolType = typeof(bool);

        private static bool _InitCalled = false;

        /// <summary>
        /// Main validation method.
        /// </summary>
        /// <param name="validation"></param>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <param name="baseFunction"></param>
        /// <param name="validationInternal"></param>
        /// <returns></returns>
        internal static ValidationResult CValidate(this ICValidation validation, object value, ValidationContext validationContext, Func<object, ValidationContext, ValidationResult> baseFunction, ICValidationInternal validationInternal)
        {
            var valueProviderResult = validationInternal.ValueProvider.GetValue(validationInternal.ConditionProperty);

            if (valueProviderResult == null)
            {
                // value for condition property is not supplied.
                return new ValidationResult(String.Format(CultureInfo.CurrentCulture, "Could not find a property named {0}.", validationInternal.ConditionProperty));
            }

            bool shouldValidate = (bool)valueProviderResult.ConvertTo(_boolType);

            // invert ValidateIfNot will give us correct right condition for test.
            bool condition = !validation.ValidateIfNot;

            if (shouldValidate == condition)
            {
                // let the normal validation handle it.
                return baseFunction(value, validationContext);
            }
            else
            {
                // TODO:    what if ObjectInstance is null here?
                // set property on model equalt to default(T) where T is property type.
                var property = validationContext.ObjectInstance.GetType().GetProperty(validationInternal.PropertyName);
                var v = property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;
                property.SetValue(validationContext.ObjectInstance, v, null);

                // also set ViewData.ModelState entry to default(T) where T is property type.
                // or else Html.TextBoxFor() etc. will pick up value from request and render it.
                ModelState modelState;
                string key = validationInternal.ContainerName != "" ? validationInternal.ContainerName + "." + validationInternal.PropertyName : validationInternal.PropertyName;

                if (validationInternal.ModelState.TryGetValue(key, out modelState))
                {
                    if (modelState.Value != null)
                    {
                        // these properties has proteced setters, so need a bit of reflection to
                        // clear out.
                        var propertyDesc = modelState.Value.GetType().GetProperty("RawValue");
                        propertyDesc.SetValue(modelState.Value, v, null);

                        propertyDesc = modelState.Value.GetType().GetProperty("AttemptedValue");
                        propertyDesc.SetValue(modelState.Value, null, null);
                    }
                }

                return null;
            }
        }

        internal static ArgumentException NullOrEmptyException(string paramName)
        {
            // TODO:    make resource and be able to localize.
            return new ArgumentException("Value cannot be null or empty.", paramName);
        }

        public static void Init()
        {
            // don't do it if user already called Init for some reason and
            // we are calling it now from MVCCvalInit.
            if (!_InitCalled)
            {
                _InitCalled = true;
                DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredAttribute), typeof(MVCCval.RequiredAttributeAdapter));
                DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(StringLengthAttribute), typeof(MVCCval.StringLengthAttributeAdapter));
                DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RangeAttribute), typeof(MVCCval.RangeAttributeAdapter));
                DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RegularExpressionAttribute), typeof(MVCCval.RegularExpressionAttributeAdapter));
                DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(CompareAttribute), typeof(MVCCval.CompareAttributeAdapter));

                var wrapModelValidatorProvider = new WrapModelValidatorProvider(new ModelValidatorProviderCollection(ModelValidatorProviders.Providers.ToList()));
                ModelValidatorProviders.Providers.Clear();
                ModelValidatorProviders.Providers.Add(wrapModelValidatorProvider);
            }
        }
    }
}
