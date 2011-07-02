using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVC_Cval
{
    internal abstract class ConditionalModelClientValidationRule : ModelClientValidationRule
    {
        public ConditionalModelClientValidationRule(string errorMessage, bool validateIfNot, string conditionProperty)
        {
            ErrorMessage = errorMessage;
            ValidationType = "CV" + VType;
            ValidationParameters.Add("validateifnot", validateIfNot);
            ValidationParameters.Add("conditionproperty", conditionProperty);
        }

        protected abstract string VType { get; }
    }
}
