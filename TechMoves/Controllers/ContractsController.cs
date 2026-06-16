using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoves.Models;
using TechMoves.Services;

namespace TechMoves.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApiClientService _api;


    public ContractsController(ApiClientService api)
        {
            _api = api;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index(string status, DateTime? startDate, DateTime? endDate)
        {
            var contracts = await _api.GetContracts();

            if (!string.IsNullOrEmpty(status))
                contracts = contracts.Where(c => c.Status == status).ToList();

            if (startDate.HasValue)
                contracts = contracts.Where(c => c.StartDate >= startDate).ToList();

            if (endDate.HasValue)
                contracts = contracts.Where(c => c.EndDate <= endDate).ToList();

            return View(contracts);
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _api.GetContract(id.Value);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public async Task<IActionResult> Create()
        {
            var clients = await _api.GetClients();

            ViewBag.ClientId = new SelectList(
                clients,
                "Id",
                "Name"
            );

            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                var clients = await _api.GetClients();

                ViewBag.ClientId = new SelectList(
                    clients,
                    "Id",
                    "Name",
                    contract.ClientId
                );

                return View(contract);
            }

            if (file != null && file.Length > 0)
            {
                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/contracts");

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

            await _api.CreateContract(contract);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _api.GetContract(id.Value);

            if (contract == null) return NotFound();

            var clients = await _api.GetClients();

            ViewBag.ClientId = new SelectList(
                clients,
                "Id",
                "Name",
                contract.ClientId
            );

            return View(contract);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile file)
        {
            if (id != contract.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var clients = await _api.GetClients();

                ViewBag.ClientId = new SelectList(
                    clients,
                    "Id",
                    "Name",
                    contract.ClientId
                );

                return View(contract);
            }

            if (file != null && file.Length > 0)
            {
                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/contracts");

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

            await _api.UpdateContract(contract);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE (GET)
        // =========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _api.GetContract(id.Value);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // =========================
        // DELETE (POST)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.DeleteContract(id);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DOWNLOAD PDF
        // =========================
        [HttpGet]
        public IActionResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/contracts",
                fileName
            );

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(filePath);

            return File(bytes, "application/pdf", fileName);
        }
    }


}
