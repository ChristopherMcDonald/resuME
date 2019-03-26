using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Resume.Controllers
{
    /// <summary>
    /// Local file class used to bring a IFormFile into local system, and delete it later
    /// </summary>
    public class LocalFile
    {
        public string LocalPath;

        public Stream Stream;

        /// <summary>
        /// Create the specified file.
        /// </summary>
        /// <returns>The create.</returns>
        /// <param name="file">File.</param>
        public static LocalFile Create(IFormFile file) {
            // get unique file name
            string newPath = Guid.NewGuid() + Path.GetExtension(file.FileName);
            LocalFile localFile = new LocalFile() { LocalPath = newPath, Stream = file.OpenReadStream() };
            return localFile;
        }
    }
}
