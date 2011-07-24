using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCCval
{
    internal interface IBaseAttributeAdapter
    {
        ICValidationInternal CValidationInternal { get; set; }
        ICValidation CValidation { get; set; }
    }

    internal class BaseAttributeAdapter<T> : DataAnnotationsModelValidator<T>, IBaseAttributeAdapter where T : global::System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        public BaseAttributeAdapter(ModelMetadata metadata, ControllerContext context, T attribute) : base(metadata, context, attribute)
        {
            ICValidationInternal cv = (ICValidationInternal)attribute;

            IValueProvider valueProvider;
            string propertyName;
            string containerName;
            ModelStateDictionary modelState;

            ExtractMetadata(context, metadata, out valueProvider, out propertyName, out containerName, out modelState);

            cv.ValueProvider = valueProvider;
            cv.PropertyName = propertyName;
            cv.ContainerName = containerName;
            cv.ModelState = modelState;

            IBaseAttributeAdapter i = (IBaseAttributeAdapter)this;

            i.CValidationInternal = cv;
            i.CValidation = (ICValidation)attribute;
        }

        ICValidationInternal IBaseAttributeAdapter.CValidationInternal { get; set; }
        ICValidation IBaseAttributeAdapter.CValidation { get; set; }

        internal static void ExtractMetadata(ControllerContext context, ModelMetadata metadata, out IValueProvider valueProvider, out string propertyName, out string containerName, out ModelStateDictionary modelState)
        {
            valueProvider = ValueProviderFactories.Factories.GetValueProvider(context);
            // TODO:    what if not generated name/id in view?
            propertyName = metadata.PropertyName;
            containerName = metadata.ContainerType != null ? metadata.ContainerType.Name : "";
            modelState = context.Controller.ViewData.ModelState;
        }
    }
}
