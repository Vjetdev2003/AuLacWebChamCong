
using AuLacChamCong.DataApi;
using AuLacChamCong.Models.Shared;
using AuLacChamCong.Models.Shared.ModelSearch;
using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.DonCongTac.Model;
using AuLacChamCong.Services.Modules.Email;
using AuLacChamCong.Services.Modules.Notification.Model;
using AuLacChamCong.Services.Modules.Notification.Service;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace AuLacChamCong.Services.Modules.DonCongTac.Service
{

    public class DonCongTacServices : IDonCongTacServices
    {
        private readonly ILogger<DonCongTacServices> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly SendMessage _sendMessage;
        private readonly INotificationServices _notificationServices;
        public DonCongTacServices(ILogger<DonCongTacServices> logger, SendMessage sendMessage, ApplicationDbContext dbContext, IConfiguration configuration, INotificationServices notificationServices)
        {
            _logger = logger;
            _dbContext = dbContext;
            _configuration = configuration;
            _sendMessage = sendMessage;
            _notificationServices = notificationServices;
        }
        public async Task<bool> CreateDonCongTacAsync(DonCongTacViewModel request)
        {
            try
            {
                var donCongTac = new DonCongTacDb
                {
                    PsnPrkID = request.PsnPrkID,
                    NoiCongTac = request.NoiCongTac,
                    NgayCongTac = request.NgayCongTac,
                    ThoiGianBatDau = request.ThoiGianBatDau,
                    ThoiGianKetThuc = request.ThoiGianKetThuc,
                    LyDo = request.LyDo,
                    Status = Constants.Pending,
                    StatusDescription = "Chờ xác nhận",
                    HospitalId = request.HospitalId
                };
                if (donCongTac == null)
                {
                    throw new ArgumentException("Đơn công tác không được để trống.");
                }
                Console.WriteLine(donCongTac.StatusDescription);
                // Log thông tin đơn công tác
                _logger.LogInformation($"[INFO] Nhận đơn công tác ngày {donCongTac.NgayCongTac}: {donCongTac.LyDo}");

                await _dbContext.DonCongTacs.AddAsync(donCongTac);
                await _dbContext.SaveChangesAsync();

                // Lấy thông tin nhân viên theo PsnPrkID
                var psn = await _dbContext.Personnels
                    .Where(p => p.PsnPrkID == donCongTac.PsnPrkID)
                    .FirstOrDefaultAsync();

                var psnType = await _dbContext.PersonnelsTypes
                    .Where(p => p.PsnTypeID == Int32.Parse(psn.PsnTypeID))
                    .FirstOrDefaultAsync();

                if (psn == null)
                {
                    _logger.LogWarning($"[WARNING] Không tìm thấy thông tin nhân viên với PsnPrkID = {donCongTac.PsnPrkID}");
                    return false; // Không gửi email nếu không có thông tin nhân viên
                }

                // Email của admin hoặc người nhận xác nhận đơn công tác
                string toEmail = "viettrun2@gmail.com";

                string subject = $"[Xác nhận] Đơn công tác của nhân viên {psn.PsnName} (ID: {psn.PsnPrkID})";

                string content = $@"
                        <h1>Thông tin đơn công tác</h1>
                        <p><b>Nhân viên:</b> {psn.PsnName}</p>
                        <p><b>Chức vụ:</b> {psnType.PsnTypeName}</p>
                        <p><b>Nơi Công Tác:</b> {donCongTac.NoiCongTac}</p>
                        <p><b>Ngày công tác:</b> {donCongTac.NgayCongTac:dd/MM/yyyy}</p>
                        <p><b>Lý do:</b> {donCongTac.LyDo}</p>
                        <p>Vui lòng kiểm tra và phản hồi nếu cần.</p>
                    ";

                // Gửi email qua MailKit
                string emailResult = await _sendMessage.SendEmailToAdmin(toEmail, subject, content);
                bool emailSent = emailResult == "Email sent successfully";

                if (!emailSent)
                {
                    _logger.LogWarning($"[WARNING] Gửi email thất bại: {emailResult}");
                }

                return emailSent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] Lỗi khi tạo đơn công tác: {ex.Message}");
                return false;
            }
        }

        public async Task<List<DonCongTacDb>> GetAllDonCongTacAsync()
        {
            try
            {
                // Lấy tất cả đơn công tác từ cơ sở dữ liệu
                var donCongTacList = await _dbContext.DonCongTacs
                    .Where(d => d.Status == 0)
                    .Include(d => d.Personnels)
                    .OrderByDescending(d => d.NgayCongTac)
                    .ToListAsync();

                return donCongTacList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] Lỗi khi lấy danh sách đơn công tác: {ex.Message}");
                return new List<DonCongTacDb>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }

        public async Task<List<DonCongTacDb>> GetHistoryDonCongTac()
        {
            try
            {
                var donCongTacHistory = await _dbContext.DonCongTacs.Where(d => d.Status == 1 || d.Status == -1)
                    .Include(d => d.Personnels)
                    .OrderByDescending(d => d.NgayCongTac)
                    .ToListAsync();
                return donCongTacHistory;

            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] Lỗi khi lấy danh sách đơn công tác: {ex.Message}");
                return new List<DonCongTacDb>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }

        public async Task<bool> UpdateDonCongTacStatusAsync(int mngDonCongTacPrkID, int status)
        {
            try
            {
                var existingDonCongTac = await _dbContext.DonCongTacs
                    .FirstOrDefaultAsync(d => d.MngDonCongTacPrkID == mngDonCongTacPrkID);
                await _notificationServices.CreateNotificationAsync(new NotificationDb
                {
                    PsnPrkID = existingDonCongTac.PsnPrkID,
                    Title = "Cập nhật trạng thái đơn công tác",
                    Message = $"Đơn công tác của bạn {existingDonCongTac.StatusDescription} vui lòng kiểm tra email để xem chi tiết"
                });
                if (existingDonCongTac == null)
                {
                    _logger.LogWarning($"[WARNING] Đơn công tác với MngDonCongTacPrkID {mngDonCongTacPrkID} không tồn tại.");
                    return false;
                }

                existingDonCongTac.Status = status;
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] Lỗi khi cập nhật trạng thái đơn công tác: {ex.Message}");
                return false;
            }
        }

        public async Task<List<DonCongTacViewModel>> GetHistoryDonCongTacByPsnPrkIDAsync(int psnPrkID)
        {
            try
            {
                // Lấy lịch sử đơn công tác theo PsnPrkID
                var donCongTacHistory = await _dbContext.DonCongTacs
                    .Where(d => (d.Status == 1 || d.Status == -1) && d.PsnPrkID == psnPrkID)
                    .OrderByDescending(d => d.NgayCongTac)
                    .Select(d => new DonCongTacViewModel
                    {
                        MngDonCongTacPrkID = d.MngDonCongTacPrkID,
                        PsnPrkID = d.PsnPrkID,
                        NgayCongTac = d.NgayCongTac,
                        ThoiGianBatDau = d.ThoiGianBatDau,
                        ThoiGianKetThuc = d.ThoiGianKetThuc,
                        NoiCongTac = d.NoiCongTac,
                        LyDo = d.LyDo,
                        Status = d.Status ?? 0, // Giả sử Status có thể null, mặc định là 0 nếu null
                        StatusDescription = d.StatusDescription
                    })
                    .ToListAsync();

                return donCongTacHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] Lỗi khi lấy lịch sử đơn công tác cho PsnPrkID {psnPrkID}: {ex.Message}");
                return new List<DonCongTacViewModel>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }

        public async Task<List<DonCongTacSearch>> SearchDonCongTacAsync(string searchKeyword, int? status, string? noiCongTac)
        {
            var query = _dbContext.DonCongTacs.AsQueryable();

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query = query.Where(d => d.Personnels.PsnName.Contains(searchKeyword));
            }
            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status);
            }
            if (!string.IsNullOrEmpty(noiCongTac))
            {
                query = query.Where(d => d.NoiCongTac.Contains(noiCongTac));
            }

            // Project to DonCongTacViewModel
            var result = await query
                .Include(d => d.Personnels)
                .Select(d => new DonCongTacSearch
                {
                    MngDonCongTacPrkID = d.MngDonCongTacPrkID,
                    PsnPrkID = d.PsnPrkID,
                    NgayCongTac = d.NgayCongTac,
                    ThoiGianBatDau = d.ThoiGianBatDau,
                    ThoiGianKetThuc = d.ThoiGianKetThuc,
                    NoiCongTac = d.NoiCongTac,
                    LyDo = d.LyDo,
                    Status = d.Status,
                    StatusDescription = d.StatusDescription,
                    PersonnelsName = d.Personnels.PsnName
                    // Add more properties from DonCongTacDb as needed
                })
                .ToListAsync();

            return result;
        }

        public async Task<bool> SaveDonCongTacAsync(DonCongTacViewModel donCongTac)
        {
            try
            {
                // Map the DonCongTacViewModel to the DonCongTac entity
                var entity = new DonCongTacDb
                {
                    MngDonCongTacPrkID = donCongTac.MngDonCongTacPrkID, // Should be 0 for new records (auto-incremented by the database)
                    PsnPrkID = donCongTac.PsnPrkID,
                    NgayCongTac = donCongTac.NgayCongTac,
                    ThoiGianBatDau = donCongTac.ThoiGianBatDau,
                    ThoiGianKetThuc = donCongTac.ThoiGianKetThuc,
                    NoiCongTac = donCongTac.NoiCongTac,
                    LyDo = donCongTac.LyDo,
                    Status = donCongTac.Status,
                    StatusDescription = donCongTac.StatusDescription,
                    HospitalId = donCongTac.HospitalId
                };

                // Add the entity to the DbSet
                _dbContext.DonCongTacs.Add(entity);

                // Save changes to the database
                int rowsAffected = await _dbContext.SaveChangesAsync();

                // Return true if the save was successful (at least one row was affected)
                if (rowsAffected > 0)
                {
                    Console.WriteLine("[INFO] Successfully saved DonCongTac to the database.");
                    return true;
                }
                else
                {
                    Console.WriteLine("[ERROR] No rows were affected. Failed to save DonCongTac.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error saving DonCongTac: {ex.Message}");
                return false;
            }
        }
    }
}
