using System.Net.Http.Json;
using TechMoves.Models;

namespace TechMoves.Services
{
    public class ApiClientService
    {
        private readonly HttpClient _http;

        public ApiClientService(HttpClient http)
        {
            _http = http;
        }

        // =========================
        // CONTRACTS
        // =========================

        public async Task<List<Contract>> GetContracts()
        {
            return await _http.GetFromJsonAsync<List<Contract>>("api/contracts")
                   ?? new List<Contract>();
        }

        public async Task<Contract?> GetContract(int id)
        {
            return await _http.GetFromJsonAsync<Contract>($"api/contracts/{id}");
        }

        public async Task<bool> CreateContract(Contract contract)
        {
            var response = await _http.PostAsJsonAsync("api/contracts", contract);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateContract(Contract contract)
        {
            var response = await _http.PutAsJsonAsync($"api/contracts/{contract.Id}", contract);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContract(int id)
        {
            var response = await _http.DeleteAsync($"api/contracts/{id}");
            return response.IsSuccessStatusCode;
        }

        // =========================
        // CLIENTS
        // =========================

        public async Task<List<Client>> GetClients()
        {
            return await _http.GetFromJsonAsync<List<Client>>("api/clients")
                   ?? new List<Client>();
        }

        public async Task<Client?> GetClient(int id)
        {
            return await _http.GetFromJsonAsync<Client>($"api/clients/{id}");
        }

        public async Task<bool> CreateClient(Client client)
        {
            var response = await _http.PostAsJsonAsync("api/clients", client);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateClient(Client client)
        {
            var response = await _http.PutAsJsonAsync($"api/clients/{client.Id}", client);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClient(int id)
        {
            var response = await _http.DeleteAsync($"api/clients/{id}");
            return response.IsSuccessStatusCode;
        }

        // =========================
        // SERVICE REQUESTS (MISSING BEFORE — THIS IS IMPORTANT)
        // =========================

        public async Task<List<ServiceRequest>> GetServiceRequests()
        {
            return await _http.GetFromJsonAsync<List<ServiceRequest>>("api/servicerequests")
                   ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetServiceRequest(int id)
        {
            return await _http.GetFromJsonAsync<ServiceRequest>($"api/servicerequests/{id}");
        }

        public async Task<bool> CreateServiceRequest(ServiceRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/servicerequests", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateServiceRequest(ServiceRequest request)
        {
            var response = await _http.PutAsJsonAsync($"api/servicerequests/{request.Id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteServiceRequest(int id)
        {
            var response = await _http.DeleteAsync($"api/servicerequests/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}