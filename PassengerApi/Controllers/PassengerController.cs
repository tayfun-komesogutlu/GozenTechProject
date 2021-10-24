
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PassengerApi.Model;
using PassengerApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PassengerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassengerController : ControllerBase
    {
        private readonly ILogger<PassengerController> _logger;
        private readonly IDataRepository<Passenger> _repository;
        private readonly PassengerApiContext _context;
        public PassengerController(ILogger<PassengerController> logger, IDataRepository<Passenger> repository, PassengerApiContext context)
        {
            _logger = logger;
            _repository = repository;
            _context = context;
        }
        /// <summary>
        /// GET: api/Passengers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Passenger> GetPassengers()
        {
            return _context.Passenger.OrderByDescending(p => p.IssueDate);
        }
        /// <summary>
        /// GET: api/Passenger/3
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPassenger([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var passenger = await _context.Passenger.FindAsync(id);
            if (passenger == null)
                return NotFound();
            return Ok(passenger);
        }
        /// <summary>
        /// PUT: api/Passenger/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="passenger"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPassenger([FromRoute] Guid id, [FromBody] Passenger passenger)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (id != passenger.UniquePassengerId)
                return BadRequest();
            _context.Entry(passenger).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PassengerExist(id))
                    return NotFound();
                else
                    _logger.Log(LogLevel.Error, ex.Message);
            }
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> PostPassenger([FromBody] Passenger passenger)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _repository.Add(passenger);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPassenger", new { id = passenger.UniquePassengerId }, passenger);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassenger([FromRoute] Guid guid)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var passenger = await _context.Passenger.FindAsync(guid);
            if (passenger == null)
                return NotFound();
            await _context.SaveChangesAsync();
            return Ok(passenger);
        }
        private bool PassengerExist(Guid id)
        {
            return _context.Passenger.Any(e => e.UniquePassengerId == id);
        }


    }
}
