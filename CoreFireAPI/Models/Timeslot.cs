namespace CoreFireAPI.Models
{
    public class Timeslot
    {
        public int Time { get; set; }
        public bool Available { get; set; }

        public Timeslot(int time, bool available)
        {
            Available = available;
            Time = time;
        }
    }

}
