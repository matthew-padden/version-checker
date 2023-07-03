namespace VersionChecker.Api.Model
{
    public class VersionInfo<T> where T : IVersionDetail
    {
        public DateTime RefreshDateTime { get; set; }
        public List<T> Details { get; set; }
    }
}
