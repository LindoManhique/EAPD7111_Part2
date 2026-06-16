using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoves.API.Data;
using TechMoves.API.Models;
using TechMoves.API.Services.Interfaces;

namespace TechMoves.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IContractService _contractService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
            AppDbContext context,
            IContractService contractService,
            ICurrencyService currencyService)
        {
            _context = context;
            _contractService = contractService;
            _currencyService = currencyService;
        }

        // =========================
        // GET ALL (SAFE - FULL DATA)
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetServiceRequests()
        {
            var data = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();

            return Ok(data);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequest>> GetServiceRequest(int id)
        {
            var request = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (request == null)
                return NotFound();

            return Ok(request);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> CreateServiceRequest(ServiceRequest serviceRequest)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == serviceRequest.ContractId);

            if (contract == null)
                return NotFound("Contract not found");

            if (!_contractService.CanCreateServiceRequest(contract))
                return BadRequest("Contract is Expired or On Hold");

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetServiceRequest),
                new { id = serviceRequest.Id },
                serviceRequest
            );
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
                return BadRequest();

            var existing = await _context.ServiceRequests.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(serviceRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);

            if (request == null)
                return NotFound();

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}