using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    using System.Xml.Schema;
    using Data;


    [RoutePrefix("api/stats")]
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repo): base(repo)
        {
            
        }

        [Route("")]
        public HttpResponseMessage Get()
        {
            var results = new
            {
                NumFoods = TheRepository.GetAllFoods().Count(),
                NumUsers = TheRepository.GetApiUsers().Count()
            };

            return Request.CreateResponse(results);
        }

        [Route("{id}")]
        public HttpResponseMessage Get(int id)
        {
            if (id == 1)
            {
                return Request.CreateResponse(new {NumFoods = TheRepository.GetAllFoods().Count()});
            }

            if (id == 2)
            {
                return Request.CreateResponse(new { NumApiUsers = TheRepository.GetApiUsers().Count() });
            }

            
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }

}
