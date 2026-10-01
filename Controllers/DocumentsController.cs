using AccountDocApi.Data;
using AccountDocApi.DTOs;
using AccountDocApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        /// Retrieves an ARRAY list of documents for the specified Account Number with Base64-encoded file content for Nintex / K2 mapping.
        /// </summary>
        /// <param name="accountNo">The Account Number associated with documents (e.g. ACC1001)</param>
        /// <returns>JSON Array of Document metadata and Base64 encoded file strings</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DocumentSignatureDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DocumentSignatureDto>>> GetAllDocuments(string accountNo)
        {
            var cleanAccountNo = string.IsNullOrWhiteSpace(accountNo) ? "ACC1001" : accountNo.Trim();

            List<Document> dbDocuments = new();
            try
            {
                dbDocuments = await _context.Documents
                    .AsNoTracking()
                    .Where(d => d.AccountNo.ToLower() == cleanAccountNo.ToLower())
                    .ToListAsync();
            }
            catch
            {
                // Fallback if database is unreachable on host
            }

            var dtoList = new List<DocumentSignatureDto>();

            if (dbDocuments.Any())
            {
                foreach (var doc in dbDocuments)
                {
                    string base64 = (doc.FileContent != null && doc.FileContent.Length > 0)
                        ? Convert.ToBase64String(doc.FileContent)
                        : "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";

                    dtoList.Add(new DocumentSignatureDto
                    {
                        DocumentId = doc.DocumentId,
                        AccountNo = cleanAccountNo.ToUpper(),
                        DocumentType = doc.DocumentType,
                        DocumentName = doc.DocumentName,
                        DocumentUrl = doc.DocumentUrl ?? $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents",
                        CreatedDate = doc.CreatedDate,
                        FileDataBase64 = base64,
                        FileName = doc.DocumentName,
                        ContentType = GetContentType(doc.DocumentName)
                    });
                }
            }
            else
            {
                // Fallback Mock Data Array for testing / Nintex integration
                dtoList.Add(new DocumentSignatureDto
                {
                    DocumentId = 1,
                    AccountNo = cleanAccountNo.ToUpper(),
                    DocumentType = "Signature",
                    DocumentName = $"Signature_{cleanAccountNo.ToUpper()}.png",
                    DocumentUrl = $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents/signature",
                    CreatedDate = DateTime.UtcNow,
                    FileDataBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==",
                    FileName = $"Signature_{cleanAccountNo.ToUpper()}.png",
                    ContentType = "image/png"
                });

                dtoList.Add(new DocumentSignatureDto
                {
                    DocumentId = 2,
                    AccountNo = cleanAccountNo.ToUpper(),
                    DocumentType = "NationalID",
                    DocumentName = $"NationalID_{cleanAccountNo.ToUpper()}.pdf",
                    DocumentUrl = $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents",
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
                    FileDataBase64 = "JVBERi0xLjQKJcOkw7zDtsOfCjIgMCBvYmoKPDwvTGVuZ3RoIDMgMCBSL0ZpbHRlci9GbGF0ZURlY29kZT4+CnN0cmVhbQp4nG2QwQ3CMBBDd56C1e4iI20a0Z5aJ8AEuID0/1NpBRInx5Z/8n/7vB9m0DnH6J1Lzp6h7N4M7h7G6C1F72+eR2fvnEtrc/c=",
                    FileName = $"NationalID_{cleanAccountNo.ToUpper()}.pdf",
                    ContentType = "application/pdf"
                });
            }

            return Ok(dtoList);
        }

        /// <summary>
        /// Retrieves the Signature document for the specified Account Number with Base64-encoded file content for Nintex mapping.
        /// </summary>
        /// <param name="accountNo">The Account Number associated with the signature document (e.g. ACC1001)</param>
        /// <returns>JSON object containing Document metadata and Base64 encoded file string</returns>
        [HttpGet("signature")]
        [ProducesResponseType(typeof(DocumentSignatureDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentSignatureDto>> GetSignatureDocument(string accountNo)
        {
            var cleanAccountNo = string.IsNullOrWhiteSpace(accountNo) ? "ACC1001" : accountNo.Trim();

            Document? document = null;
            try
            {
                document = await _context.Documents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.AccountNo.ToLower() == cleanAccountNo.ToLower() && d.DocumentType.ToLower() == "signature");
            }
            catch
            {
                // Fallback if database is unreachable on host
            }

            string base64Content = string.Empty;
            if (document != null && document.FileContent != null && document.FileContent.Length > 0)
            {
                base64Content = Convert.ToBase64String(document.FileContent);
            }
            else
            {
                base64Content = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
            }

            var dto = new DocumentSignatureDto
            {
                DocumentId = document?.DocumentId ?? 1,
                AccountNo = cleanAccountNo.ToUpper(),
                DocumentType = document?.DocumentType ?? "Signature",
                DocumentName = document?.DocumentName ?? $"Signature_{cleanAccountNo.ToUpper()}.png",
                DocumentUrl = document?.DocumentUrl ?? $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents/signature",
                CreatedDate = document?.CreatedDate ?? DateTime.UtcNow,
                FileDataBase64 = base64Content,
                FileName = document?.DocumentName ?? $"Signature_{cleanAccountNo.ToUpper()}.png",
                ContentType = GetContentType(document?.DocumentName ?? "Signature.png")
            };

            return Ok(dto);
        }

        /// <summary>
        /// Uploads a new document file for the specified Account Number (Swagger form-data file picker supported).
        /// Stores binary content in PostgreSQL database.
        /// </summary>
        /// <param name="accountNo">The Account Number (e.g. ACC1001)</param>
        /// <param name="uploadDto">Document file and metadata</param>
        /// <returns>Saved Document record metadata and Base64 string</returns>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(DocumentSignatureDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentSignatureDto>> UploadDocument(
            string accountNo,
            [FromForm] DocumentUploadDto uploadDto)
        {
            if (uploadDto.File == null || uploadDto.File.Length == 0)
            {
                return BadRequest(new { message = "Please select a valid non-empty file to upload." });
            }

            var cleanAccountNo = string.IsNullOrWhiteSpace(accountNo) ? "ACC1001" : accountNo.Trim().ToUpper();

            // Ensure Account exists in Accounts table to prevent Foreign Key constraint error
            try
            {
                var existingAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNo.ToLower() == cleanAccountNo.ToLower());

                if (existingAccount != null)
                {
                    cleanAccountNo = existingAccount.AccountNo;
                }
                else
                {
                    var newAccount = new Account
                    {
                        AccountNo = cleanAccountNo,
                        CustomerId = $"CUST-{cleanAccountNo}",
                        CustomerName = $"Customer {cleanAccountNo}",
                        AccountType = "Savings",
                        AccountStatus = "Active",
                        Branch = "Main Branch"
                    };
                    _context.Accounts.Add(newAccount);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                // Proceed if table check fails
            }

            byte[] fileBytes;
            using (var memoryStream = new System.IO.MemoryStream())
            {
                await uploadDto.File.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            var fileName = !string.IsNullOrWhiteSpace(uploadDto.DocumentName)
                ? uploadDto.DocumentName.Trim()
                : uploadDto.File.FileName;

            var docType = !string.IsNullOrWhiteSpace(uploadDto.DocumentType)
                ? uploadDto.DocumentType.Trim()
                : "General";

            var doc = new Document
            {
                AccountNo = cleanAccountNo,
                DocumentType = docType,
                DocumentName = fileName,
                DocumentUrl = $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents",
                CreatedDate = DateTime.UtcNow,
                FileContent = fileBytes
            };

            try
            {
                _context.Documents.Add(doc);
                await _context.SaveChangesAsync();

                doc.DocumentUrl = $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents/{doc.DocumentId}";
                await _context.SaveChangesAsync();
            }
            catch
            {
                try
                {
                    // Auto-fix PostgreSQL identity sequence if it fell behind seeded IDs
                    var fixSequenceSql = @"SELECT setval(pg_get_serial_sequence('""Documents""', 'DocumentId'), COALESCE((SELECT MAX(""DocumentId"") FROM ""Documents""), 1));";
                    await _context.Database.ExecuteSqlRawAsync(fixSequenceSql);

                    _context.Entry(doc).State = EntityState.Detached;
                    doc.DocumentId = 0;
                    _context.Documents.Add(doc);
                    await _context.SaveChangesAsync();

                    doc.DocumentUrl = $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents/{doc.DocumentId}";
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var detailedError = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "Error saving document to database", error = detailedError });
                }
            }

            var base64 = Convert.ToBase64String(fileBytes);

            var dto = new DocumentSignatureDto
            {
                DocumentId = doc.DocumentId,
                AccountNo = cleanAccountNo,
                DocumentType = doc.DocumentType,
                DocumentName = doc.DocumentName,
                DocumentUrl = doc.DocumentUrl,
                CreatedDate = doc.CreatedDate,
                FileDataBase64 = base64,
                FileName = doc.DocumentName,
                ContentType = GetContentType(doc.DocumentName)
            };

            return Ok(dto);
        }

        /// <summary>
        /// Uploads a new document for the specified Account Number via Base64 JSON payload.
        /// Stores binary content in PostgreSQL database.
        /// </summary>
        /// <param name="accountNo">The Account Number (e.g. ACC1001)</param>
        /// <param name="dto">Base64 encoded file string and metadata</param>
        /// <returns>Saved Document record metadata and Base64 string</returns>
        [HttpPost("upload-base64")]
        [ProducesResponseType(typeof(DocumentSignatureDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentSignatureDto>> UploadDocumentBase64(
            string accountNo,
            [FromBody] DocumentBase64UploadDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FileDataBase64))
            {
                return BadRequest(new { message = "FileDataBase64 field is required." });
            }

            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(dto.FileDataBase64);
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Invalid Base64 string encoding." });
            }

            var cleanAccountNo = string.IsNullOrWhiteSpace(accountNo) ? "ACC1001" : accountNo.Trim().ToUpper();

            try
            {
                var existingAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNo.ToLower() == cleanAccountNo.ToLower());

                if (existingAccount != null)
                {
                    cleanAccountNo = existingAccount.AccountNo;
                }
                else
                {
                    var newAccount = new Account
                    {
                        AccountNo = cleanAccountNo,
                        CustomerId = $"CUST-{cleanAccountNo}",
                        CustomerName = $"Customer {cleanAccountNo}",
                        AccountType = "Savings",
                        AccountStatus = "Active",
                        Branch = "Main Branch"
                    };
                    _context.Accounts.Add(newAccount);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
            }

            var fileName = !string.IsNullOrWhiteSpace(dto.FileName) ? dto.FileName.Trim() : "UploadedDocument.bin";
            var docType = !string.IsNullOrWhiteSpace(dto.DocumentType) ? dto.DocumentType.Trim() : "General";

            var doc = new Document
            {
                AccountNo = cleanAccountNo,
                DocumentType = docType,
                DocumentName = fileName,
                DocumentUrl = $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents",
                CreatedDate = DateTime.UtcNow,
                FileContent = fileBytes
            };

            try
            {
                _context.Documents.Add(doc);
                await _context.SaveChangesAsync();

                doc.DocumentUrl = $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents/{doc.DocumentId}";
                await _context.SaveChangesAsync();
            }
            catch
            {
                try
                {
                    var fixSequenceSql = @"SELECT setval(pg_get_serial_sequence('""Documents""', 'DocumentId'), COALESCE((SELECT MAX(""DocumentId"") FROM ""Documents""), 1));";
                    await _context.Database.ExecuteSqlRawAsync(fixSequenceSql);

                    _context.Entry(doc).State = EntityState.Detached;
                    doc.DocumentId = 0;
                    _context.Documents.Add(doc);
                    await _context.SaveChangesAsync();

                    doc.DocumentUrl = $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents/{doc.DocumentId}";
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var detailedError = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "Error saving document to database", error = detailedError });
                }
            }

            var responseDto = new DocumentSignatureDto
            {
                DocumentId = doc.DocumentId,
                AccountNo = cleanAccountNo,
                DocumentType = doc.DocumentType,
                DocumentName = doc.DocumentName,
                DocumentUrl = doc.DocumentUrl,
                CreatedDate = doc.CreatedDate,
                FileDataBase64 = dto.FileDataBase64,
                FileName = doc.DocumentName,
                ContentType = GetContentType(doc.DocumentName)
            };

            return Ok(responseDto);
        }

        /// <summary>
        /// Retrieves a specific document by its Document ID.
        /// </summary>
        /// <param name="accountNo">The Account Number (e.g. ACC1001)</param>
        /// <param name="documentId">The unique Document ID</param>
        /// <returns>Document metadata and Base64 encoded file string</returns>
        [HttpGet("{documentId:int}")]
        [ProducesResponseType(typeof(DocumentSignatureDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentSignatureDto>> GetDocumentById(string accountNo, int documentId)
        {
            var cleanAccountNo = string.IsNullOrWhiteSpace(accountNo) ? "ACC1001" : accountNo.Trim();

            Document? doc = null;
            try
            {
                doc = await _context.Documents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.DocumentId == documentId && d.AccountNo.ToLower() == cleanAccountNo.ToLower());
            }
            catch
            {
            }

            if (doc == null)
            {
                return NotFound(new { message = $"Document with ID {documentId} for account '{cleanAccountNo}' was not found." });
            }

            string base64 = (doc.FileContent != null && doc.FileContent.Length > 0)
                ? Convert.ToBase64String(doc.FileContent)
                : string.Empty;

            var dto = new DocumentSignatureDto
            {
                DocumentId = doc.DocumentId,
                AccountNo = cleanAccountNo.ToUpper(),
                DocumentType = doc.DocumentType,
                DocumentName = doc.DocumentName,
                DocumentUrl = doc.DocumentUrl ?? $"https://kyc.runasp.net/api/accounts/{cleanAccountNo}/documents/{doc.DocumentId}",
                CreatedDate = doc.CreatedDate,
                FileDataBase64 = base64,
                FileName = doc.DocumentName,
                ContentType = GetContentType(doc.DocumentName)
            };

            return Ok(dto);
        }

        /// <summary>
        /// Downloads the raw binary file for a document by Document ID.
        /// </summary>
        /// <param name="accountNo">The Account Number (e.g. ACC1001)</param>
        /// <param name="documentId">The unique Document ID</param>
        /// <returns>Binary file stream</returns>
        [HttpGet("{documentId:int}/download")]
        public async Task<IActionResult> DownloadDocumentFile(string accountNo, int documentId)
        {
            var cleanAccountNo = string.IsNullOrWhiteSpace(accountNo) ? "ACC1001" : accountNo.Trim();

            Document? doc = null;
            try
            {
                doc = await _context.Documents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.DocumentId == documentId && d.AccountNo.ToLower() == cleanAccountNo.ToLower());
            }
            catch
            {
            }

            if (doc == null || doc.FileContent == null || doc.FileContent.Length == 0)
            {
                return NotFound(new { message = $"File content for Document ID {documentId} was not found." });
            }

            var contentType = GetContentType(doc.DocumentName);
            return File(doc.FileContent, contentType, doc.DocumentName);
        }

        private static string GetContentType(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return "application/octet-stream";
            if (filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) return "image/png";
            if (filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)) return "image/jpeg";
            if (filename.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) return "application/pdf";
            if (filename.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)) return "text/plain";
            if (filename.EndsWith(".doc", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)) return "application/msword";
            return "application/octet-stream";
        }
    }
}
