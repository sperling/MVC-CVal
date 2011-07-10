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

            cv.ValueProvider = ValueProviderFactories.Factories.GetValueProvider(context);
            // TODO:    what if not generated name/id in view?
            cv.PropertyName = metadata.PropertyName;
            cv.ContainerName = metadata.ContainerType != null ? metadata.ContainerType.Name : "";
            cv.ModelState = context.Controller.ViewData.ModelState;

            IBaseAttributeAdapter i = (IBaseAttributeAdapter)this;

            i.CValidationInternal = cv;
            i.CValidation = (ICValidation)attribute;
        }

        ICValidationInternal IBaseAttributeAdapter.CValidationInternal { get; set; }
        ICValidation IBaseAttributeAdapter.CValidation { get; set; }
    }
}
