using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.WebSockets;
using CountingKs.Data;
using CountingKs.Data.Entities;

namespace CountingKs.Controllers
{
    
    public class FoodsController : ApiController
    {
        private ICountingKsRepository _repo;

        public FoodsController(ICountingKsRepository repo)
        {
            _repo = repo;

        }

        [Authorize]
        public IEnumerable<Food> Get()
        {
            var results = _repo.GetAllFoods()
                               .OrderBy(f => f.Description)
                               .Take(25);
            return results;
        }
    }
}
