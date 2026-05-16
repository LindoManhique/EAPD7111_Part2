using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMoves.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }

        public string Description { get; set; }

        public decimal Cost { get; set; }

        public string Status { get; set; }
    }
}