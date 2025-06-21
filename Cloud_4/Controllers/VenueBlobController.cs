using Azure.Storage.Blobs;
using Cloud_4.Data;
using Cloud_4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Cloud_4.Controllers
{
    public class VenueBlobController : Controller
    {
        private readonly Cloud_4Context _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public VenueBlobController(Cloud_4Context context, IConfiguration config)
        {

            _context = context;
            _blobServiceClient = new BlobServiceClient(config
                ["AzureBlobStorage:ConnectionString"]);
            _containerName = config["AzureBlobStorage:ContainerName"];
        }
        public async Task<IActionResult> Index()
        {

            return View(await _context.VenueBlobs.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile imageFile, string venueName, string description)
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

            // Generate unique venue name if needed
            var existingVenues = _context.VenueBlobs
                .Where(v => v.VenueName != null && v.VenueName.StartsWith(venueName))
                .ToList();

            string uniqueName = existingVenues.Count > 0
                ? $"{venueName}{existingVenues.Count + 1}"
                : venueName;

            // Create and save venue
            var newVenue = new VenueBlob
            {
                VenueName = uniqueName,
                Description = description,
                ImageUrl = blobClient.Uri.ToString()
            };

            _context.VenueBlobs.Add(newVenue);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var venueToDelete = await _context.VenueBlobs.FindAsync(id);
            if (venueToDelete == null)
                return NotFound();

            // Delete image from Blob Storage if it's the only reference
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobUri = new Uri(venueToDelete.ImageUrl);
            var blobName = WebUtility.UrlDecode(blobUri.Segments.Last());
            var blobClient = containerClient.GetBlobClient(blobName);

            if (_context.VenueBlobs.Count(v => v.ImageUrl == venueToDelete.ImageUrl) <= 1)
            {
                await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
            }

            _context.VenueBlobs.Remove(venueToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}