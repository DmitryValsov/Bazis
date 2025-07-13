using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Bazis.Data;
using Bazis.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bazis.Services
{
    public class DataSeeder : IHostedService
{
    private readonly IServiceProvider _sp;
    public DataSeeder(IServiceProvider sp) => _sp = sp;

    public async Task StartAsync(CancellationToken token)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync(token);

        // 1) Центры
        if (!await db.ServiceCenters.AnyAsync(token))
        {
            for (int i = 5; i <= 5; i++)
                db.ServiceCenters.Add(new ServiceCenter { Name = "Базис-сервис - 11 км Окружной дороги,6",  Address = "Базис-сервис - 11 км Окружной дороги,6", Phone = "+7 (3452) 39‒46‒96", OpeningHours = "9:00-20:00", Rating = "4.9"});
                 db.ServiceCenters.Add(new ServiceCenter { Name = "Базис-сервис - Федюнинского,83",  Address = "Базис-сервис - Федюнинского,83", Phone = "+7 (3452) 57‒91‒51", OpeningHours = "9:00-20:00", Rating = "4.9"});
                  db.ServiceCenters.Add(new ServiceCenter { Name = "Базис-сервис - Пермякова, 19",  Address = "Базис-сервис - Пермякова, 19", Phone = "+73452579838", OpeningHours = "9:00-20:00", Rating = "4.9"});
                  db.ServiceCenters.Add(new ServiceCenter { Name = "Базис-сервис - Окружная дорога, 202",  Address = "Базис-сервис - Окружная дорога, 202", Phone = "+7 (3452) 56‒44‒35", OpeningHours = "9:00-20:00", Rating = "4.8"});

            await db.SaveChangesAsync(token);
        }



            // 2) Подъёмники
            if (!await db.Lifts.AnyAsync(token))
            {
                var centers = await db.ServiceCenters.ToListAsync(token);
                foreach (var c in centers)
                    for (int j = 1; j <= 4; j++)
                        db.Lifts.Add(new Lift
                        {
                            ServiceCenterId = c.ServiceCenterId,
                            Name = $"Подъёмник {j}"
                        });
                await db.SaveChangesAsync(token);
            }

        // 3) Типы услуг
        if (!await db.ServiceTypes.AnyAsync(token))
        {
            db.ServiceTypes.AddRange(
                new ServiceType { Name = "Шиномонтаж", DurationHours = 1 },
                new ServiceType { Name = "Малярка",     DurationHours = 3 },
                new ServiceType { Name = "ТО",          DurationHours = 6 }
                // …добавьте свои
            );
            await db.SaveChangesAsync(token);
        }

        // 4) Начальные слоты на 40 дней вперёд
        if (!await db.Slots.AnyAsync(token))
        {
            var lifts = await db.Lifts.ToListAsync(token);
            var today = DateTime.Today;
            for (int d = 0; d < 40; d++)
            {
                var dt = today.AddDays(d).ToString("yyyy-MM-dd");
                foreach (var lift in lifts)
                    for (int h = 8; h < 20; h++)
                        db.Slots.Add(new Slot {
                            ServiceCenterId = lift.ServiceCenterId,
                            LiftId          = lift.LiftId,
                            Date            = dt,
                            Time            = $"{h:00}:00",
                            IsBooked        = false
                        });
            }
            await db.SaveChangesAsync(token);
        }
    }
    public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}

}