﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using CountingKs.Services;

namespace CountingKs.Controllers
{
    public class DiaryEntriesController : BaseApiController
    {

        private ICountingKsIdentityService _identityService;

        public DiaryEntriesController(ICountingKsRepository repo, ICountingKsIdentityService identityService)
            : base(repo)
        {
            _identityService = identityService;
        }

        public IEnumerable<DiaryEntryModel> Get(DateTime diaryId)
        {
            var results = TheRepository.GetDiaryEntries(_identityService.CurrentUser, diaryId.Date)
                                       .ToList()
                                       .Select(e => TheModelFactory.Create(e));

            return results;
        }

        public HttpResponseMessage Get(DateTime diaryId, int id)
        {
            var result = TheRepository.GetDiaryEntry(_identityService.CurrentUser, diaryId.Date, id);

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(result));
        }

        public HttpResponseMessage Post(DateTime diaryId, [FromBody]DiaryEntryModel model)
        {
            try
            {
                var entity = TheModelFactory.Parse(model);
                if (entity == null)
                {
                    Request.CreateResponse(HttpStatusCode.BadRequest, "could not read diary entry in body");
                }

                var diary = TheRepository.GetDiary(_identityService.CurrentUser, diaryId);
                if (diary == null)
                {
                    Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (diary.Entries.Any(e => e.Measure.Id == entity.Measure.Id))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "duplicate measure not allowed");
                }

                diary.Entries.Add(entity);
                if (TheRepository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, TheModelFactory.Create(entity));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Error saving record to the database.");
                }



            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            } 
        }

        public HttpResponseMessage Delete(DateTime diaryId, int id)
        {
            try
            {
                if (TheRepository.GetDiaryEntries(_identityService.CurrentUser, diaryId).Any(e => e.Id == id) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (TheRepository.DeleteDiaryEntry(id) && TheRepository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                   return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }

        }

    }
}