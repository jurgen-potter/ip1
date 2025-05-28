using Google;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("/api/[controller]")]
public class FileProxyController : Controller
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName = "whimp24-bucket";

    public FileProxyController(StorageClient storageClient)
    {
        _storageClient = storageClient;
    }

    [HttpGet("meeting/{meetingId}/{fileName}")]
    [Authorize] // Ensures only authenticated users can access
    public async Task<IActionResult> GetMeetingDocument(int meetingId, string fileName)
    {
        var objectName = $"{meetingId}/{fileName}";

        try
        {
            // Download the file from Google Cloud Storage
            using var stream = new MemoryStream();
            await _storageClient.DownloadObjectAsync(_bucketName, objectName, stream);
            
            stream.Position = 0;
            var fileBytes = stream.ToArray();

            // Determine content type based on file extension
            var contentType = GetContentType(fileName);
            
            return File(fileBytes, contentType, fileName);
        }
        catch (GoogleApiException e) when (e.Error.Code == 404)
        {
            return NotFound("File not found");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving file: {ex.Message}");
        }
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };
    }
}