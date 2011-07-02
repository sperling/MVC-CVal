using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVC_Cval
{
    public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute, IClientValidatable
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConditionProperty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ValidateIfNot { get; set; }

        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationRequiredRule(this.FormatErrorMessage(metadata.PropertyName), ValidateIfNot, ConditionProperty) };
        }

        #endregion

        class ModelClientValidationRequiredRule : ConditionalModelClientValidationRule
        {
            public ModelClientValidationRequiredRule(string errorMessage, bool validateIfNot, string conditionProperty) : base(errorMessage, validateIfNot, conditionProperty)
            {
            }

            protected override string VType
            {
                get { return "required"; }
            }
        }
    }
}
