using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application2.Models
{

    public class MachineTemper
    {
        public Areadata[] areadatas { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public int code { get; set; }
    }

    public class Areadata
    {
        public string warmareaname { get; set; }
        public int temperature { get; set; }
        public int temperaturemode { get; set; }
    }

}