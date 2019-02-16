using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using CountingKs.Services;

namespace CountingKs.Controllers
{
    public class DiarySummaryController : BaseApiController
    {
        private ICountingKsIdentityService _identityService;

        public DiarySummaryController(ICountingKsRepository repo, ICountingKsIdentityService identyService) : base(repo)
        {
            _identityService = identyService;
        }

        public IHttpActionResult Get(DateTime diaryId)
        {
            try
            {
                var diary = TheRepository.GetDiary(_identityService.CurrentUser, diaryId);
                if (diary == null)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }

                return Ok(TheModelFactory.CreateSummary(diary));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e);
            }
        }

    }
}
