using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CountingKs.Services
{
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;

    public class CountingKsControllerSelector:  DefaultHttpControllerSelector
    {
        private HttpConfiguration _config;

        public CountingKsControllerSelector(HttpConfiguration config): base(config)
        {
            _config = config;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            // get a list of the known controllers
            var controllers = GetControllerMapping();

            var routeData = request.GetRouteData();

            var controllerName = (string) routeData.Values["controller"];

            if (controllers.TryGetValue(controllerName, out HttpControllerDescriptor descriptor))
            {
                string version = "2";

                var newName = string.Concat(controllerName, "V", version);
               
                if (controllers.TryGetValue(newName, out HttpControllerDescriptor versionedDescriptor))
                {
                    return versionedDescriptor;
                }
                
                return descriptor;
            }

            return null;
        }
    }
}