using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMGADSB.Data;
using EMGADSB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using EMGADSB.Data;
using EMGADSB.Models;

namespace EMGADSB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarMakesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarMakesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarMake>>> GetCarMakes()
        {
            return await _context.CarMakes.ToListAsync();
        }

        [HttpGet("{makeId}/models")]
        public async Task<ActionResult<IEnumerable<CarModel>>> GetCarModelsByMake(int makeId)
        {
            var carModels = await _context.CarModels
                .Where(m => m.CarMakeId == makeId)
                .ToListAsync();

            return carModels;
        }
    }
}
