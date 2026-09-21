using System;

namespace AccountDocApi.DTOs
{
    /// <summary>
    /// Represents the Document Signature record with Base64-encoded binary content wrapped in a standard JSON object for Nintex mapping.
    /// </summary>
    public class DocumentSignatureDto
    {
        /// <summary>
        /// Unique Document ID
        /// </summary>
        public int DocumentId { get; set; }

        /// <summary>
        /// Account Number associated with this signature document
        /// </summary>
        public string AccountNo { get; set; } = string.Empty;

        /// <summary>
        /// Type of document (e.g. Signature)
        /// </summary>
        public string DocumentType { get; set; } = string.Empty;

        /// <summary>
        /// Document Name
        /// </summary>
        public string DocumentName { get; set; } = string.Empty;

        /// <summary>
        /// External or Storage URL of the document
        /// </summary>
        public string DocumentUrl { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when document was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Base64 encoded file attachment content wrapped inside standard JSON object for Nintex database buffer processing
        /// </summary>
        public string FileDataBase64 { get; set; } = string.Empty;

        /// <summary>
        /// Original filename for Nintex binary file creation
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// MIME Content Type (e.g. image/png, application/pdf)
        /// </summary>
        public string ContentType { get; set; } = "image/png";
    }
}
