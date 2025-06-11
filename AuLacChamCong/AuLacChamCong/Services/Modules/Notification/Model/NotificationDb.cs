namespace AuLacChamCong.Services.Modules.Notification.Model
{
    public class NotificationDb
    {
        public int NotificationID { get; set; }
        public int PsnPrkID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
    }
}
