using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoves.Data;
using TechMoves.Interfaces;
using TechMoves.Models;
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

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var data = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
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

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null)
                return NotFound();

            return View(serviceRequest);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public IActionResult Create()
        {
            ViewData["ContractId"] = new SelectList(
                _context.Contracts.Include(c => c.Client)
                    .Select(c => new
                    {
                        c.Id,
                        DisplayText = "Contract #" + c.Id + " - " + c.Client.Name
                    }),
                "Id",
                "DisplayText"
            );

            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ContractId"] = new SelectList(
                    _context.Contracts.Include(c => c.Client)
                        .Select(c => new
                        {
                            c.Id,
                            DisplayText = "Contract #" + c.Id + " - " + c.Client.Name
                        }),
                    "Id",
                    "DisplayText",
                    serviceRequest.ContractId
                );

                return View(serviceRequest);
            }

            // LOAD CONTRACT
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == serviceRequest.ContractId);

            if (contract == null)
                return NotFound();

            // BUSINESS RULE VALIDATION
            if (!_contractService.CanCreateServiceRequest(contract))
            {
                ModelState.AddModelError(
                    "",
                    "Cannot create Service Request because contract is Expired or On Hold."
                );

                ViewData["ContractId"] = new SelectList(
                    _context.Contracts.Include(c => c.Client)
                        .Select(c => new
                        {
                            c.Id,
                            DisplayText = "Contract #" + c.Id + " - " + c.Client.Name
                        }),
                    "Id",
                    "DisplayText",
                    serviceRequest.ContractId
                );

                return View(serviceRequest);
            }

            // SAVE REQUEST
            _context.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
                return NotFound();

            ViewData["ContractId"] = new SelectList(
                _context.Contracts.Include(c => c.Client)
                    .Select(c => new
                    {
                        c.Id,
                        DisplayText = "Contract #" + c.Id + " - " + c.Client.Name
                    }),
                "Id",
                "DisplayText",
                serviceRequest.ContractId
            );

            return View(serviceRequest);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ContractId"] = new SelectList(
                    _context.Contracts.Include(c => c.Client)
                        .Select(c => new
                        {
                            c.Id,
                            DisplayText = "Contract #" + c.Id + " - " + c.Client.Name
                        }),
                    "Id",
                    "DisplayText",
                    serviceRequest.ContractId
                );

                return View(serviceRequest);
            }

            try
            {
                _context.Update(serviceRequest);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceRequestExists(serviceRequest.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE (GET)
        // =========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null)
                return NotFound();

            return View(serviceRequest);
        }

        // =========================
        // DELETE (POST)
        // =========================
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

        // =========================
        // EXISTS CHECK
        // =========================
        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.Id == id);
        }
    }
}