using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVC_Cval
{
    internal abstract class ConditionalModelClientValidationRule : ModelClientValidationRule
    {
        public ConditionalModelClientValidationRule(string errorMessage, ICValidation validation)
        {
            ErrorMessage = errorMessage;
            ValidationType = "cv" + OriginalValidationType;
            ValidationParameters.Add("conditionproperty", validation.ConditionProperty);
            ValidationParameters.Add("validateifnot", validation.ValidateIfNot);
        }

        protected abstract string OriginalValidationType { get; }
    }
}
