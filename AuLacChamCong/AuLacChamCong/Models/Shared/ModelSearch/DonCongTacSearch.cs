namespace AuLacChamCong.Models.Shared.ModelSearch
{
    public class DonCongTacSearch
    {
        public int MngDonCongTacPrkID { get; set; }
        public int PsnPrkID { get; set; }
        public DateTime NgayCongTac { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string NoiCongTac { get; set; }
        public string LyDo { get; set; }
        public int? Status { get; set; }
        public string? StatusDescription { get; set; }
        public string PersonnelsName { get; set; }
    }
}
