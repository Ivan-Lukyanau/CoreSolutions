using System.Collections.Generic;

namespace CoreFireAPI.Models.Time
{
public class Timeslot
{
    public string Time { get; set; }
    public bool Available { get; set; }

    public Timeslot(string time, bool available)
    {
        Available = available;
        Time = time;
    }

    public static IEnumerable<Timeslot> FromDictionary(Dictionary<string, bool> dictionary)
    {
        foreach (var el in dictionary)
        {
            yield return new Timeslot(el.Key, el.Value);
        }
    }
}
}
