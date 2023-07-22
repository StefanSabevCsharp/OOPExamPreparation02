using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Models
{
    public class DomesticAssistant : Robot
    {
        const int BatteryCapacity = 20000;
        const int ConvertionCapacityIndex = 2000;
        public DomesticAssistant(string model) 
            : base(model, BatteryCapacity, ConvertionCapacityIndex)
        {
        }
    }
}
