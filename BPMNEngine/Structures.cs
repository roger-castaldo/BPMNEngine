using BPMNEngine.Elements;

namespace BPMNEngine
{
    /// <summary>
    /// This structure is used to house a File associated within a process instance.  It is used to both store, encode, decode and retreive File variables inside the process state.
    /// </summary>
    public readonly struct SFile 
    {
        /// <summary>
        /// The name of the File.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The extension of the File.
        /// </summary>
        public string Extension { get; init; }

        /// <summary>
        /// The content type tag for the File.  e.g. text/html
        /// </summary>
        public string ContentType { get; init; } = null;

        /// <summary>
        /// The binary content of the File.
        /// </summary>
        public byte[] Content { get; init; }

        internal SFile(DefinitionFile file)
        {
            Name=file.Name;
            Extension=file.Extension;
            ContentType = file.ContentType;
            Content = file.Content;
        }

        /// <summary>
        /// Compares the object to this
        /// </summary>
        /// <param name="obj">the object to compare</param>
        /// <returns>true if obj is an sFile and is equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is SFile fle)
            {
                return fle.Name.Equals(Name,StringComparison.InvariantCultureIgnoreCase) &&
                    fle.Extension.Equals(Extension, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(fle.ContentType??ContentType??String.Empty,ContentType??fle.ContentType??string.Empty,StringComparison.InvariantCultureIgnoreCase) &&
                    Content.Length==fle.Content.Length &&
                    !Content.Select((b, i) => new { val = b, index = i }).Any(o => fle.Content[o.index]!=o.val);
            }
            return false;
        }

        public override int GetHashCode()
            => $"{Name}.{Extension}[{ContentType}]:{Convert.ToBase64String(Content)}".GetHashCode();

        /// <summary>
        /// Compares left and right files
        /// </summary>
        /// <param name="left">left file for comparison</param>
        /// <param name="right">right file for comparison</param>
        /// <returns>true if are equal</returns>
        public static bool operator ==(SFile left, SFile right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares left and right files
        /// </summary>
        /// <param name="left">left file for comparison</param>
        /// <param name="right">right file for comparison</param>
        /// <returns>true if are not equal</returns>
        public static bool operator !=(SFile left, SFile right)
        {
            return !(left==right);
        }
    }

    /// <summary>
    /// This structure is used to specify a Process Runtime Constant.  These Constants are used as a Dynamic Constant, so a read only variable within the process that can be unique to the instance running, only a constant to that specific process instance.
    /// </summary>
    public readonly struct SProcessRuntimeConstant
    {
        /// <summary>
        /// The Name of the variable.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The Value of the variable.
        /// </summary>
        public object Value { get; init; }
    }
}
