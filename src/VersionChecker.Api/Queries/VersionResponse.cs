namespace VersionChecker.Api.Queries
{
    public class VersionResponse
    {
        public VersionResponse(bool isInSupport)
        {
            IsInSupport = isInSupport;
        }

        public bool IsInSupport { get; private set; }
    }
}
