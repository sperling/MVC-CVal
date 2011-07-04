using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVC_Cval
{
    public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute, IClientValidatable, ICValidation
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConditionProperty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ValidateIfNot { get; set; }

        internal IValueProvider ValueProvider {get;set;}

        
        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            return this.CValidate(value, validationContext, base.IsValid, ValueProvider);
        }

        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationRequiredRule(this.FormatErrorMessage(metadata.GetDisplayName()), this) };
        }

        #endregion

        class ModelClientValidationRequiredRule : ConditionalModelClientValidationRule
        {
            public ModelClientValidationRequiredRule(string errorMessage, ICValidation validation) : base(errorMessage, validation)
            {
            }

            protected override string OriginalValidationType
            {
                get { return "required"; }
            }
        }
        
    }

    internal class RequiredAttributAdapter : DataAnnotationsModelValidator<RequiredAttribute>
    {
        public RequiredAttributAdapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute)
            : base(metadata, context, attribute)
        {
            attribute.ValueProvider = ValueProviderFactories.Factories.GetValueProvider(context);
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            return base.Validate(container);
        }
    }
}
