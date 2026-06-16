using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoves.Models;
using TechMoves.Services;
using TechMoves.ViewModels;

namespace TechMoves.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApiClientService _api;

        public ServiceRequestsController(ApiClientService api)
        {
            _api = api;
        }

        // =========================
        // INDEX (FIXED)
        // =========================
        public async Task<IActionResult> Index()
        {
            var requests = await _api.GetServiceRequests();

            var vm = requests.Select(r => new ServiceRequestViewModel
            {
                ServiceRequest = r,
                UsdToZarRate = 18.5m,
                CostInZar = r.Cost * 18.5m
            }).ToList();

            return View(vm);
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var request = await _api.GetServiceRequest(id.Value);

            if (request == null) return NotFound();

            return View(request);
        }

        // =========================
        // CREATE (GET) - FIXED (LOAD CONTRACTS)
        // =========================
        public async Task<IActionResult> Create()
        {
            var contracts = await _api.GetContracts();

            ViewBag.ContractId = new SelectList(
                contracts,
                "Id",
                "Id" // or change to a better display field like "Name" if you have it
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
                var contracts = await _api.GetContracts();

                ViewBag.ContractId = new SelectList(
                    contracts,
                    "Id",
                    "Id",
                    serviceRequest.ContractId
                );

                return View(serviceRequest);
            }

            await _api.CreateServiceRequest(serviceRequest);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT (GET) - FIXED
        // =========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var request = await _api.GetServiceRequest(id.Value);

            if (request == null) return NotFound();

            var contracts = await _api.GetContracts();

            ViewBag.ContractId = new SelectList(
                contracts,
                "Id",
                "Id",
                request.ContractId
            );

            return View(request);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var contracts = await _api.GetContracts();

                ViewBag.ContractId = new SelectList(
                    contracts,
                    "Id",
                    "Id",
                    serviceRequest.ContractId
                );

                return View(serviceRequest);
            }

            await _api.UpdateServiceRequest(serviceRequest);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var request = await _api.GetServiceRequest(id.Value);

            if (request == null) return NotFound();

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.DeleteServiceRequest(id);

            return RedirectToAction(nameof(Index));
        }
    }
}