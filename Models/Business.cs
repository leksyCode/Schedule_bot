using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SheduleBot.Models
{
    public class Business
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Weekday { get; set; }
        public string Time { get; set; }
        public string  NotificationTime{ get; set; }

        public Business(int id, string name, string weekday, string time, string notification)
        {
            Id = id;
            Name = name;
            Weekday = weekday;
            Time = time;
            NotificationTime = notification;
        }

    }
}
