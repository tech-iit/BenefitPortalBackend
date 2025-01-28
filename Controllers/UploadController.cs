using System.Web.Http;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.Http;  // For image format detection

namespace BenefitPortalServices.Controllers
{
    public class UploadController : ApiController
    {
        private readonly string connectionString;

        public UploadController()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }


        [HttpPost]
        [Route("api/image/upload")]
        public IHttpActionResult UploadImage()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count == 0)
                return BadRequest("No file uploaded.");

            var file = httpRequest.Files[0];
            if (file == null || file.ContentLength <= 0)
                return BadRequest("Invalid file.");

            var fileName = Path.GetFileName(file.FileName);
            var contentType = file.ContentType;
            byte[] fileData;

            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Images (FileName, ContentType, Data) VALUES (@FileName, @ContentType, @Data)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FileName", fileName);
                    command.Parameters.AddWithValue("@ContentType", contentType);
                    command.Parameters.AddWithValue("@Data", fileData);
                    command.ExecuteNonQuery();
                }
            }

            return Ok("Image uploaded successfully.");
        }

        [HttpGet]
        [Route("api/image/{id}")]
        public IHttpActionResult GetImage(int id)
        {
            byte[] fileData;
            string fileName, contentType;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT FileName, ContentType, Data FROM Images WHERE Id = @Id";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                return NotFound();

                            fileName = reader["FileName"].ToString();
                            contentType = reader["ContentType"].ToString();
                            fileData = (byte[])reader["Data"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception (not shown here for brevity)
                return InternalServerError(ex);
            }

            var result = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileData)
            };
            //result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            //result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            //{
            //    FileName = fileName
            //};

            return ResponseMessage(result);
        }
    }
    }

