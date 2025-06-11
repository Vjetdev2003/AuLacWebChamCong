using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuLacChamCong.Services.Modules.ChamCong.Model.FaceId
{
    public class FaceId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? MngFaceIdPrkID { get; set; } // Khóa chính
        public decimal PsnPrkID { get; set; } 
        public string PsnName { get; set; }
        public string FaceIdValue { get; set; } 

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public byte[]? ImageData { get; set; }

    }
}
