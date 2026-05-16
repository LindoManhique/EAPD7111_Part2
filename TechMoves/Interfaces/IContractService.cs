using TechMoves.Models;

namespace TechMoves.Interfaces
{
    public interface IContractService
    {
        bool CanCreateServiceRequest(Contract contract);

        bool IsValidFile(string fileName);
    }
}