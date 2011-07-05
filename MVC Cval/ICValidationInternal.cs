using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVC_Cval
{
    internal interface ICValidationInternal
    {
        IValueProvider ValueProvider { get; set; }

        string PropertyName { get; set; }

        string ContainerName { get; set; }

        ModelStateDictionary ModelState { get; set; }
    }
}
