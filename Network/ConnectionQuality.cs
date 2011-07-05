namespace Network
{
    public class ConnectionQuality
    {
        public ConnectionQuality(int downloadMbps, int uploadMbps)
        {
            DownloadMbps = downloadMbps;
            UploadMbps = uploadMbps;
        }

        protected ConnectionQuality()
        { }

        public int DownloadMbps { get; protected set; }

        public int UploadMbps { get; protected set; }
    }
}