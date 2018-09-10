namespace CoreFireAPI.Models
{
    public class DaySchedule
    {
        public string Day { get; set; }
        public Timeslot[] Timeslots { get; set; }
    }
}
