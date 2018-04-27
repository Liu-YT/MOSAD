using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Models
{
    class weather
    {
        public string windDirection { get; set; }

        public string windPower { get; set; }

        public string high { get; set; }

        public string low { get; set; }

        public string date { get; set; }

        public string type { get; set; }

        public weather(string winDirection, string winPower, string high, string low, string date, string type)
        {
            this.windDirection = windDirection;
            this.windPower = windPower;
            this.high = high;
            this.low = low;
            this.type = type;
        }
    }
}
