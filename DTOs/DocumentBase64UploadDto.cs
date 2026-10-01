using System.ComponentModel.DataAnnotations;

namespace AccountDocApi.DTOs
{
    /// <summary>
    /// Model for uploading a document via JSON payload with Base64 encoded string
    /// </summary>
    public class DocumentBase64UploadDto
    {
        /// <summary>
        /// Base64 encoded file content string
        /// </summary>
        /// <example>iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==</example>
        [Required]
        public string FileDataBase64 { get; set; } = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";

        /// <summary>
        /// File name (e.g. document.pdf, signature.png)
        /// </summary>
        /// <example>NationalID_ACC1001.png</example>
        [Required]
        public string FileName { get; set; } = "NationalID_ACC1001.png";

        /// <summary>
        /// Type of document (e.g. Signature, NationalID, Passport, General)
        /// </summary>
        /// <example>NationalID</example>
        public string? DocumentType { get; set; } = "NationalID";
    }
}
