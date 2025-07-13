using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Bazis.Data;

namespace CarLiftBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceCentersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ServiceCentersController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Get()
        {
            var list = _db.ServiceCenters
                .Select(c => new
                {
                    c.ServiceCenterId,
                    c.Name,
                    c.Address,
                    c.Phone,
                    c.Rating,
                    c.MapLink,
                    c.OpeningHours,
                }).ToList();
            return Ok(list);
        }
    }
}