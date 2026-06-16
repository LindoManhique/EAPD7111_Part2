using TechMoves.API.Models;

namespace TechMoves.API.ViewModels
{
    public class ServiceRequestViewModel
    {
        public ServiceRequest ServiceRequest { get; set; }

        public decimal UsdToZarRate { get; set; }

        public decimal CostInZar { get; set; }
    }
}