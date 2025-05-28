using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;


public class UploadsController : Controller
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName = "whimp24-bucket";

    public UploadsController(StorageClient storageClient)
    {
        _storageClient = storageClient;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        // Genereer unieke bestandsnaam
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        try
        {
            using var stream = file.OpenReadStream();
            await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: $"uploads/{uniqueFileName}",  // Opslaan in "uploads/" map in de bucket
                contentType: file.ContentType,
                source: stream
            );
        }
        catch (Google.GoogleApiException ex)
        {
            return BadRequest("Upload gefaald: " + ex.Message);
        }

        // Genereer publieke URL (aangezien je bucket openbaar is gemaakt in je startup script)
        var publicUrl = $"https://storage.googleapis.com/{_bucketName}/uploads/{uniqueFileName}";

        return Ok(new { url = publicUrl });
    }
}
