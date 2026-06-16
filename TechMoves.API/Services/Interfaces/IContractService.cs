using TechMoves.API.Models;

namespace TechMoves.API.Services.Interfaces
{
    public interface IContractService
    {
        bool CanCreateServiceRequest(Contract contract);

        bool IsValidFile(string fileName);
    }
}