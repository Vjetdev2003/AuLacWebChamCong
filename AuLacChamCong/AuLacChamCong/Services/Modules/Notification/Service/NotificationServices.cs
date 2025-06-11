using AuLacChamCong.DataApi;
using AuLacChamCong.Services.Modules.Notification.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuLacChamCong.Services.Modules.Notification.Service
{
    public class NotificationServices : INotificationServices
    {
        private readonly ApplicationDbContext _dbContext;
        public NotificationServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CreateNotificationAsync(NotificationDb notification)
        {
            try
            {
                if (notification.PsnPrkID <= 0 || string.IsNullOrEmpty(notification.Title))
                {
                    throw new ArgumentException("PsnPrkID and Title are required.");
                }

                notification.CreatedDate = DateTime.Now;
                notification.IsRead = false;

                _dbContext.NotificationDbs.Add(notification);
                var rows = await _dbContext.SaveChangesAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating notification: {ex.Message}");
                return false;
            }
        }

        public async Task<List<NotificationDb>> GetAllNotifications()
        {
                var request =  await _dbContext.NotificationDbs
                    .OrderByDescending(n => n.CreatedDate)
                    .ToListAsync();
            return request;
        }
        public async Task<List<NotificationDb>> GetNotificationsByPsnPrkIDAsync(int psnPrkID)
        {
            try
            {
                return await _dbContext.NotificationDbs
                    .Where(n => n.PsnPrkID == psnPrkID)
                    .OrderByDescending(n => n.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications for PsnPrkID {psnPrkID}: {ex.Message}");
                return new List<NotificationDb>();
            }
        }

        public async Task<bool> MarkAsReadAsync(int notificationID)
        {
            try
            {
                var notification = await _dbContext.NotificationDbs
                    .FirstOrDefaultAsync(n => n.NotificationID == notificationID);
                if (notification == null) return false;

                notification.IsRead = true;
                var rows = await _dbContext.SaveChangesAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking notification as read: {ex.Message}");
                return false;
            }
        }
    }
}
