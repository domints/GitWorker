namespace GitWorker.Download
{
    public class DlObject
    {
        public string Path { get; set; }
        public bool IsDownloaded { get; set; }
        public int DlResultCode { get; set; }
    }
}