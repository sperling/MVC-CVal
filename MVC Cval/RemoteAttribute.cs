using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCCval
{
    // it's not a server side validation attribute, so no need for ICValidationInternal here or register an adaptor.
    public class RemoteAttribute : System.Web.Mvc.RemoteAttribute, System.Web.Mvc.IClientValidatable, ICValidation
    {
        private string _conditionProperty;

        public RemoteAttribute(string conditionProperty)
            : base()
        {
            if (String.IsNullOrEmpty(conditionProperty))
            {
                throw Helpers.NullOrEmptyException("conditionProperty");
            }

            _conditionProperty = conditionProperty;
        }

        public RemoteAttribute(string conditionProperty, string routeName)
            : base(routeName)
        {
            if (String.IsNullOrEmpty(conditionProperty))
            {
                throw Helpers.NullOrEmptyException("conditionProperty");
            }

            _conditionProperty = conditionProperty;
        }

        public RemoteAttribute(string conditionProperty, string action, string controller)
            : this(conditionProperty, action, controller, null)
        {
        }

        public RemoteAttribute(string conditionProperty, string action, string controller, string areaName)
            : base(action, controller, areaName)
        {
            if (String.IsNullOrEmpty(conditionProperty))
            {
                throw Helpers.NullOrEmptyException("conditionProperty");
            }

            _conditionProperty = conditionProperty;
        }

        #region IClientValidatable Members

        public IEnumerable<System.Web.Mvc.ModelClientValidationRule> GetClientValidationRules(System.Web.Mvc.ModelMetadata metadata, System.Web.Mvc.ControllerContext context)
        {
            yield return new ModelClientValidationRemoteRule(_conditionProperty, ValidateIfNot, FormatErrorMessage(metadata.GetDisplayName()), GetUrl(context), HttpMethod, FormatAdditionalFieldsForClientValidation(metadata.PropertyName));
        }

        #endregion


        public override bool IsValid(object value)
        {
            return base.IsValid(value);
        }

        class ModelClientValidationRemoteRule : System.Web.Mvc.ModelClientValidationRule
        {
            public ModelClientValidationRemoteRule(string conditionProperty, bool validateIfNot, string errorMessage, string url, string httpMethod, string additionalFields)
            {
                ErrorMessage = errorMessage;
                ValidationType = ConditionalModelClientValidationRule.Setup("remote", ValidationParameters, conditionProperty, validateIfNot);
                ValidationParameters["url"] = url;

                if (!string.IsNullOrEmpty(httpMethod))
                {
                    ValidationParameters["type"] = httpMethod;
                }

                ValidationParameters["additionalfields"] = additionalFields;
            }
        }

        #region ICValidation Members

        public bool ValidateIfNot
        {
            get;
            set;
        }

        #endregion
    }
}
