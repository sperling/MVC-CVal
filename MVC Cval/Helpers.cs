using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Globalization;

namespace MVC_Cval
{
    public static class Helpers
    {
        private static readonly Type _boolType = typeof(bool);

        internal static ValidationResult CValidate(this ICValidation validation, object value, ValidationContext validationContext, Func<object, ValidationContext, ValidationResult> baseFunction, IValueProvider valueProvider)
        {
            var valueProviderResult = valueProvider.GetValue(validation.ConditionProperty);

            if (valueProviderResult == null)
            {
                // value for condition property is not supplied.
                return new ValidationResult(String.Format(CultureInfo.CurrentCulture, "Could not find a property named {0}.", validation.ConditionProperty));
            }

            bool shouldValidate = (bool)valueProviderResult.ConvertTo(_boolType);

            bool condition = !validation.ValidateIfNot;

            if (shouldValidate == condition)
            {
                return baseFunction(value, validationContext);
            }

            return null;
        }

        public static void Init()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredAttribute), typeof(RequiredAttributeAdapter));
        }
    }
}
