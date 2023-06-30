namespace VersionChecker.Api.Model
{
    /// <summary>
    /// Represents a version of a .NET runtime.
    /// </summary>
    public class VersionInfo
    {
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime? EndOfSupportDate { get; set; }
        public bool IsInSupport => DateTime.UtcNow < EndOfSupportDate;
    }
}
