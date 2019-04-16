using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using System.Web.WebSockets;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Filters;
using CountingKs.Models;
using WebGrease;

namespace CountingKs.Controllers
{
    [CountingKsAuthorize(false)]
    [RoutePrefix("api/nutrition/foods")]
    public class FoodsController : BaseApiController
    {
        private const int PAGE_SIZE = 2; // 2 items ;-(
        public FoodsController(ICountingKsRepository repo): base(repo)
        {
        }

        //[Authorize]
        [Route("", Name="Foods")]
        public IHttpActionResult Get(bool includeMeasures = true, int page = 0)
        {
            IQueryable<Food> query;

            if (includeMeasures)
            {
                query = TheRepository.GetAllFoodsWithMeasures();
            }
            else
            {
                query = TheRepository.GetAllFoods();
            }

            var baseQuery = query.OrderBy(f => f.Description);
            var totalCount = baseQuery.Count();                   
            var totalPages = Math.Ceiling((double)totalCount / PAGE_SIZE);

            var helper = new UrlHelper(Request);
            var prevUrl = page > 0 ? helper.Link("Foods", new {page = page - 1}) : "";
            var nextUrl = page < totalPages - 1 ? helper.Link("Foods", new { page = page + 1 }) : "";

            var results = baseQuery.Skip(PAGE_SIZE * page)
                                  .Take(PAGE_SIZE)
                                  .ToList()
                                  .Select(f => TheModelFactory.Create(f));

            return Ok( new
                        {
                            TotalCount = totalCount,
                            TotalPage = totalPages,
                            PreviousPage = prevUrl,
                            NextPage= nextUrl,
                            Results = results 
                        }
            );
        }

        //Named route (Food) in WebApi2
        //[Route("{foodid}", Name="Food")]
        //public FoodModel Get(int foodid)
        //{
        //    return TheModelFactory.Create(TheRepository.GetFood(foodid));
        //}

       // Named route(Food) in WebApi2
        [Route("{foodid}", Name = "Food")]
        public IHttpActionResult Get(int foodid)
        {
            return Versioned(TheModelFactory.Create(TheRepository.GetFood(foodid)));
        }

    }
}
