
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Bazis.Data;

namespace CarLiftBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ServiceTypesController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Get()
        {
            var list = _db.ServiceTypes
                .Select(t => new
                {
                    t.ServiceTypeId,
                    t.Name,
                    t.DurationHours,
                }).ToList();
            return Ok(list);
        }
    }
}
