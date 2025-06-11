using AuLacChamCong.Services.Modules.Notification.Model;

namespace AuLacChamCong.Services.Modules.Notification.Service
{
    public interface INotificationServices
    {
        Task<List<NotificationDb>> GetAllNotifications();
        Task<bool> CreateNotificationAsync(NotificationDb notification);
        Task<List<NotificationDb>> GetNotificationsByPsnPrkIDAsync(int psnPrkID);
        Task<bool> MarkAsReadAsync(int notificationID);

    }
}
