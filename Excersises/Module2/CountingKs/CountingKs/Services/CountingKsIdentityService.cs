﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                return "shawnwildermuth";
            }
        }
    }
}