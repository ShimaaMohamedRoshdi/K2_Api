using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountDocApi.Models
{
    [Table("Documents")]
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AccountNo { get; set; } = string.Empty;

        [ForeignKey(nameof(AccountNo))]
        public Account? Account { get; set; }

        [Required]
        [MaxLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string DocumentName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string DocumentUrl { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Stores the raw binary file content in PostgreSQL bytea column.
        /// </summary>
        public byte[]? FileContent { get; set; }
    }
}
