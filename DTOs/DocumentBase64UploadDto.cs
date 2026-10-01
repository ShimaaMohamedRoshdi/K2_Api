using System.ComponentModel.DataAnnotations;

namespace AccountDocApi.DTOs
{
    /// <summary>
    /// Model for uploading a document via JSON payload with Base64 encoded string
    /// </summary>
    public class DocumentBase64UploadDto
    {
        /// <summary>
        /// Base64 encoded file content
        /// </summary>
        [Required]
        public string FileDataBase64 { get; set; } = string.Empty;

        /// <summary>
        /// File name (e.g. document.pdf, signature.png)
        /// </summary>
        [Required]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Type of document (e.g. Signature, NationalID, Passport, General)
        /// </summary>
        public string? DocumentType { get; set; } = "General";
    }
}
