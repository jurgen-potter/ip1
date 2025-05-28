using Google;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("/api/[controller]")]
public class UploadsController : Controller
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName = "whimp24-bucket-public";

    public UploadsController(StorageClient storageClient)
    {
        _storageClient = storageClient;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        // Create unique filename to avoid collisions
        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var objectName = $"uploads/{uniqueFileName}";

        using var stream = file.OpenReadStream();
        
        try
        {
            await _storageClient.UploadObjectAsync(_bucketName, objectName, file.ContentType, stream);
        }
        catch (GoogleApiException ex)
        {
            return BadRequest("Upload failed: " + ex.Message);
        }

        // Return the public URL to the uploaded file
        var fileUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        return Ok(new { url = fileUrl });
    }
}