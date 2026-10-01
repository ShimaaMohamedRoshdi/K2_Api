using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AccountDocApi.DTOs
{
    /// <summary>
    /// Model for uploading a document file via form-data (Swagger / HTTP Form)
    /// </summary>
    public class DocumentUploadDto
    {
        /// <summary>
        /// The file to upload (e.g. PDF, PNG, JPG)
        /// </summary>
        [Required]
        public IFormFile File { get; set; } = null!;

        /// <summary>
        /// Type of document (e.g. Signature, NationalID, Passport, Contract, General)
        /// </summary>
        public string? DocumentType { get; set; } = "General";

        /// <summary>
        /// Optional document name. If left blank, the original filename of the uploaded file will be used.
        /// </summary>
        public string? DocumentName { get; set; }
    }
}
