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
                #if DEBUG
                    return "shawnwildermuth2";
                #else
                    return Thread.CurrentPrincipal.Identity.Name;
                #endif
            }
        }
    }
}