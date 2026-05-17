using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoves.Data;
using TechMoves.Models;

namespace TechMoves.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        //  INDEX (FILTERS INCLUDED)
        public async Task<IActionResult> Index(string status, DateTime? startDate, DateTime? endDate)
        {
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                contracts = contracts.Where(c => c.Status == status);
            }

            if (startDate.HasValue)
            {
                contracts = contracts.Where(c => c.StartDate >= startDate);
            }

            if (endDate.HasValue)
            {
                contracts = contracts.Where(c => c.EndDate <= endDate);
            }

            return View(await contracts.ToListAsync());
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
                return View(contract);
            }

            //  FILE VALIDATION
            if (file != null && file.Length > 0)
            {
                var ext = Path.GetExtension(file.FileName).ToLower();

                if (ext != ".pdf")
                {
                    ModelState.AddModelError("file", "Only PDF files allowed.");
                    ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
                    return View(contract);
                }

                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/contracts");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + ".pdf";
                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                contract.AgreementFilePath = "/uploads/contracts/" + fileName;
            }

            _context.Add(contract);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null) return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);

            return View(contract);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile file)
        {
            if (id != contract.Id) return NotFound();

            var existing = await _context.Contracts.FindAsync(id);

            if (existing == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
                return View(contract);
            }

            // UPDATE FIELDS
            existing.ClientId = contract.ClientId;
            existing.StartDate = contract.StartDate;
            existing.EndDate = contract.EndDate;
            existing.Status = contract.Status;
            existing.ServiceLevel = contract.ServiceLevel;

            //  FILE VALIDATION + REPLACE
            if (file != null && file.Length > 0)
            {
                var ext = Path.GetExtension(file.FileName).ToLower();

                if (ext != ".pdf")
                {
                    ModelState.AddModelError("file", "Only PDF files allowed.");
                    ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
                    return View(contract);
                }

                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/contracts");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + ".pdf";
                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                existing.AgreementFilePath = "/uploads/contracts/" + fileName;
            }

            _context.Update(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DOWNLOAD PDF
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/contracts",
                fileName
            );

            if (!System.IO.File.Exists(path))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(path);

            return File(bytes, "application/pdf", fileName);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}