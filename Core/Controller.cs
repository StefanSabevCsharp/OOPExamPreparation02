using RobotService.Core.Contracts;
using RobotService.Models;
using RobotService.Models.Contracts;
using RobotService.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Core
{
    public class Controller : IController
    {
        SupplementRepository supplements;
        RobotRepository robots;
        public Controller()
        {
            supplements = new SupplementRepository();
            robots = new RobotRepository();
        }
        public string CreateRobot(string model, string typeName)
        {
            if (typeName != "DomesticAssistant" && typeName != "IndustrialAssistant")
            {
                return $"Robot type {typeName} cannot be created.";

            }

            IRobot robot = null;

            if (typeName == "DomesticAssistant")
            {
                robot = new DomesticAssistant(model);

            }
            else if (typeName == "IndustrialAssistant")
            {
                robot = new IndustrialAssistant(model);
            }

            robots.AddNew(robot);
            return $"{typeName} {model} is created and added to the RobotRepository.";
        }

        public string CreateSupplement(string typeName)
        {
            if (typeName != "SpecializedArm" || typeName != "LaserRadar")
            {
                return $"{typeName} is not compatible with our robots.";
            }
            ISupplement supplement = null;
            if (typeName == "SpecializedArm")
            {
                supplement = new SpecializedArm();
            }
            else if (typeName == "LaserRadar")
            {
                supplement = new LaserRadar();
            }
            supplements.AddNew(supplement);
            return $"{typeName} is created and added to the SupplementRepository.";
        }

        public string PerformService(string serviceName, int intefaceStandard, int totalPowerNeeded)
        {
            List<IRobot> robotsToPerform = robots.Models().Where(x => x.InterfaceStandards.Contains(intefaceStandard)).OrderByDescending(x => x.BatteryLevel).ToList();

            int count = 0;
            if (robotsToPerform.Count == 0)
            {
                return $"Unable to perform service, {intefaceStandard} not supported!";
            }

            int sumOfBatteryLevels = robotsToPerform.Sum(x => x.BatteryLevel);

            if (sumOfBatteryLevels < totalPowerNeeded)
            {
                return $"{serviceName} cannot be executed! {totalPowerNeeded - sumOfBatteryLevels} more power needed.";
            }
            else if (sumOfBatteryLevels >= totalPowerNeeded)
            {
                foreach (var robot in robotsToPerform)
                {
                    if (robot.BatteryLevel >= totalPowerNeeded)
                    {

                        robot.ExecuteService(totalPowerNeeded);
                        count++;
                        break;
                    }
                    else if (robot.BatteryLevel < totalPowerNeeded)
                    {
                        totalPowerNeeded -= robot.BatteryLevel;
                        robot.ExecuteService(robot.BatteryLevel);
                        count++;
                    }
                }
            }
            return $"{serviceName} is performed successfully with {count} robots.";
        }

        public string Report()
        {
            List<IRobot> robotsToPrint = robots.Models().OrderByDescending(x => x.BatteryLevel).ThenBy(x => x.BatteryCapacity).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var robot in robotsToPrint)
            {
                sb.AppendLine(robot.ToString());

            }
            return sb.ToString().TrimEnd();
        }

        public string RobotRecovery(string model, int minutes)
        {
            List<IRobot> robotsToFeed = robots.Models().Where(robot => robot.BatteryLevel < robot.BatteryCapacity).ToList();
            foreach (var robot in robotsToFeed)
            {
                robot.Eating(minutes);
            }
            return $"Robots fed: {robotsToFeed.Count}";
        }

        public string UpgradeRobot(string model, string supplementTypeName)
        {
            ISupplement supplement = supplements.Models().FirstOrDefault(x => x.GetType().Name == supplementTypeName);
            List<IRobot> robotsToUpgrade = robots.Models().Where(x => !x.InterfaceStandards.Contains(supplement.InterfaceStandard) && x.Model == model).ToList();
            if (robotsToUpgrade.Count == 0)
            {
                return $"All {model} are already upgraded!";
            }
            IRobot robot = robotsToUpgrade.First();
            robot.InstallSupplement(supplement);
            supplements.RemoveByName(supplementTypeName);
            return $"{model} is upgraded with {supplementTypeName}.";
        }
    }
}
