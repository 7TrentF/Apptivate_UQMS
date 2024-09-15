namespace Apptivate_UQMS_WebApp.Services
{
    using Google.Cloud.Storage.V1;
    using System.IO;
    using System.Threading.Tasks;

    public class FileUploadService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public FileUploadService()
        {
            var firebaseConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "uqms-firebase-adminsdk.json");
            _storageClient = StorageClient.Create(Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(firebaseConfigPath));
            _bucketName = "uqms-a3d87.appspot.com"; // Replace with your bucket name
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

            using (var stream = file.OpenReadStream())
            {
                await _storageClient.UploadObjectAsync(
                    new Google.Apis.Storage.v1.Data.Object
                    {
                        Bucket = _bucketName,
                        Name = uniqueFileName
                    },
                    stream);

                var documentUrl = $"https://storage.googleapis.com/{_bucketName}/{uniqueFileName}";

                return documentUrl; // Return the URL instead of void
            }
        }
    }
}
