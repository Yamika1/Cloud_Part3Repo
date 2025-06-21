using Azure.Storage.Blobs;
using Cloud_4.Data;
using Cloud_4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Cloud_4.Controllers
{
    public class EventBlobController : Controller
    {
        private readonly Cloud_4Context _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public EventBlobController(Cloud_4Context context, IConfiguration config)
        {

            _context = context;
            _blobServiceClient = new BlobServiceClient(config
                ["AzureBlobStorage:ConnectionString"]);
            _containerName = config["AzureBlobStorage:ContainerName"];
        }
        public async Task<IActionResult> Index()
        {

            return View(await _context.EventBlobs.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile imageFile, string name, string description)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No image uploaded.");

            // Get container and blob client
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(imageFile.FileName);

            // Upload image
            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            // Generate unique event name
            var existingEvents = _context.EventBlobs
                .Where(e => e.EventName != null && e.EventName.StartsWith(name))
                .ToList();

            string uniqueName = existingEvents.Count > 0
                ? $"{name}{existingEvents.Count + 1}"
                : name;

            // Create and save event
            var newEvent = new EventBlob
            {
                EventName = uniqueName,
                Description = description,
                ImageUrl = blobClient.Uri.ToString()
            };

            _context.EventBlobs.Add(newEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var eventToDelete = await _context.EventBlobs.FindAsync(id);
            if (eventToDelete == null)
                return NotFound();

            // Delete image from Blob Storage if it's the only reference
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobUri = new Uri(eventToDelete.ImageUrl);
            var blobName = WebUtility.UrlDecode(blobUri.Segments.Last());
            var blobClient = containerClient.GetBlobClient(blobName);

            if (_context.EventBlobs.Count(e => e.ImageUrl == eventToDelete.ImageUrl) <= 1)
            {
                await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
            }

            _context.EventBlobs.Remove(eventToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}