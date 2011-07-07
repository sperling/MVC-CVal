using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCCval
{
    public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute, IClientValidatable, ICValidation, ICValidationInternal
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConditionProperty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ValidateIfNot { get; set; }

        /*internal IValueProvider ValueProvider {get;set;}

        internal string PropertyName { get; set; }
        
        internal string ContainerName { get; set; }

        internal ModelStateDictionary ModelState { get; set; }*/

        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var result = this.CValidate(value, validationContext, base.IsValid, (ICValidationInternal)this);
            
            return result;
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


        #region ICValidationInternal Members

        IValueProvider ICValidationInternal.ValueProvider
        {
            get
            ;
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
    }

    internal class RequiredAttributAdapter : BaseAttributeAdapter<RequiredAttribute>
    {
        public RequiredAttributAdapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute) : base(metadata, context, attribute)
        {

        }
    }
}
