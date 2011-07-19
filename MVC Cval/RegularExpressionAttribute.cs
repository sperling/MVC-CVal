using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCCval
{
    public class RegularExpressionAttribute : System.ComponentModel.DataAnnotations.RegularExpressionAttribute, IClientValidatable, ICValidation, ICValidationInternal
    {
         public RegularExpressionAttribute(string conditionProperty, string pattern) : base(pattern)
        {
            if (String.IsNullOrEmpty(conditionProperty))
            {
                throw Helpers.NullOrEmptyException("conditionProperty");
            }

            ((ICValidationInternal)this).ConditionProperty = conditionProperty;
        }

        
        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationRegularExpressionRule(this.FormatErrorMessage(metadata.GetDisplayName()), this, this, Pattern) };
        }

        #endregion

        #region ICValidation Members
        
        /// <summary>
        /// 
        /// </summary>
        public bool ValidateIfNot { get; set; }

        #endregion

        #region ICValidationInternal Members

        string ICValidationInternal.ConditionProperty
        {
            get;
            set;
        }

        IValueProvider ICValidationInternal.ValueProvider
        {
            get;
            set;
        }

        string ICValidationInternal.PropertyName
        {
            get;
            set;
        }

        string ICValidationInternal.ContainerName
        {
            get;
            set;
        }

        ModelStateDictionary ICValidationInternal.ModelState
        {
            get;
            set;
        }


        #endregion

        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var result = this.CValidate(value, validationContext, base.IsValid, this);
            
            return result;
        }

        class ModelClientValidationRegularExpressionRule : ConditionalModelClientValidationRule
        {
            public ModelClientValidationRegularExpressionRule(string errorMessage, ICValidation validation, ICValidationInternal validationInternal, string pattern)
                : base(errorMessage, validation, validationInternal)
            {
                ValidationParameters.Add("pattern", pattern);
            }

            protected override string OriginalValidationType
            {
                get { return "regex"; }
            }
        }
    }

    internal class RegularExpressionAttributeAdapter : BaseAttributeAdapter<RegularExpressionAttribute>
    {
        public RegularExpressionAttributeAdapter(ModelMetadata metadata, ControllerContext context, RegularExpressionAttribute attribute)
            : base(metadata, context, attribute)
        {

        }
    }
}
