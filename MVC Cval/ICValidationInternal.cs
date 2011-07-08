using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCCval
{
    internal interface ICValidationInternal
    {
        string ConditionProperty { get; set; }

        IValueProvider ValueProvider { get; set; }

        string PropertyName { get; set; }

        string ContainerName { get; set; }

        ModelStateDictionary ModelState { get; set; }
    }
}
