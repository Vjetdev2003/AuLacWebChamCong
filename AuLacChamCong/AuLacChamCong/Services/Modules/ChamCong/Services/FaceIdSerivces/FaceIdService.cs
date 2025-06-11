using AuLacChamCong.DataApi;
using AuLacChamCong.Services.Modules.ChamCong.Model.FaceId;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using OpenCvSharp;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;

namespace AuLacChamCong.Services.Modules.ChamCong.Services.FaceIdSerivces
{
    public class FaceIdService : IFaceIdService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _cascadePath;
        private readonly string _faceImagesPath;

        public FaceIdService(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _cascadePath = Path.Combine(environment.ContentRootPath, "Cascades", "haarcascade_frontalface_default.xml");
            _faceImagesPath = Path.Combine(environment.ContentRootPath, "wwwroot", "FaceImages");

            if (!File.Exists(_cascadePath))
            {
                throw new FileNotFoundException($"Không tìm thấy file Haar Cascade tại: {_cascadePath}. Vui lòng kiểm tra thư mục Cascades trong dự án.");
            }

            // Tạo thư mục FaceImages nếu chưa tồn tại
            try
            {
                if (!Directory.Exists(_faceImagesPath))
                {
                    Directory.CreateDirectory(_faceImagesPath);
                    Console.WriteLine($"[INFO] Đã tạo thư mục FaceImages tại: {_faceImagesPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi tạo thư mục FaceImages: {ex.Message}");
                throw;
            }
        }

        public Task<bool> DetectFace(byte[] faceImage)
        {
            try
            {
                if (faceImage == null || faceImage.Length == 0)
                {
                    Console.WriteLine("[ERROR] Dữ liệu hình ảnh không hợp lệ!");
                    return Task.FromResult(false);
                }

                using (var originalMat = Cv2.ImDecode(faceImage, ImreadModes.Color))
                {
                    if (originalMat.Empty())
                    {
                        Console.WriteLine("[ERROR] Không thể tải hình ảnh!");
                        return Task.FromResult(false);
                    }

                    Console.WriteLine($"[INFO] Kích thước hình ảnh gốc: {originalMat.Width}x{originalMat.Height}");

                    Mat matToUse;
                    double scale = 1.0;
                    if (originalMat.Width > 640 || originalMat.Height > 640)
                    {
                        scale = Math.Min(640.0 / originalMat.Width, 640.0 / originalMat.Height);
                        int newWidth = (int)(originalMat.Width * scale);
                        int newHeight = (int)(originalMat.Height * scale);
                        matToUse = new Mat();
                        Cv2.Resize(originalMat, matToUse, new OpenCvSharp.Size(newWidth, newHeight));
                        Console.WriteLine($"[INFO] Đã resize hình ảnh thành: {matToUse.Width}x{matToUse.Height}");
                    }
                    else
                    {
                        matToUse = originalMat;
                    }

                    using (var gray = new Mat())
                    {
                        Cv2.CvtColor(matToUse, gray, ColorConversionCodes.BGR2GRAY);

                        using (var faceCascade = new CascadeClassifier(_cascadePath))
                        {
                            var faces = faceCascade.DetectMultiScale(
                                gray,
                                scaleFactor: 1.05,
                                minNeighbors: 1,
                                flags: HaarDetectionTypes.ScaleImage,
                                minSize: new OpenCvSharp.Size(20, 20)
                            );

                            if (faces.Length == 0)
                            {
                                Console.WriteLine("[INFO] Không phát hiện được khuôn mặt trong hình ảnh!");
                                return Task.FromResult(false);
                            }

                            Console.WriteLine($"[INFO] Phát hiện được {faces.Length} khuôn mặt trong hình ảnh!");
                            return Task.FromResult(true);
                        }
                    }

                    if (matToUse != originalMat)
                    {
                        matToUse.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi phát hiện khuôn mặt: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public async Task<string> RegisterFace(decimal psnPrkID, List<string> encodedImages)
        {
            try
            {
                Console.WriteLine($"[INFO] Bắt đầu đăng ký khuôn mặt cho PsnPrkID: {psnPrkID}");

                if (psnPrkID <= 0)
                {
                    Console.WriteLine("[ERROR] ID người dùng không hợp lệ!");
                    return "ID người dùng không hợp lệ!";
                }

                if (encodedImages == null || encodedImages.Count != 20 || encodedImages.Any(string.IsNullOrEmpty))
                {
                    Console.WriteLine("[ERROR] Danh sách hình ảnh không hợp lệ!");
                    return "Danh sách hình ảnh không hợp lệ! Cần đúng 20 hình ảnh.";
                }

                Console.WriteLine("[INFO] Đang kiểm tra thông tin nhân viên...");
                var user = await _dbContext.Personnels.FirstOrDefaultAsync(u => u.PsnPrkID == psnPrkID);
                if (user == null)
                {
                    Console.WriteLine($"[ERROR] Không tìm thấy nhân viên với ID: {psnPrkID}");
                    return "Không tìm thấy nhân viên với ID này!";
                }
                var name = user.PsnName;

                Console.WriteLine("[INFO] Đang kiểm tra khuôn mặt trong hình ảnh...");
                for (int i = 0; i < encodedImages.Count; i++)
                {
                    byte[] faceImage;
                    try
                    {
                        faceImage = Convert.FromBase64String(encodedImages[i]);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"[ERROR] Hình ảnh thứ {i + 1} có chuỗi base64 không hợp lệ: {ex.Message}");
                        return $"Hình ảnh thứ {i + 1} có chuỗi base64 không hợp lệ!";
                    }

                    bool faceDetected = await DetectFace(faceImage);
                    if (!faceDetected)
                    {
                        Console.WriteLine($"[ERROR] Hình ảnh thứ {i + 1} không chứa khuôn mặt!");
                        return $"Hình ảnh thứ {i + 1} không chứa khuôn mặt!";
                    }
                }

                Console.WriteLine("[INFO] Đang tạo thư mục lưu trữ hình ảnh...");
                string userFolder = Path.Combine(_faceImagesPath, psnPrkID.ToString());
                try
                {
                    if (Directory.Exists(userFolder))
                    {
                        Directory.Delete(userFolder, true);
                        Console.WriteLine($"[INFO] Đã xóa thư mục cũ tại: {userFolder}");
                    }
                    Directory.CreateDirectory(userFolder);
                    Console.WriteLine($"[INFO] Đã tạo thư mục tại: {userFolder}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Lỗi khi tạo thư mục lưu trữ: {ex.Message}");
                    return $"Lỗi khi tạo thư mục lưu trữ: {ex.Message}";
                }

                Console.WriteLine("[INFO] Đang lưu hình ảnh vào thư mục...");
                for (int i = 0; i < encodedImages.Count; i++)
                {
                    try
                    {
                        byte[] faceImage = Convert.FromBase64String(encodedImages[i]);
                        string imagePath = Path.Combine(userFolder, $"image_{i + 1}.jpg");
                        await File.WriteAllBytesAsync(imagePath, faceImage);
                        Console.WriteLine($"[INFO] Đã lưu hình ảnh thứ {i + 1} tại: {imagePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Lỗi khi lưu hình ảnh thứ {i + 1}: {ex.Message}");
                        return $"Lỗi khi lưu hình ảnh thứ {i + 1}: {ex.Message}";
                    }
                }

                Console.WriteLine("[INFO] Đang lưu thông tin vào cơ sở dữ liệu...");
                string faceId = Guid.NewGuid().ToString();
                var existingFaceId = await _dbContext.FaceIds.FirstOrDefaultAsync(f => f.PsnPrkID == psnPrkID);
                if (existingFaceId != null)
                {
                    existingFaceId.FaceIdValue = faceId;
                    existingFaceId.PsnName = name;
                    existingFaceId.ImageData = null;
                    existingFaceId.UpdatedAt = DateTime.Now;
                    _dbContext.FaceIds.Update(existingFaceId);
                }
                else
                {
                    var newFaceId = new FaceId
                    {
                        PsnPrkID = psnPrkID,
                        PsnName = name,
                        FaceIdValue = faceId,
                        ImageData = null,
                        CreatedAt = DateTime.Now
                    };
                    _dbContext.FaceIds.Add(newFaceId);
                }

                await _dbContext.SaveChangesAsync();
                Console.WriteLine("[INFO] Đăng ký khuôn mặt thành công!");
                return "Đăng ký khuôn mặt thành công!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi đăng ký khuôn mặt cho PsnPrkID {psnPrkID}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[INNER EXCEPTION] {ex.InnerException.Message}");
                }
                return $"Lỗi khi đăng ký khuôn mặt: {ex.Message}";
            }
        }
        public async Task<bool> VerifyFace(decimal psnPrkID, byte[] faceImage)
        {
            try
            {
                if (faceImage == null || faceImage.Length == 0)
                {
                    Console.WriteLine("[ERROR] Dữ liệu hình ảnh không hợp lệ!");
                    return false;
                }

                const int minImageSizeBytes = 1024;
                if (faceImage.Length < minImageSizeBytes)
                {
                    Console.WriteLine("[ERROR] Hình ảnh quá nhỏ, không thể chứa khuôn mặt!");
                    return false;
                }

                bool hasFace = await DetectFace(faceImage);
                if (!hasFace)
                {
                    Console.WriteLine("[INFO] Không tìm thấy khuôn mặt trong hình ảnh!");
                    return false;
                }

                string userFolder = Path.Combine(_faceImagesPath, psnPrkID.ToString("0")); // Bỏ phần thập phân
                if (!Directory.Exists(userFolder))
                {
                    Console.WriteLine($"[ERROR] Không tìm thấy thư mục hình ảnh cho userId: {psnPrkID}");
                    return false;
                }

                var imageFiles = Directory.GetFiles(userFolder, "*.jpg");
                if (imageFiles.Length == 0)
                {
                    Console.WriteLine($"[ERROR] Không tìm thấy hình ảnh nào trong thư mục của userId: {psnPrkID}");
                    return false;
                }

                using var inputMat = Cv2.ImDecode(faceImage, ImreadModes.Color);
                using var inputGray = new Mat();
                Cv2.CvtColor(inputMat, inputGray, ColorConversionCodes.BGR2GRAY);

                using var faceCascade = new CascadeClassifier(_cascadePath);
                var inputFaces = faceCascade.DetectMultiScale(inputGray, 1.05, 1, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(20, 20));
                if (inputFaces.Length == 0)
                {
                    return false;
                }

                using var inputFaceRegion = new Mat(inputGray, inputFaces[0]);
                using var resizedInputFace = new Mat();
                Cv2.Resize(inputFaceRegion, resizedInputFace, new OpenCvSharp.Size(100, 100));

                foreach (var imagePath in imageFiles)
                {
                    byte[] storedImage = await File.ReadAllBytesAsync(imagePath);
                    using var storedMat = Cv2.ImDecode(storedImage, ImreadModes.Color);
                    if (storedMat.Empty())
                    {
                        continue;
                    }

                    using var storedGray = new Mat();
                    Cv2.CvtColor(storedMat, storedGray, ColorConversionCodes.BGR2GRAY);

                    var storedFaces = faceCascade.DetectMultiScale(storedGray, 1.05, 1, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(20, 20));
                    if (storedFaces.Length == 0)
                    {
                        continue;
                    }

                    using var storedFaceRegion = new Mat(storedGray, storedFaces[0]);
                    using var resizedStoredFace = new Mat();
                    Cv2.Resize(storedFaceRegion, resizedStoredFace, new OpenCvSharp.Size(100, 100));

                    using var diff = new Mat();
                    Cv2.Absdiff(resizedInputFace, resizedStoredFace, diff);
                    Scalar meanDiff = Cv2.Mean(diff);

                    if (meanDiff.Val0 < 30)
                    {
                        Console.WriteLine($"[INFO] Xác nhận thành công cho userId: {psnPrkID}");
                        return true;
                    }
                }

                Console.WriteLine($"[INFO] Không nhận diện được khuôn mặt khớp với userId: {psnPrkID}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi xác minh khuôn mặt: {ex.Message}");
                return false;
            }
        }

        public async Task<List<FaceId>> GetAllFaceId()
        {
            try
            {
                var faceIds = await _dbContext.FaceIds.ToListAsync();
                return faceIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lấy danh sách FaceIds: {ex.Message}");
                throw;
            }
        }

        
    }
}