using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using ThreadStaticAttribute = System.ThreadStaticAttribute;

namespace CountingKs.Services
{
    public interface ICountingKsIdentityService
    {
        string CurrentUser { get; }
    }

    public class CountingKsIdentityService : ICountingKsIdentityService
    {
        public string CurrentUser
        {
            get
            {
                // return "shawnwildermuth2";
                return Thread.CurrentPrincipal.Identity.Name;
            }
        }
    }
}