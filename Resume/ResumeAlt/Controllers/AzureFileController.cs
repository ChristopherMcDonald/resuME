using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

namespace Resume.Controllers
{
    public class AzureFileController
    {
        private readonly CloudStorageAccount cloud;

        /// <summary>
        /// The global storage key. Used for read access 
        /// </summary>
        private string GlobalStorageKey;


        /// <summary>
        /// Initializes a new instance of the <see cref="T:Resume.Controllers.AzureFileController"/> class.
        /// Used for interfacing with Azure File Storage
        /// </summary>
        /// <param name="connection">Connection string, provided by Azure</param>
        public AzureFileController(string connection, string read)
        {
            cloud = CloudStorageAccount.Parse(connection);
            GlobalStorageKey = read;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="type">What type of File is being retrieved</param>
        /// <param name="guid">The GUID for the File</param>
        public async Task<Stream> GetFileAsStream(FileType type, string guid)
        {
            CloudFileShare fileShare = cloud.CreateCloudFileClient().GetShareReference(type.ToString().ToLower());
            CloudFileDirectory root = fileShare.GetRootDirectoryReference();
            CloudFile file = root.GetFileReference(guid);
            Stream memstream = new MemoryStream();
            await file.DownloadToStreamAsync(memstream);
            return await file.OpenReadAsync();
        }

        public async Task<string> GetFile(FileType type, string fileName)
        {
            CloudFileShare fileShare = cloud.CreateCloudFileClient().GetShareReference(type.ToString().ToLower());
            CloudFileDirectory root = fileShare.GetRootDirectoryReference();
            CloudFile file = root.GetFileReference(Path.GetFileNameWithoutExtension(fileName));
            string path = $"/data/Resume/OutputDocument-{Guid.NewGuid()}.docx";
            await file.DownloadToFileAsync(path, FileMode.Create);
            return path;
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="type">Type of file being uploaded</param>
        /// <param name="path">File name to upload to</param>
        /// <param name="s">Stream to upload</param>
        public async Task UploadFile(FileType type, string path, Stream s)
        {
            CloudFileDirectory root = cloud.CreateCloudFileClient()
                                           .GetShareReference(type.ToString().ToLower())
                                           .GetRootDirectoryReference();

            CloudFile cloudFile = root.GetFileReference(Path.GetFileNameWithoutExtension(path));
            await cloudFile.UploadFromStreamAsync(s);
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <returns>The file</returns>
        /// <param name="type">Type of file being deleted</param>
        public async Task<bool> DeleteFile(FileType type, string guid) {
            try
            {
                CloudFileDirectory root = cloud.CreateCloudFileClient()
                                           .GetShareReference(type.ToString().ToLower())
                                           .GetRootDirectoryReference();

                CloudFile cloudFile = root.GetFileReference(Path.GetFileName(guid));
                if(cloudFile != null)
                {
                    await cloudFile.DeleteAsync();
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceWarning("Exception during deleting file: " + e.ToString());
            }

            return false;
        }

        /// <summary>
        /// Gets the Link the Public can use to display Preview Images
        /// </summary>
        /// <returns>The shareable image link.</returns>
        /// <param name="guid">GUID for the Image</param>
        public string GetShareableImageLink(string guid) {
            StorageUri uri = cloud.CreateCloudFileClient()
                                  .GetShareReference(FileType.Images.ToString().ToLower())
                                  .GetRootDirectoryReference()
                                  .GetFileReference(Path.GetFileNameWithoutExtension(guid))
                                  .StorageUri;
            string primary = uri.PrimaryUri.ToString() + GlobalStorageKey;
            WebRequest request = WebRequest.Create(primary);
            request.Method = "HEAD";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response != null && response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    return primary;
                }
            }

            return uri.SecondaryUri.ToString() + GlobalStorageKey;
        }
    }
}
