
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PassengerApi.Model;
using PassengerApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PassengerApi.Controllers
{

    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        public static class CacheKeys
        {
            public static string Entry => "_Entry";
            public static string CallbackEntry => "_Callback";
            public static string CallbackMessage => "_CallbackMessage";
            public static string Parent => "_Parent";
            public static string Child => "_Child";
            public static string DependentMessage => "_DependentMessage";
            public static string DependentCTS => "_DependentCTS";
            public static string Ticks => "_Ticks";
            public static string CancelMsg => "_CancelMsg";
            public static string CancelTokenSource => "_CancelTokenSource";
        }
        private readonly IMemoryCache _cache;
        private readonly ILogger<PassengerController> _logger;
        private readonly IDataRepository<Passenger> _repository;
        private readonly PassengerApiContext _context;
        public PassengerController(IMemoryCache cache, ILogger<PassengerController> logger, IDataRepository<Passenger> repository, PassengerApiContext context)
        {
            _cache = cache;
            _logger = logger;
            _repository = repository;
            _context = context;
        }
        /// <summary>
        /// GET: api/passengers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Passenger> GetPassengers()
        {
            //cache process..
            if (!_cache.TryGetValue(CacheKeys.Entry, out IEnumerable<Passenger> cacheEntry))
            {
                cacheEntry = _context.Passenger.OrderByDescending(p => p.IssueDate);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(CacheKeys.Entry, cacheEntry.ToList(), cacheEntryOptions);
            }
            return cacheEntry;
        }
        /// <summary>
        /// GET: api/passenger/3
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
        }/// <summary>
        /// POST: 
        /// </summary>
        /// <param name="passenger"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostPassenger([FromBody] Passenger passenger)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _repository.Add(passenger);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPassenger", new { id = passenger.UniquePassengerId }, passenger);
        }
        /// <summary>
        /// DELETE: api/passenger/id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
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
