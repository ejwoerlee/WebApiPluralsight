using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
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
                Url = _urlHelper.Link("Measures", new { foodid = measure.Food.Id, id = measure.Id }),
                // Met onderstaande constructie wordt automatisch de url parameter ?v=2 aan de link toegevoegd:
                // Url = _urlHelper.Link("Measures", new { foodid = measure.Food.Id,  id = measure.Id, v = 2 }),
                Description = measure.Description,
                Calories = Math.Round(measure.Calories)
            };
        }

        public MeasureV2Model Create2(Measure measure)
        {
            return new MeasureV2Model() {
                Url = _urlHelper.Link("Measures", new { foodid = measure.Food.Id, id = measure.Id}),
                Description = measure.Description,
                Calories = Math.Round(measure.Calories),
                TotalFat =  measure.TotalFat,
                SaturatedFat = measure.SaturatedFat,
                Protein = measure.Protein,
                Carbohydrates = measure.Carbohydrates,
                Fiber = measure.Fiber,
                Sugar = measure.Sugar,
                Sodium = measure.Sodium,
                Iron = measure.Iron,
                Cholestrol = measure.Cholestrol
            };
        }


        public DiaryModel Create(Diary diary)
        {
            var rel = _urlHelper.Link("Diaries", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") });
            var rel2 = _urlHelper.Link("Diaries", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") });

            return new DiaryModel()
            {                
                Links = new List<LinkModel>()
                {
                    CreateLink("Diaries", rel, "GET"),
                    CreateLink("Diaries", rel2, "POST")

                },
               // Url = _urlHelper.Link("Diaries", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") }),
                CurrentDate = diary.CurrentDate,
                Entries = diary.Entries.Select(e => Create(e))
            };
        }

        public LinkModel CreateLink(string href, string rel, string method = "GET", bool isTemplated = false)
        {
            return new LinkModel()
            {
                Href = href,
                Rel = rel,
                Method = method,
                IsTemplated = isTemplated
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

        public Diary Parse(DiaryModel model)
        {
            try {
                var entity = new Diary();

                var selfLink = model.Links.Where(l => l.Rel == "self").FirstOrDefault();
                if (selfLink != null && !string.IsNullOrWhiteSpace(selfLink.Href)) {
                    var uri = new Uri(selfLink.Href);
                    entity.Id = int.Parse(uri.Segments.Last());
                }

                entity.CurrentDate = model.CurrentDate;

                if (model.Entries != null) {
                    foreach (var entry in model.Entries)
                        entity.Entries.Add(Parse(entry));
                }

                return entity;
            } catch {
                return null;
            }
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

        public DiarySummaryModel CreateSummary(Diary diary)
        {
            return new DiarySummaryModel()
            {
                DiaryDate = diary.CurrentDate,
                TotalCalories = Math.Round(diary.Entries.Sum(e => e.Measure.Calories * e.Quantity))
            };
        }

        public AuthTokenModel Create(AuthToken authToken)
        {
            return new AuthTokenModel() {
                Token = authToken.Token,
                Expiration = authToken.Expiration
            };
        }


    }
}
