using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Resume.Controllers
{
    /// <summary>
    /// Uploaded File Stream class used to bring a IFormFile into local system, and delete it later
    /// </summary>
    public class UploadedFileStream : IDisposable
    {
        public static string TempPath => Path.GetTempPath();

        public string LocalPath;

        /// <summary>
        /// Create the specified file.
        /// </summary>
        /// <returns>The create.</returns>
        /// <param name="file">File.</param>
        public static async Task<LocalFile> Create(IFormFile file) {
            // get unique file name
            string newPath = TempPath + Guid.NewGuid() + Path.GetExtension(file.FileName);

            using (FileStream stream = new FileStream(newPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            LocalFile localFile = new LocalFile() { LocalPath = newPath };
            return localFile;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:Resume.Controllers.LocalFile"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:Resume.Controllers.LocalFile"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="T:Resume.Controllers.LocalFile"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:Resume.Controllers.LocalFile"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:Resume.Controllers.LocalFile"/> was occupying.</remarks>
        public void Dispose()
        {
            File.Delete(LocalPath);
        }
    }
}
