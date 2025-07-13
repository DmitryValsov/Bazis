using System;

using System.Globalization;

using System.Linq;

using System.Threading;

using Bazis.Data;

using Bazis.Models;

using Microsoft.Extensions.DependencyInjection;

 

namespace Bazis.Services

{

   public class SlotWindowService(IServiceProvider sp) : IHostedService, IDisposable

   {

       private readonly IServiceProvider _sp = sp;

       private Timer _timer;

 

       // те же константы, что и в DataSeeder

       private const int DaysAhead = 40;

       private const int StartHour = 8;

       private const int SlotCount = 12;

        public Task StartAsync(CancellationToken _)

       {

           RefreshWindow();  

           // запускаем раз в сутки

           var now  = DateTime.Now;

           var next = now.Date.AddDays(1).AddSeconds(10);

           _timer = new Timer(_ => RefreshWindow(), null,

                              next - now, TimeSpan.FromHours(24));

           return Task.CompletedTask;

       }

 

       private void RefreshWindow()

       {

           using var scope = _sp.CreateScope();

           var db    = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

           var today = DateTime.Today;

           var todayStr = today.ToString("yyyy-MM-dd");

 

           // 1) Удаляем все слоты с датой раньше сегодняшней

           var oldSlots = db.Slots

                            .Where(s => s.Date.CompareTo(todayStr) < 0);

           db.Slots.RemoveRange(oldSlots);

           db.SaveChanges();

 

           // 2) Узнаём, до какой даты уже есть слоты

           DateTime maxDate;

           if (db.Slots.Any())

           {

               // выберем максимальную строковую дату из БД

               var maxDateStr = db.Slots.Max(s => s.Date);

               // и распарсим её на клиенте

               maxDate = DateTime.ParseExact(maxDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

           }

           else

           {

               maxDate = today.AddDays(-1);

           }

 

           // 3) Добавляем новые дни вперёд до today + DaysAhead

           var services = db.ServiceCenters.Select(x => x.ServiceCenterId).ToArray();

           var lifts    = db.Lifts    .Select(x => x.LiftId   ).ToArray();

 

           for (var d = maxDate.AddDays(1); d <= today.AddDays(DaysAhead); d = d.AddDays(1))

           {

               var dateStr = d.ToString("yyyy-MM-dd");

               foreach (var svc in services)

               foreach (var lift in lifts)

               {

                   for (int h = StartHour; h < StartHour + SlotCount; h++)

                   {

                       db.Slots.Add(new Slot

                       {

                           ServiceCenterId = svc,

                           LiftId    = lift,

                           Date      = dateStr,

                           Time      = TimeSpan.FromHours(h).ToString(@"hh\:mm"),

                           IsBooked  = false

                       });

                   }

               }

           }

 

           db.SaveChanges();

       }

 

       public Task StopAsync(CancellationToken _) => Task.CompletedTask;

       public void Dispose() => _timer?.Dispose();

   }

}