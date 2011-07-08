using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCCval
{
    public class RangeAttribute : System.ComponentModel.DataAnnotations.RangeAttribute, IClientValidatable, ICValidation, ICValidationInternal
    {
        public RangeAttribute(string conditionProperty, double minimum, double maximum)
            : base(minimum, maximum)
        {
            if (String.IsNullOrEmpty(conditionProperty))
            {
                throw Helpers.NullOrEmptyException("conditionProperty");
            }

            ((ICValidationInternal)this).ConditionProperty = conditionProperty;
        }

        public RangeAttribute(string conditionProperty, int minimum, int maximum)
            : base(minimum, maximum)
        {
            if (String.IsNullOrEmpty(conditionProperty))
            {
                throw Helpers.NullOrEmptyException("conditionProperty");
            }

            ((ICValidationInternal)this).ConditionProperty = conditionProperty;
        }

        public RangeAttribute(string conditionProperty, Type type, string minimum, string maximum)
            : base(type, minimum, maximum)
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
            string errorMessage = ErrorMessage; // Per Dev10 Bug #923283, need to make sure ErrorMessage is called before Minimum/Maximum

            return new[] { new ModelClientValidationRangeRule(this.FormatErrorMessage(metadata.GetDisplayName()), this, this, Minimum, Maximum) };
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

        class ModelClientValidationRangeRule : ConditionalModelClientValidationRule
        {
            public ModelClientValidationRangeRule(string errorMessage, ICValidation validation, ICValidationInternal validationInternal, object minValue, object maxValue)
                : base(errorMessage, validation, validationInternal)
            {
                ValidationParameters["min"] = minValue;
                ValidationParameters["max"] = maxValue;
            }

            protected override string OriginalValidationType
            {
                get { return "range"; }
            }
        }
    }

    internal class RangeAttributAdapter : BaseAttributeAdapter<RangeAttribute>
    {
        public RangeAttributAdapter(ModelMetadata metadata, ControllerContext context, RangeAttribute attribute)
            : base(metadata, context, attribute)
        {

        }
    }
}
