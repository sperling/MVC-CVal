using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCCval
{
    public class CompareAttribute : System.Web.Mvc.CompareAttribute, System.Web.Mvc.IClientValidatable, ICValidation, ICValidationInternal
    {
        public CompareAttribute(string conditionProperty, string otherProperty)
            : base(otherProperty)
        {
            if (String.IsNullOrEmpty(conditionProperty))
            {
                throw Helpers.NullOrEmptyException("conditionProperty");
            }

            ((ICValidationInternal)this).ConditionProperty = conditionProperty;
        }

        public new IEnumerable<System.Web.Mvc.ModelClientValidationRule> GetClientValidationRules(System.Web.Mvc.ModelMetadata metadata, System.Web.Mvc.ControllerContext context)
        {
            yield return new ModelClientValidationEqualToRule(FormatErrorMessage(metadata.GetDisplayName()), this, this, FormatPropertyForClientValidation(OtherProperty));
        }

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

        System.Web.Mvc.IValueProvider ICValidationInternal.ValueProvider
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

        System.Web.Mvc.ModelStateDictionary ICValidationInternal.ModelState
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

        class ModelClientValidationEqualToRule : ConditionalModelClientValidationRule
        {
            public ModelClientValidationEqualToRule(string errorMessage, ICValidation validation, ICValidationInternal validationInternal, string other)
                : base(errorMessage, validation, validationInternal)
            {
                ValidationParameters["other"] = other;
            }

            protected override string OriginalValidationType
            {
                get { return "equalto"; }
            }
        }
    }

    internal class CompareAttributeAdapter : BaseAttributeAdapter<CompareAttribute>
    {
        public CompareAttributeAdapter(System.Web.Mvc.ModelMetadata metadata, System.Web.Mvc.ControllerContext context, CompareAttribute attribute)
            : base(metadata, context, attribute)
        {

        }
    }
}
