using TechMoves.Models;

namespace TechMoves.Services.Interfaces
{
    public interface IContractService
    {
        bool CanCreateServiceRequest(Contract contract);

        bool IsValidFile(string fileName);
    }
}