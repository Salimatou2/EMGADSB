using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EMGADSB.Data;
using EMGADSB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using EMGADSB.Data;
using EMGADSB.Models;

namespace EMGADSB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/api/get-models-by-make")]
        public async Task<IActionResult> GetModelsByMake(int makeId)
        {
            try
            {
                if (makeId <= 0)
                {
                    return BadRequest("L'ID de la marque doit être positif");
                }

                var make = await _context.CarMakes.FindAsync(makeId);
                if (make == null)
                {
                    return NotFound("Marque non trouvée");
                }

                var models = await _context.CarModels
                    .Where(m => m.CarMakeId == makeId)
                    .Select(m => new {
                        id = m.Id,
                        name = m.Name
                    })
                    .ToListAsync();

                return Ok(models);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur est survenue: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            return await _context.Cars
                .Include(c => c.CarMake)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars
                .Include(c => c.CarMake)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            if (car.Year < 2018)
            {
                return BadRequest("L'année de la voiture doit être 2018 ou plus récente.");
            }

            var make = await _context.CarMakes.FindAsync(car.CarMakeId);
            if (make == null)
            {
                return BadRequest("La marque spécifiée n'existe pas.");
            }

            car.DateAdded = DateTime.Now;
            car.IsAvailable = true;
            car.IsSold = false;

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCar(int id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            if (car.Year < 2018)
            {
                return BadRequest("L'année de la voiture doit être 2018 ou plus récente.");
            }

            var make = await _context.CarMakes.FindAsync(car.CarMakeId);
            if (make == null)
            {
                return BadRequest("La marque spécifiée n'existe pas.");
            }

            var originalCar = await _context.Cars.FindAsync(id);
            car.DateAdded = originalCar.DateAdded;

            _context.Entry(originalCar).State = EntityState.Detached;
            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/mark-sold")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkCarAsSold(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            car.IsSold = true;
            car.IsAvailable = false;
            car.DateSold = DateTime.Now;

            _context.Entry(car).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/mark-unavailable")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkCarAsUnavailable(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            car.IsAvailable = false;

            _context.Entry(car).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
