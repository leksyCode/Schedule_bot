using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SheduleBot.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Shedule_FileId { get; set; }
        public List<string> TimeLine { get; set; } = new List<string>();
        public bool SetShedule { get; set; }
        public bool SetTimeline { get; set; }
        public List<Business> Doings { get; set;} = new List<Business>();

        public User(int id)
        {
            Id = id;
        }
    }
}
