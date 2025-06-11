namespace AuLacChamCong.Models.Shared
{
    public class WorkDaysResult
    {
        public Dictionary<int, double> TotalHours { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public int TotalEmployees { get; set; }
    }
}
