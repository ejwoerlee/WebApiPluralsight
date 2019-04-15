using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using CountingKs.Filters;
using Newtonsoft.Json.Serialization;
using WebApiContrib.Formatting.Jsonp;

namespace CountingKs
{
    using System.Configuration;
    using System.Web.Http.Cors;
    using System.Web.Http.Dispatcher;
    using CacheCow.Common;
    using CacheCow.Server.EntityTagStore.SqlServer;
    using Services;

    public static class WebApiConfig
  {
      public static void Register(HttpConfiguration config)
      {
        //>>> Zelf toevoegen om met Route attributes in controllers te werken!
         config.MapHttpAttributeRoutes();
        //<<<

          // {controller} niet in routeTemplate, daarom  in defaults
          // new { controller = "Foods" ..
          // letop de naam van een paramater id Get en Post functions van de controller
          // is nu letterlijk 'id'. dus get (int -> id <-)

          //config.Routes.MapHttpRoute(
          //    name: "Food",
          //    routeTemplate: "api/nutrition/foods/{id}",
          //    defaults: new
          //    {
          //        controller = "foods",
          //        id = RouteParameter.Optional,
          //        includeMeasures = RouteParameter.Optional
          //    } //,
          //    //constraints: new { id = "/d+" }
          //);

          config.Routes.MapHttpRoute(
              name: "Measures",
              routeTemplate: "api/nutrition/foods/{foodid}/measures/{id}",
              defaults: new
              {
                  controller = "Measures",
                  id = RouteParameter.Optional
              }
          );

          config.Routes.MapHttpRoute(
              name: "Diaries",
              routeTemplate: "api/user/diaries/{diaryid}",
              defaults: new
              {
                  controller = "diaries",
                  diaryid = RouteParameter.Optional
              }
          );

          config.Routes.MapHttpRoute(
              name: "DiaryEntries",
              routeTemplate: "api/user/diaries/{diaryId}/entries/{id}",
              defaults: new {controller = "diaryentries", id = RouteParameter.Optional}
          );


          config.Routes.MapHttpRoute(
              name: "DiarySummary",
              routeTemplate: "api/user/diaries/{diaryid}/summary",
              defaults:
              new {controller = "diarysummary"}
          );

          config.Routes.MapHttpRoute(
              name: "Token",
              routeTemplate: "api/token",
              defaults: new { controller = "Token" }
          );


            //>> niet gebruiken: bijvoorbeeld 'per ongeluk' api beschikbaar maken..
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new {id = RouteParameter.Optional}
            //);
            //<<

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Versioning
            CreateMediaTypes(jsonFormatter);



#if !DEBUG
// Forece HTTPS on entire API
          config.Filters.Add(new RequireHttpsAttribute());
#endif
            // Add support for JSONP
            // var formatter = new JsonpMediaTypeFormatter(jsonFormatter, "cb");
            // config.Formatters.Insert(0, formatter);


            // Support CORS on the entire API
            // config.EnableCors(new EnableCorsAttribute);

            // Support CORS per method call
            // config.EnableCors();

            // Replace the Controller Configuration (versioning)
            config.Services.Replace(typeof(IHttpControllerSelector),
            new CountingKsControllerSelector(config));

            // Configure Caching/ETag Support
            var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var etagStore = new SqlServerEntityTagStore(connString);            
            var cacheHandler = new CacheCow.Server.CachingHandler(config, etagStore);


            // cacheHandler.AddLastModifiedHeader = true;
            // cacheHandler.CacheControlHeaderProvider = etc etc
            // cacheHandler.CacheRefreshPolicyProvider = etc etc.
            config.MessageHandlers.Add(cacheHandler);

            // Add support CORS
            // Allowing other websites to call into this webapi
            // By default it expects calls only from calls from this api or from a non-website
            // like a mobile app are going to be supported.
            var attr = new EnableCorsAttribute("*", "*", "GET");
            config.EnableCors(attr);
      }

      private static void CreateMediaTypes(JsonMediaTypeFormatter jsonFormatter)
      {
          var mediaTypes = new String[]
          {
              "application/vnd.countingks.food.v1+json",
              "application/vnd.countingks.measure.v1+json",
              "application/vnd.countingks.measure.v2+json",
              "application/vnd.countingks.diary.v1+json",
              "application/vnd.countingks.diaryEntry.v1+json",
          };
          foreach (var mediaType in mediaTypes)
          {
              jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));

          }
      }
  }
}