using Microsoft.AspNetCore.Mvc;
using TechMoves.Services;
using TechMoves.Models;

namespace TechMoves.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApiClientService _api;

        public ClientsController(ApiClientService api)
        {
            _api = api;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = await _api.GetClients();
            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _api.GetClient(id.Value);

            if (client == null) return NotFound();

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid)
                return View(client);

            await _api.CreateClient(client);

            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _api.GetClient(id.Value);

            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(client);

            await _api.UpdateClient(client);

            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _api.GetClient(id.Value);

            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.DeleteClient(id);

            return RedirectToAction(nameof(Index));
        }
    }
}