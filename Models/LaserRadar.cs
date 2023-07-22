using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Models
{
    public class LaserRadar : Supplement
    {
        const int InterfaceStandard = 20082;
        const int BatteryUsage = 5000;

        public LaserRadar()
            : base(InterfaceStandard, BatteryUsage)
        {
        }
    }
}
