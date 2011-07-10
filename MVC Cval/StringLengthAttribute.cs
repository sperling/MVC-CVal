using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCCval
{
    public class StringLengthAttribute : System.ComponentModel.DataAnnotations.StringLengthAttribute, IClientValidatable, ICValidation, ICValidationInternal
    {
        public StringLengthAttribute(string conditionProperty, int maximumLength)
            : base(maximumLength)
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
            return new[] { new ModelClientValidationStringLengthRule(this.FormatErrorMessage(metadata.GetDisplayName()), this, this, MinimumLength, MaximumLength) };
        }

        #endregion

        #region ICValidation Members

        public bool ValidateIfNot
        {
            get;
            set;
        }

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

        class ModelClientValidationStringLengthRule : ConditionalModelClientValidationRule
        {
            public ModelClientValidationStringLengthRule(string errorMessage, ICValidation validation, ICValidationInternal validationInternal, int minimumLength, int maximumLength)
                : base(errorMessage, validation, validationInternal)
            {
                if (minimumLength != 0)
                {
                    ValidationParameters["min"] = minimumLength;
                }

                if (maximumLength != Int32.MaxValue)
                {
                    ValidationParameters["max"] = maximumLength;
                }
            }

            protected override string OriginalValidationType
            {
                get { return "length"; }
            }
        }
    }

    internal class StringLengthAttributeAdapter : BaseAttributeAdapter<StringLengthAttribute>
    {
        public StringLengthAttributeAdapter(ModelMetadata metadata, ControllerContext context, StringLengthAttribute attribute)
            : base(metadata, context, attribute)
        {

        }
    }
}
