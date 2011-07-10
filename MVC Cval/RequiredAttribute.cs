using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCCval
{
    public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute, IClientValidatable, ICValidation, ICValidationInternal
    {
        public RequiredAttribute(string conditionProperty) : base()
        {
            if (String.IsNullOrEmpty(conditionProperty))
            {
                throw Helpers.NullOrEmptyException("conditionProperty");
            }

            ((ICValidationInternal)this).ConditionProperty = conditionProperty;
        }

        internal RequiredAttribute(string conditionProperty, bool validateIfNot, System.ComponentModel.DataAnnotations.RequiredAttribute orginalAttribute) : this(conditionProperty)
        {
            ValidateIfNot = validateIfNot;
            this.AllowEmptyStrings = orginalAttribute.AllowEmptyStrings;
            if (!String.IsNullOrEmpty(orginalAttribute.ErrorMessage))
            {
                this.ErrorMessage = orginalAttribute.ErrorMessage;
            }
            if (!String.IsNullOrEmpty(orginalAttribute.ErrorMessageResourceName))
            {
                this.ErrorMessageResourceName = orginalAttribute.ErrorMessageResourceName;
            }
            if (orginalAttribute.ErrorMessageResourceType != null)
            {
                this.ErrorMessageResourceType = orginalAttribute.ErrorMessageResourceType;
            }
        }

        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationRequiredRule(this.FormatErrorMessage(metadata.GetDisplayName()), this, this) };
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

        class ModelClientValidationRequiredRule : ConditionalModelClientValidationRule
        {
            public ModelClientValidationRequiredRule(string errorMessage, ICValidation validation, ICValidationInternal validationInternal) : base(errorMessage, validation, validationInternal)
            {
            }

            protected override string OriginalValidationType
            {
                get { return "required"; }
            }
        }
    }

    internal class RequiredAttributeAdapter : BaseAttributeAdapter<RequiredAttribute>
    {
        public RequiredAttributeAdapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute)
            : base(metadata, context, attribute)
        {

        }
    }
}
