using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    using System.Web.Http.Cors;
    using System.Xml.Schema;
    using Data;

    // [EnableCors("*", "X-OURAPP", "")] // force including a header in my application
    // [EnableCors("*", "*", "*")] // all operations allowed

    [RoutePrefix("api/stats")]
    // [EnableCors("*", "*", "GET")]
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repo): base(repo)
        {
            
        }

        //[Route("")]
        ////[DisableCors()]
        //public HttpResponseMessage Get()
        //{
        //    var results = new
        //    {
        //        NumFoods = TheRepository.GetAllFoods().Count(),
        //        NumUsers = TheRepository.GetApiUsers().Count()
        //    };

        //    return Request.CreateResponse(results);
        //}

        // web api 2 versie
        [Route("")]
        //[DisableCors()]
        public IHttpActionResult Get()
        {
            var results = new {
                NumFoods = TheRepository.GetAllFoods().Count(),
                NumUsers = TheRepository.GetApiUsers().Count()
            };

            return Ok(results);
        }

        // gebruik het ~teken voor een uitzondering op de standaard route..
        //[Route("~/api/stat/{id:int}")]
        //public HttpResponseMessage Get(int id)
        //{
        //    if (id == 1)
        //    {
        //        return Request.CreateResponse(new {NumFoods = TheRepository.GetAllFoods().Count()});
        //    }

        //    if (id == 2)
        //    {
        //        return Request.CreateResponse(new { NumApiUsers = TheRepository.GetApiUsers().Count() });
        //    }


        //    return Request.CreateResponse(HttpStatusCode.NotFound);
        //}

       // gebruik het ~teken voor een uitzondering op de standaard route..

        [Route("~/api/stat/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            if (id == 1) {
                return Ok(new { NumFoods = TheRepository.GetAllFoods().Count() });
            }

            if (id == 2) {
                return Ok(new { NumApiUsers = TheRepository.GetApiUsers().Count() });
            }


            return NotFound();
        }



        // Route attribute constaints: {name:<constaint>}
        //[Route("~/api/stat/{name:alpha}")]
        //public HttpResponseMessage Get(string name)
        //{
        //    if (name == "foods") {
        //        return Request.CreateResponse(new { NumFoods = TheRepository.GetAllFoods().Count() });
        //    }

        //    if (name == "users") {
        //        return Request.CreateResponse(new { NumApiUsers = TheRepository.GetApiUsers().Count() });
        //    }


        //    return Request.CreateResponse(HttpStatusCode.NotFound);
        //}

        [Route("~/api/stat/{name:alpha}")]
        public IHttpActionResult Get(string name)
        {
            if (name == "foods") {
                Ok(new { NumFoods = TheRepository.GetAllFoods().Count() });
            }

            if (name == "users") {
                Ok(new { NumApiUsers = TheRepository.GetApiUsers().Count() });
            }


            return NotFound();
        }
    }

}
