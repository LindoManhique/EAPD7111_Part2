using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoves.Data;
using TechMoves.Interfaces;
using TechMoves.Models;
using TechMoves.Services;
using TechMoves.ViewModels;

namespace TechMoves.Controllers
{
    public class ServiceRequestsController : Controller
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

        // 🔥 INDEX (NOW WITH API + VIEWMODEL)
        public async Task<IActionResult> Index()
        {
            var data = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ToListAsync();

            var rate = await _currencyService.GetUsdToZarRateAsync();

            var vm = data.Select(s => new ServiceRequestViewModel
            {
                ServiceRequest = s,
                UsdToZarRate = rate,
                CostInZar = s.Cost * rate
            }).ToList();

            return View(vm);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            ViewData["ContractId"] =
                new SelectList(_context.Contracts, "Id", "Id");

            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ContractId"] =
                    new SelectList(_context.Contracts, "Id", "Id", serviceRequest.ContractId);

                return View(serviceRequest);
            }

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == serviceRequest.ContractId);

            if (contract == null)
                return NotFound();

            if (!_contractService.CanCreateServiceRequest(contract))
            {
                ModelState.AddModelError("", "Contract is Expired or On Hold.");

                ViewData["ContractId"] =
                    new SelectList(_context.Contracts, "Id", "Id", serviceRequest.ContractId);

                return View(serviceRequest);
            }

            _context.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null) return NotFound();

            ViewData["ContractId"] =
                new SelectList(_context.Contracts, "Id", "Id", serviceRequest.ContractId);

            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ContractId"] =
                    new SelectList(_context.Contracts, "Id", "Id", serviceRequest.ContractId);

                return View(serviceRequest);
            }

            _context.Update(serviceRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}