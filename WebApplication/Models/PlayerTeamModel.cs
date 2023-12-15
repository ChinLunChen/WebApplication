using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class PlayerTeamModel
    {
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public string League { get; set; }
        public int Age { get; set; }
        public int Height {  get; set; }
        public int Weight { get; set; }
    }
}