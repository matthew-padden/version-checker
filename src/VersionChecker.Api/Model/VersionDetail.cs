namespace VersionChecker.Api.Model
{
    public interface IVersionDetail
    {
        string Version { get; set; }
        DateTime ReleaseDate { get; set; }
        DateTime? EndOfSupportDate { get; set; }
        Dictionary<string, string> AdditionalProperties { get; set; }
    }

    public abstract class VersionDetail : IVersionDetail
    {
        public VersionDetail()
        {
            AdditionalProperties = new Dictionary<string, string>();
        }

        public string Version { get; set; }

        public DateTime ReleaseDate { get; set; }

        public DateTime? EndOfSupportDate { get; set; }

        public bool IsInSupport => this.EndOfSupportDate is null || DateTime.UtcNow < this.EndOfSupportDate;

        public Dictionary<string, string> AdditionalProperties { get; set; }
    }
}
