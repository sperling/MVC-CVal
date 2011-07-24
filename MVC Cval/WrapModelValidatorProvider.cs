using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCCval
{
    internal class WrapModelValidatorProvider : ModelValidatorProvider
    {
        private readonly ModelValidatorProviderCollection _collection;

        public WrapModelValidatorProvider(ModelValidatorProviderCollection collection)
        {
            _collection = collection;
        }

        private readonly Type _baseAttributeAdapterType = typeof(BaseAttributeAdapter<>);
        private readonly Type _requiredAttributeType = typeof(System.ComponentModel.DataAnnotations.RequiredAttribute);
        private readonly Type _numericModelValidatorType = typeof(System.Web.Mvc.ClientDataTypeModelValidatorProvider).Assembly.GetType("System.Web.Mvc.ClientDataTypeModelValidatorProvider+NumericModelValidator");

        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var modelValidators = _collection.GetValidators(metadata, context);
            Func<ModelValidator, bool> predicate = x =>
                {
                    var type = x.GetType();

                    if (type.BaseType.IsGenericType && _baseAttributeAdapterType.IsAssignableFrom(type.BaseType.GetGenericTypeDefinition()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };

            // typeof(BaseAttributeAdapter<>).IsAssignableFrom(modelValidators.ToList()[0].GetType().BaseType.GetGenericTypeDefinition())
            var conditionAttributeAdapter = modelValidators.FirstOrDefault(predicate);
            if (conditionAttributeAdapter != null)
            {
                var list = new List<ModelValidator>();
                var conditionBaseAttribute = (IBaseAttributeAdapter)conditionAttributeAdapter;

                foreach (var i in modelValidators)
                {
                    var requiredAttributeAdapter = i as System.Web.Mvc.RequiredAttributeAdapter;

                    if (requiredAttributeAdapter != null)
                    {
                        // DataAnnotationsModelValidatorProvider implicity adds RequiredAttribute for
                        // non-nullable value types. need to work around by adding our own if
                        // we have any conditional attribute.
                        // need orginal attribute for error message and so on. 
                        // so need reflection here.
                        var attributeProperty = requiredAttributeAdapter.GetType().GetProperty("Attribute", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, _requiredAttributeType, new Type[0], null);
                        list.Add(new MVCCval.RequiredAttributeAdapter(metadata, context, new MVCCval.RequiredAttribute(conditionBaseAttribute.CValidationInternal.ConditionProperty, conditionBaseAttribute.CValidation.ValidateIfNot, (System.ComponentModel.DataAnnotations.RequiredAttribute)attributeProperty.GetValue(requiredAttributeAdapter, null))));
                    }
                    else if (i.GetType() == _numericModelValidatorType)
                    {
                        // ClientDataTypeModelValidatorProvider implicity adds validation for 
                        // any numeric value type.
                        // need to add our own if we have any conditional attribute.
                        // it's only client side.
                        list.Add(new WrapNumericModelValidator(metadata, context, i, conditionBaseAttribute.CValidationInternal.ConditionProperty, conditionBaseAttribute.CValidation.ValidateIfNot));
                    }
                    else
                    {
                        list.Add(i);
                    }
                }

                return list;
            }
            else
            {
                return modelValidators;
            }
        }

        class WrapNumericModelValidator : ModelValidator
        {
            private readonly ModelValidator _inner;
            private readonly string _condtionProperty;
            private readonly bool _validateIfNot;

            private readonly IValueProvider _valueProvider;
            private readonly string _propertyName;
            private readonly string _containerName;
            private readonly ModelStateDictionary _modelState;

            public WrapNumericModelValidator(ModelMetadata metadata, ControllerContext controllerContext, ModelValidator inner, string conditionProperty, bool validateIfNot) : base(metadata, controllerContext)
            {
                _inner = inner;
                _condtionProperty = conditionProperty;
                _validateIfNot = validateIfNot;

                BaseAttributeAdapter<RequiredAttribute>.ExtractMetadata(controllerContext, metadata, out _valueProvider, out _propertyName, out _containerName, out _modelState);

            }

            public override IEnumerable<ModelValidationResult> Validate(object container)
            {
                //return _inner.Validate(container);
                return Helpers.CValidate(container, _condtionProperty, _validateIfNot, _propertyName, _containerName, _valueProvider, _modelState, _inner.Validate);
            }

            public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
            {
                foreach (var rule in _inner.GetClientValidationRules())
                {
                    rule.ValidationType = ConditionalModelClientValidationRule.Setup(rule.ValidationType, rule.ValidationParameters, _condtionProperty, _validateIfNot);
                    
                    yield return rule;
                }
            }
        }
    }
}
