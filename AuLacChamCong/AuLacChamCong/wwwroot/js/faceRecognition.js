let stream = null;
let video = null;
let canvas = null;
let context = null;
let overlayCanvas = null;
let overlayContext = null;
let recognizedName = null;

export async function startWebcam() {
    try {
        video = document.getElementById('webcam');
        canvas = document.getElementById('canvas');
        context = canvas.getContext('2d');

        // Tạo overlay canvas để vẽ tên
        overlayCanvas = document.createElement('canvas');
        overlayCanvas.style.position = 'absolute';
        overlayCanvas.style.top = video.offsetTop + 'px';
        overlayCanvas.style.left = video.offsetLeft + 'px';
        video.parentNode.appendChild(overlayCanvas);
        overlayContext = overlayCanvas.getContext('2d');

        if (!video || !canvas || !context || !overlayCanvas || !overlayContext) {
            throw new Error("Không tìm thấy video hoặc canvas element!");
        }

        stream = await navigator.mediaDevices.getUserMedia({ video: true });
        video.srcObject = stream;
        await video.play();

        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        overlayCanvas.width = video.videoWidth;
        overlayCanvas.height = video.videoHeight;

        // Kiểm tra xem faceapi có sẵn không
        if (typeof faceapi === 'undefined') {
            throw new Error("face-api.js chưa được tải!");
        }

        // Tải mô hình từ CDN
        console.log("Đang tải mô hình face-api.js từ CDN...");
        await faceapi.nets.tinyFaceDetector.loadFromUri('https://cdn.jsdelivr.net/npm/@vladmandic/face-api/model');
        console.log("Mô hình face-api.js đã được tải từ CDN!");

        // Bắt đầu phát hiện khuôn mặt và vẽ tên
        detectFacesAndDrawName();

        console.log("Webcam đã được khởi động!");
    } catch (error) {
        console.error("Lỗi khi khởi động webcam:", error);
        throw error;
    }
}

export function captureFrame() {
    if (!video || !canvas || !context) {
        throw new Error("Webcam chưa được khởi động!");
    }

    try {
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        const dataUrl = canvas.toDataURL('image/jpeg', 0.8);
        return dataUrl.split(',')[1];
    } catch (error) {
        console.error("Lỗi khi chụp khung hình:", error);
        throw error;
    }
}

export function stopWebcam() {
    if (stream) {
        const tracks = stream.getTracks();
        tracks.forEach(track => track.stop());
        stream = null;
    }
    if (video) {
        video.srcObject = null;
    }
    if (overlayCanvas) {
        overlayCanvas.remove();
        overlayCanvas = null;
        overlayContext = null;
    }
    recognizedName = null;
    console.log("Webcam đã được dừng!");
}

export function setRecognizedName(name) {
    recognizedName = name;
    console.log("Đã nhận tên nhân viên:", recognizedName);
}

async function detectFacesAndDrawName() {
    if (!video || !overlayCanvas || !overlayContext) return;

    try {
        // Tăng độ chính xác phát hiện khuôn mặt
        const options = new faceapi.TinyFaceDetectorOptions({
            inputSize: 320, // Giảm kích thước đầu vào để tăng tốc độ
            scoreThreshold: 0.5 // Giảm ngưỡng để dễ phát hiện hơn
        });

        const detections = await faceapi.detectAllFaces(video, options);

        // Xóa canvas trước khi vẽ lại
        overlayContext.clearRect(0, 0, overlayCanvas.width, overlayCanvas.height);

        console.log(`Phát hiện được ${detections.length} khuôn mặt.`);

        if (detections.length === 0) {
            // Nếu không phát hiện được khuôn mặt, vẽ tên ở góc trên bên trái (vị trí mặc định)
            if (recognizedName) {
                overlayContext.font = '20px Arial';
                overlayContext.fillStyle = 'red';
                overlayContext.fillText(recognizedName, 10, 30);
                console.log("Không phát hiện được khuôn mặt, vẽ tên ở vị trí mặc định.");
            }
        } else {
            detections.forEach(detection => {
                const box = detection.box;
                console.log(`Vị trí khuôn mặt: x=${box.x}, y=${box.y}, width=${box.width}, height=${box.height}`);
                // Vẽ tên phía trên khuôn mặt
                if (recognizedName) {
                    overlayContext.font = '20px Arial';
                    overlayContext.fillStyle = 'red';
                    overlayContext.fillText(recognizedName, box.x, box.y - 10);
                    console.log(`Đã vẽ tên "${recognizedName}" phía trên khuôn mặt.`);
                }
                // Vẽ khung bao quanh khuôn mặt
                overlayContext.strokeStyle = 'red';
                overlayContext.lineWidth = 2;
                overlayContext.strokeRect(box.x, box.y, box.width, box.height);
            });
        }
    } catch (error) {
        console.error("Lỗi khi phát hiện khuôn mặt:", error);
    }

    // Tiếp tục phát hiện khuôn mặt
    requestAnimationFrame(detectFacesAndDrawName);
}