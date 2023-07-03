namespace VersionChecker.Api.Model
{
    public class VersionInfo
    {
        public List<VersionDetail> Details { get; set; }
    }

    public class VersionDetail
    {

        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime? EndOfSupportDate { get; set; }
        public bool IsInSupport => DateTime.UtcNow < EndOfSupportDate;
    }
}
