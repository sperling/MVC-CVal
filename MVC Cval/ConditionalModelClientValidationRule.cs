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
            ValidationType = "cv" + OriginalValidationType;
            ValidationParameters.Add("conditionproperty", internalValidation.ConditionProperty);
            ValidationParameters.Add("validateifnot", validation.ValidateIfNot.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
        }

        protected abstract string OriginalValidationType { get; }
    }
}
