using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountDocApi.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        [MaxLength(50)]
        public string AccountNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string AccountType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string AccountStatus { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Branch { get; set; } = string.Empty;

        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
