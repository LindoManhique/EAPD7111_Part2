using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechMoves.Models;

namespace TechMoves.Models
{
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; }

        public string ServiceLevel { get; set; }

        public string? AgreementFilePath { get; set; }

        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}