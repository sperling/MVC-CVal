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

        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var modelValidators = _collection.GetValidators(metadata, context);

            // typeof(BaseAttributeAdapter<>).IsAssignableFrom(modelValidators.ToList()[0].GetType().BaseType.GetGenericTypeDefinition())

            return modelValidators;
        }
    }
}
