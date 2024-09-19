namespace Apptivate_UQMS_WebApp.Services
{
    using Google;
    using Google.Cloud.Storage.V1;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class FileUploadService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly ILogger<FileUploadService> _logger;

        public FileUploadService(ILogger<FileUploadService> logger)
        {
            _logger = logger;
            try
            {
                var firebaseConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "uqms-firebase-adminsdk.json");
                _storageClient = StorageClient.Create(Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(firebaseConfigPath));
                _bucketName = "uqms-a3d87.appspot.com"; // Replace with your bucket name
                _logger.LogInformation("Successfully initialized Google Cloud Storage client.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Google Cloud Storage client. Check the Firebase config path or credentials.");
                throw;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            try
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                _logger.LogInformation("Uploading file: {FileName} as {UniqueFileName}", file.FileName, uniqueFileName);

                using (var stream = file.OpenReadStream())
                {
                    await _storageClient.UploadObjectAsync(
                        new Google.Apis.Storage.v1.Data.Object
                        {
                            Bucket = _bucketName,
                            Name = uniqueFileName
                        },
                        stream);

                    _logger.LogInformation("File uploaded successfully: {UniqueFileName}", uniqueFileName);
                    return uniqueFileName;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading file: {FileName}", file.FileName);
                throw;
            }
        }


        public string GenerateSignedUrl(string objectName)
        {
            try
            {
                _logger.LogInformation("Attempting to generate signed URL for object: {ObjectName}", objectName);

                var urlSigner = UrlSigner.FromServiceAccountPath("Properties/uqms-firebase-adminsdk.json");
                string signedUrl = urlSigner.Sign(
                    _bucketName,
                    objectName,
                    TimeSpan.FromMinutes(15)
                );

                _logger.LogInformation("Successfully generated signed URL for object: {ObjectName}", objectName);
                return signedUrl;
            }
            catch (GoogleApiException ex) when (ex.Error.Code == 404)
            {
                _logger.LogError("File not found: {ObjectName}. Ensure the file exists in the bucket.", objectName);
                return null;  // Return null if the file does not exist
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating a signed URL for object: {ObjectName}", objectName);
                throw;
            }
        }

    }
}
