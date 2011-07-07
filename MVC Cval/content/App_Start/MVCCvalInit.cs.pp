using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: WebActivator.PostApplicationStartMethod(typeof($rootnamespace$.App_Start.MVCCvalInit), "PostStart")]

namespace $rootnamespace$.App_Start
{
    public static class MVCCvalInit
    {
        public static void PostStart() 
        {
            MVCCval.Helpers.Init();
        }
    }
}
