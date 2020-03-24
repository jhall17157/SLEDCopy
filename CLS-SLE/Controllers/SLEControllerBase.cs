using CLS_SLE.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    public abstract class SLEControllerBase : Controller
    {
        private SLEConfigurationInfo _sleConfig;

        public SLEControllerBase()
        {

        }

        protected SLEConfigurationInfo SLEConfig
        {
            get
            {
                if (_sleConfig == null)
                {
                    _sleConfig = new SLEConfigurationInfo();
                }
                return _sleConfig;
            }
        }
    }
}