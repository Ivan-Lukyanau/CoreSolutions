namespace CoreFireAPI.Models.Reservation
{
    public class UpdateTimeslotAvailability
    {
        public string MonthName { get; set; }
        public string MonthId { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public bool Availability { get; set; }
    }
}