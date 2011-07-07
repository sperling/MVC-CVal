using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: WebActivator.PostApplicationStartMethod(typeof($rootnamespace$.App_Start.MVCCValInit), "PostStart")]

namespace $rootnamespace$.App_Start
{
    public static class MVCCValInit
    {
        public static void PostStart() 
        {
            MVCCval.Helpers.Init();
        }
    } 
}
