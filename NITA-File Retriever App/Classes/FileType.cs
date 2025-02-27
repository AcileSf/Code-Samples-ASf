namespace PO_FileRetrieverUI.Classes
{
    internal class FileType
    {
        /// <summary>
        /// Represents the description that is to appear in the CheckBoxes
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Represents the possible extensions of the files in question
        /// </summary>
        public string[] Extension { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">Description that is to appear in the UI</param>
        /// <param name="extension">String containing a comma separated list of possible associated file extensions, or one extension if it's unique and doesn't have alternatives</param>
        public FileType(string description, string extension)
        {
            Description = description;
            Extension = extension.Split(',');
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">Description that is to appear in the UI</param>
        /// <param name="extension">String Array representing the possible file extensions</param>
        public FileType(string description, string[] extension)
        {
            Description = description;
            Extension = extension;
        }


    }

}
