using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;
using CountingKs.Data;
using CountingKs.Data.Entities;

namespace CountingKs.Models
{
    public class ModelFactory
    {
        private ICountingKsRepository _repository;
        private UrlHelper _urlHelper;

        public ModelFactory(HttpRequestMessage request, ICountingKsRepository repository)
        {
            _repository = repository;
            _urlHelper = new UrlHelper(request);
        }
        public FoodModel Create(Food food)
        {
            return new FoodModel()
            {
                Url= _urlHelper.Link("Food", new {id = food.Id}),
                Description = food.Description,
                Measures = food.Measures.Select(m => Create(m))
            };
        }

        public MeasureModel Create(Measure measure)
        {
            return new MeasureModel()
            {
                Url = _urlHelper.Link("Food", new { foodid = measure.Food.Id,  id = measure.Id }),
                Description = measure.Description,
                Calories = Math.Round(measure.Calories)
            };
        }

        public DiaryModel Create(Diary diary)
        {
            return new DiaryModel()
            {
                Url = _urlHelper.Link("Diaries", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") }),
                CurrentDate = diary.CurrentDate,
                Entries = diary.Entries.Select(e => Create(e))
            };
        }

        public DiaryEntryModel Create(DiaryEntry entry)
        {
            return new DiaryEntryModel()
            {
                Url = _urlHelper.Link("DiaryEntries", new { diaryid = entry.Diary.CurrentDate.ToString("yyyy-MM-dd"), id = entry.Id }),
                Quantity = entry.Quantity,
                FoodDescription = entry.FoodItem.Description,
                MeasureDescription = entry.Measure.Description
              //  MeasureUrl = _urlHelper.Link("Measures", new { foodid = entry.FoodItem.Id, id = entry.Measure.Id })
            };
        }

        public DiaryEntry Parse(DiaryEntryModel model)
        {
            try
            {
                var entry = new DiaryEntry();
                if (model.Quantity != default(double))
                {
                    entry.Quantity = model.Quantity;
                }

                if (!string.IsNullOrWhiteSpace(model.MeasureUrl))
                {
                    var uri = new Uri(model.MeasureUrl);
                    var measeId = int.Parse(uri.Segments.Last());
                    var measure = _repository.GetMeasure((measeId));

                    entry.Measure = measure;
                    entry.FoodItem = measure.Food;
                }

                return entry;
            }
            catch
            {
                return null;
            }
        }
    }
}