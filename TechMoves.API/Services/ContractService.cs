using System;
using System.Linq;
using TechMoves.API.Models;
using TechMoves.API.Services.Interfaces;

namespace TechMoves.API.Services
{
    public class ContractService : IContractService
    {
        public bool CanCreateServiceRequest(Contract contract)
        {
            if (contract == null)
                return false;

            if (string.IsNullOrWhiteSpace(contract.Status))
                return false;

            string[] blockedStatuses = { "Expired", "On Hold" };

            return !blockedStatuses.Contains(contract.Status);
        }

        // ================================
        // FILE VALIDATION (TEST REQUIREMENT)
        // ================================
        public bool IsValidFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var extension = System.IO.Path.GetExtension(fileName).ToLower();

            return extension == ".pdf";
        }
    }
}