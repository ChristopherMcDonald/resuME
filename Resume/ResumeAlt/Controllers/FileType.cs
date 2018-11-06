using System;

namespace Resume.Controllers
{
    /// <summary>
    /// Describes the types of files that can be uploaded, toString will point to Folder on Azure File Storage
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// Templates for Resumes
        /// </summary>
        Templates,

        /// <summary>
        /// Images that show what the template looks like filled out
        /// </summary>
        Images,

        /// <summary>
        /// The generated templates
        /// </summary>
        Generated
    }
}
