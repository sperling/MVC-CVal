using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Globalization;

namespace MVCCval
{
    internal abstract class ConditionalModelClientValidationRule : ModelClientValidationRule
    {
        public ConditionalModelClientValidationRule(string errorMessage, ICValidation validation, ICValidationInternal internalValidation)
        {
            ErrorMessage = errorMessage;
            ValidationType = Setup(OriginalValidationType, ValidationParameters, internalValidation.ConditionProperty, validation.ValidateIfNot);
        }

        protected abstract string OriginalValidationType { get; }

        /// <summary>
        /// Shared with WrapNumericModelValidator.
        /// </summary>
        /// <param name="validationType"></param>
        /// <param name="validationParameters"></param>
        /// <param name="conditionProperty"></param>
        /// <param name="validateIfNot"></param>
        /// <returns></returns>
        internal static string Setup(string validationType, IDictionary<string, object> validationParameters, string conditionProperty, bool validateIfNot)
        {
            validationParameters.Add("conditionproperty", conditionProperty);
            validationParameters.Add("validateifnot", validateIfNot.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());

            return "cv" + validationType;
        }
    }
}
