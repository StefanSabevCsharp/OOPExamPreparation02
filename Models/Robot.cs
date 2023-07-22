using RobotService.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Models
{
    public abstract class Robot : IRobot
    {
        string model;
        int batteryCapacity;
        int batteryLevel;
        int convertionCapacityIndex;
        private List<int> interfaceStandardsList;

        protected Robot(string model, int batteryCapacity, int convertionCapacityIndex)
        {
            Model = model;
            BatteryCapacity = batteryCapacity;
            BatteryLevel = batteryCapacity;
            ConvertionCapacityIndex = convertionCapacityIndex;
            interfaceStandardsList = new List<int>();
        }

        public string Model
        {
            get => model;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(Utilities.Messages.ExceptionMessages.ModelNullOrWhitespace);
                }
                model = value;
            }
        }

        public int BatteryCapacity
        {
            get => batteryCapacity;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException(Utilities.Messages.ExceptionMessages.BatteryCapacityBelowZero);
                }
                batteryCapacity = value;
            }
        }

        public int BatteryLevel { get; private set; }


        public int ConvertionCapacityIndex { get; private set; }

        public IReadOnlyCollection<int> InterfaceStandards => interfaceStandardsList;

        public void Eating(int minutes)
        {

            for (int i = 0; i < minutes; i++)
            {
                int producedEnergy = convertionCapacityIndex * minutes;
                batteryLevel += producedEnergy;
                if (batteryLevel == batteryCapacity || batteryLevel >= batteryCapacity)
                {
                    batteryLevel = batteryCapacity;
                    break;
                }
            }
        }

        public bool ExecuteService(int consumedEnergy)
        {
            if (batteryLevel >= consumedEnergy)
            {
                batteryLevel -= consumedEnergy;
                return true;
            }

            return false;

        }

        public void InstallSupplement(ISupplement supplement)
        {
            interfaceStandardsList.Add(supplement.InterfaceStandard);
            BatteryCapacity -= supplement.BatteryUsage;
            BatteryLevel -= supplement.BatteryUsage;
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string supplements = interfaceStandardsList.Count == 0 ? "none" : string.Join(" ", interfaceStandardsList);
           
            stringBuilder.AppendLine($"{GetType().Name} {Model}:");
            stringBuilder.AppendLine($"--Maximum battery capacity: {BatteryCapacity}");
            stringBuilder.AppendLine($"--Current battery level: {BatteryLevel}");
            stringBuilder.AppendLine($"--Supplements installed: {supplements}");

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
