using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCCval
{
    internal abstract class ConditionalModelClientValidationRule : ModelClientValidationRule
    {
        public ConditionalModelClientValidationRule(string errorMessage, ICValidation validation, ICValidationInternal internalValidation)
        {
            ErrorMessage = errorMessage;
            ValidationType = "cv" + OriginalValidationType;
            ValidationParameters.Add("conditionproperty", internalValidation.ConditionProperty);
            ValidationParameters.Add("validateifnot", validation.ValidateIfNot);
        }

        protected abstract string OriginalValidationType { get; }
    }
}
