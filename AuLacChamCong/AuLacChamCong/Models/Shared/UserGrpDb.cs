using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.Models.Shared
{
    public class UserGrpDb
    {
        [Key]
        public int UserGrpPrkID { get; set; }
        public string UserGrpID { get; set; }
        public string? UserGrpName { get; set; }
        public string? UserGrpNote { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsActive { get; set; }
        public string? XmlMenuAcept { get; set; }
    }
}
