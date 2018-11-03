﻿using System;
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
        private readonly string GlobalStorageKey = "?sv=2017-11-09&ss=f&srt=sco&sp=r&se=9999-11-04T04:37:37Z&st=2018-11-03T19:37:37Z&spr=https&sig=4SXBqGuFDzsDTmu1uVVLXllghIhYUNxA0QeuV7Eg9xc%3D";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Resume.Controllers.AzureFileController"/> class.
        /// Used for interfacing with Azure File Storage
        /// </summary>
        /// <param name="connection">Connection string, provided by Azure</param>
        public AzureFileController(string connection)
        {
            cloud = CloudStorageAccount.Parse(connection);
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

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="type">What type of File is being retrieved</param>
        /// <param name="path">Where the file is located locally</param>
        public async Task UploadFile(FileType type, string path) {
            // TODO shrink file
            // TODO catch exceptions, log failure and return false?
            CloudFileDirectory root = cloud.CreateCloudFileClient()
                                           .GetShareReference(type.ToString().ToLower())
                                           .GetRootDirectoryReference();

            CloudFile cloudFile = root.GetFileReference(Path.GetFileName(path));
            using (FileStream fs = new FileInfo(path).OpenRead())
            {
                await cloudFile.UploadFromStreamAsync(fs);
            }
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <returns>The file</returns>
        /// <param name="type">Type of file being deleted</param>
        public async Task DeleteFile(FileType type, string guid) {
            // TODO catch exceptions, log failure and return false?
            CloudFileDirectory root = cloud.CreateCloudFileClient()
                                           .GetShareReference(type.ToString().ToLower())
                                           .GetRootDirectoryReference();

            CloudFile cloudFile = root.GetFileReference(Path.GetFileName(guid));
            await cloudFile.DeleteAsync();
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
                                  .GetFileReference(guid)
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