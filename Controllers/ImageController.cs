using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Configuration;
using System.Diagnostics;

[EnableCors(origins: "*", headers: "*", methods: "*")]
[RoutePrefix("hello/image")]
public class ImageController : ApiController
{
    private string connectionString = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
    private string containerName = "images"; // The name of the container in your Azure Blob Storage

    [HttpPost]
    [Route("UploadImage")]
    public IHttpActionResult UploadImage()
    {
        var file = HttpContext.Current.Request.Files["file"];  // Retrieve file from FormData
        if (file == null || file.ContentLength == 0)
            return Content(HttpStatusCode.BadRequest, new { success = false, message = "No file uploaded." });

        try
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = blobContainerClient.GetBlobClient(uniqueFileName);

            using (var stream = file.InputStream)
            {
                blobClient.Upload(stream);
            }

            string fileUrl = blobClient.Uri.ToString();
            return Ok(new { success = true, imageUrl = fileUrl });
        }
        catch (Exception ex)
        {
            return InternalServerError(new Exception("Error uploading file: " + ex.Message));
        }
    }
}
