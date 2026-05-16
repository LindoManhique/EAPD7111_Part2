using TechMoves.Models;

namespace TechMoves.ViewModels
{
    public class ServiceRequestViewModel
    {
        public ServiceRequest ServiceRequest { get; set; }

        public decimal UsdToZarRate { get; set; }

        public decimal CostInZar { get; set; }
    }
}