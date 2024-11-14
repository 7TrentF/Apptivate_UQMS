namespace Apptivate_UQMS_WebApp.Models
{
    public class LogsModel
    {

        // Log Entry class
        public class LogEntry
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public DateTime LastModified { get; set; }
        }

    }
}
