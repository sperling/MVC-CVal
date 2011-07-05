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

        internal string PropertyName { get; set; }
        
        internal string ContainerName { get; set; }

        internal ModelStateDictionary ModelState { get; set; }

        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var result = this.CValidate(value, validationContext, base.IsValid, ValueProvider);

            if (result == null)
            {
                var property = validationContext.ObjectInstance.GetType().GetProperty(PropertyName);
                var v = property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;
                property.SetValue(validationContext.ObjectInstance, v, null);

                ModelState modelState;
                string key = ContainerName != "" ? ContainerName + "." + PropertyName : PropertyName;

                if (ModelState.TryGetValue(key, out modelState)) 
                {
                    if (modelState.Value != null)
                    {
                        var propertyDesc = modelState.Value.GetType().GetProperty("RawValue");
                        propertyDesc.SetValue(modelState.Value, v, null);

                        propertyDesc = modelState.Value.GetType().GetProperty("AttemptedValue");
                        propertyDesc.SetValue(modelState.Value, null, null);
                    }
                }

                /*if (modelState.Value != null) {
                var vr = ValueProvider.GetValue(ContainerName != "" ? ContainerName + "." + PropertyName : PropertyName);

                var propertyDesc = vr.GetType().GetProperty("RawValue");
                propertyDesc.SetValue(vr, v, null);

                propertyDesc = vr.GetType().GetProperty("AttemptedValue");
                propertyDesc.SetValue(vr, null, null);*/
            }
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
        
    }

    internal class RequiredAttributAdapter : DataAnnotationsModelValidator<RequiredAttribute>
    {
        public RequiredAttributAdapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute)
            : base(metadata, context, attribute)
        {
            attribute.ValueProvider = ValueProviderFactories.Factories.GetValueProvider(context);
            attribute.PropertyName = metadata.PropertyName;
            attribute.ContainerName = metadata.ContainerType != null ? metadata.ContainerType.Name : "";
            attribute.ModelState = context.Controller.ViewData.ModelState;
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            return base.Validate(container);
        }
    }
}
