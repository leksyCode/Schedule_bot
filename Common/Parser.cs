using SheduleBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SheduleBot.Common
{
    public class Parser
    {
        public Parser()
        {

        }

        public List<Business> ParseText(List<string> TimeLine)
        {
            List<Business> Doings = new List<Business>();
            string[] arr;
            foreach (var str in TimeLine)
            {
                arr = str.Split(',');

                Doings.Add(new Business(id: TimeLine.IndexOf(str), name: arr[0].Trim(), weekday: arr[1].Trim(), time: arr[2].Trim(), notification: arr[3].Trim()));
            }           
            return Doings;
        }
    }
}
