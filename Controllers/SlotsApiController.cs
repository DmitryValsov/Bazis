
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Bazis.Data;
using Bazis.Models;

namespace CarLiftBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public SlotsController(ApplicationDbContext db) => _db = db;

        public class SlotInfo
        {
            public string Time { get; set; }
            public int    Free { get; set; }
        }

        // GET api/slots?center=1&start=2025-07-10&end=2025-07-20&type=3
        [HttpGet]
        public ActionResult<Dictionary<string, List<SlotInfo>>> Get(
            [FromQuery] int center,
            [FromQuery] string start,
            [FromQuery] string end,
            [FromQuery] int type)
        {
            // 1) длительность услуги
            var svcType = _db.ServiceTypes.Find(type);
            if (svcType == null) return BadRequest("Неверный тип услуги");
            int D = svcType.DurationHours;

            var closeAt = TimeSpan.FromHours(20);

            // 2) получаем все слоты для заданного центра
            var rawSlots = _db.Slots
                .Where(s => s.ServiceCenterId == center && !s.IsBooked)
                .AsEnumerable()  // делаем всё дальше в памяти
                .Where(s => String.Compare(s.Date, start, StringComparison.Ordinal) >= 0 &&
                            String.Compare(s.Date, end, StringComparison.Ordinal) <= 0);

            // 3) фильтруем слоты по времени
            var eligible = rawSlots
                .Where(s => 
                {
                    var ts = TimeSpan.ParseExact(s.Time, @"hh\:mm", null);
                    return ts.Add(TimeSpan.FromHours(D)) <= closeAt;
                });

            // 4) группируем по дате и времени
            var result = eligible
                .GroupBy(s => s.Date)
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .GroupBy(s => s.Time)
                        .OrderBy(g2 => g2.Key)
                        .Select(g2 => new SlotInfo {
                            Time = g2.Key,
                            Free = g2.Count()
                        })
                        .ToList()
                );

            return Ok(result);
        }

        public class BookRequest
        {
            public int    ServiceCenterId { get; set; }
            public int    ServiceTypeId   { get; set; }
            public string Date            { get; set; }
            public string Time            { get; set; }
            public int    Duration        { get; set; }
        }

        // POST api/slots/book
        [HttpPost("book")]
        public IActionResult Book([FromBody] BookRequest req)
        {
            // 1) проверяем услугу
            var svcType = _db.ServiceTypes.Find(req.ServiceTypeId);
            if (svcType == null || svcType.DurationHours != req.Duration)
                return BadRequest(new { error = "Неверная услуга или длительность" });

            // 2) парсим время
            if (!TimeSpan.TryParseExact(req.Time, @"hh\:mm", null, out var startTs))
                return BadRequest(new { error = "Неверный формат времени" });

            // 3) генерим подряд часы
            var times = Enumerable.Range(0, req.Duration)
                                  .Select(i => startTs
                                    .Add(TimeSpan.FromHours(i))
                                    .ToString(@"hh\:mm"))
                                  .ToList();

            // 4) получаем list подъёмников центра
            var liftIds = _db.Lifts
                .Where(l => l.ServiceCenterId == req.ServiceCenterId)
                .Select(l => l.LiftId)
                .ToList();

            int? chosenLift = null;
            // 5) ищем подъёмник с подряд D слотов
            foreach (var liftId in liftIds)
            {
                var count = _db.Slots
                    .Where(s =>
                        s.LiftId          == liftId &&
                        s.ServiceCenterId == req.ServiceCenterId &&
                        s.Date            == req.Date &&
                        !s.IsBooked       &&
                        times.Contains(s.Time))
                    .Count();
                if (count == req.Duration) { chosenLift = liftId; break; }
            }
            if (chosenLift == null)
                return Conflict(new { error = "Нет подряд свободных слотов" });

            // 6) бронируем
            using var tx = _db.Database.BeginTransaction();
            var toBook = _db.Slots
                .Where(s =>
                    s.LiftId          == chosenLift.Value &&
                    s.ServiceCenterId == req.ServiceCenterId &&
                    s.Date            == req.Date &&
                    !s.IsBooked       &&
                    times.Contains(s.Time))
                .ToList();
            toBook.ForEach(s => s.IsBooked = true);
            _db.SaveChanges();
            tx.Commit();

            return Ok(new { success = true });
        }
    }
}
