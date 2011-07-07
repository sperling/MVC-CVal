using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCCval
{
    public interface ICValidation
    {
        /// <summary>
        /// 
        /// </summary>
        string ConditionProperty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool ValidateIfNot { get; set; }
    }
}
