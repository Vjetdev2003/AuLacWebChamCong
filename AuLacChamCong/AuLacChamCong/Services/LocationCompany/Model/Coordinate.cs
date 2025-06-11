namespace AuLacChamCong.Services.LocationCompany.Model
{
    public class Coordinate
    {
        private const double HospitalLatitude = 16.4578597;
        private const double HospitalLongitude = 107.6117389;
        //private const double HospitalLatitude = 18.4578597;
        // private const double HospitalLongitude = 108.6117389;
        public double DistanceToHeadquarter { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public void CalculateDistance()
        {
            DistanceToHeadquarter = Haversine(Latitude, Longitude, HospitalLatitude, HospitalLongitude);
        }
        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Bán kính Trái Đất (m)
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
