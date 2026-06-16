using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoves.API.Data;
using TechMoves.API.Models;

namespace TechMoves.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL CONTRACTS
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContracts()
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .ToListAsync();
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
                return NotFound();

            return contract;
        }

        // =========================
        // CREATE CONTRACT (FIXED)
        // =========================
        [HttpPost]
        public async Task<ActionResult<Contract>> CreateContract(Contract contract)
        {
            if (contract == null)
                return BadRequest("Contract cannot be null");

            var clientExists = await _context.Clients
                .AnyAsync(c => c.Id == contract.ClientId);

            if (!clientExists)
                return BadRequest("Client does not exist");

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
        }

        // =========================
        // UPDATE CONTRACT
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(int id, Contract contract)
        {
            if (id != contract.Id)
                return BadRequest();

            var existing = await _context.Contracts.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.ClientId = contract.ClientId;
            existing.StartDate = contract.StartDate;
            existing.EndDate = contract.EndDate;
            existing.Status = contract.Status;
            existing.ServiceLevel = contract.ServiceLevel;
            existing.AgreementFilePath = contract.AgreementFilePath;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // DELETE CONTRACT
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // PATCH STATUS
        // =========================
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            contract.Status = status;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}