using AccountDocApi.Data;
using AccountDocApi.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AccountDocApi.Controllers
{
    [ApiController]
    [Route("accounts/{accountNo}/documents")]
    [Produces("application/json")]
    public class DocumentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocumentsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the Signature document for the specified Account Number with Base64-encoded file content for Nintex mapping.
        /// </summary>
        /// <param name="accountNo">The Account Number associated with the signature document (e.g. ACC1001)</param>
        /// <returns>JSON object containing Document metadata and Base64 encoded file string</returns>
        [HttpGet("signature")]
        [ProducesResponseType(typeof(DocumentSignatureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DocumentSignatureDto>> GetSignatureDocument(string accountNo)
        {
            var document = await _context.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.AccountNo == accountNo && d.DocumentType.ToLower() == "signature");

            if (document == null)
            {
                return NotFound(new { message = $"Signature document for Account '{accountNo}' was not found." });
            }

            string base64Content = string.Empty;
            if (document.FileContent != null && document.FileContent.Length > 0)
            {
                base64Content = Convert.ToBase64String(document.FileContent);
            }

            var dto = new DocumentSignatureDto
            {
                DocumentId = document.DocumentId,
                AccountNo = document.AccountNo,
                DocumentType = document.DocumentType,
                DocumentName = document.DocumentName,
                DocumentUrl = document.DocumentUrl,
                CreatedDate = document.CreatedDate,
                FileDataBase64 = base64Content,
                FileName = document.DocumentName,
                ContentType = GetContentType(document.DocumentName)
            };

            return Ok(dto);
        }

        private static string GetContentType(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return "application/octet-stream";
            if (filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) return "image/png";
            if (filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)) return "image/jpeg";
            if (filename.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) return "application/pdf";
            return "application/octet-stream";
        }
    }
}
