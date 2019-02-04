using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.WebSockets;
using CountingKs.Data;
using CountingKs.Data.Entities;
using WebGrease;

namespace CountingKs.Controllers
{
    
    public class FoodsController : ApiController
    {
        private ICountingKsRepository _repo;

        public FoodsController(ICountingKsRepository repo)
        {
            _repo = repo;

        }

        //[Authorize]
        public IEnumerable<object> Get()
        {
            var results = _repo.GetAllFoodsWithMeasures()
                .OrderBy(f => f.Description)
                .Take(25)
                .ToList()
                .Select(f => new
                {
                    Description = f.Description,
                    Measures = f.Measures.Select(m => new { Description = m.Description, Calories = m.Calories})
                });
            
            return results;
        }
    }
}
