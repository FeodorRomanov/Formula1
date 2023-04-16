using Formula1.Core.Contracts;
using Formula1.Models.Contracts;
using Formula1.Models.Formula;
using Formula1.Models.ForPilots;
using Formula1.Models.Racing;
using Formula1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula1.Core
{
    public class Controller : IController
    {
        private PilotRepository pilotRepository;
        private RaceRepository raceRepository;
        private FormulaOneCarRepository carRepository;
        public Controller()
        {
            this.pilotRepository= new PilotRepository();
            this.raceRepository= new RaceRepository();
            this.carRepository= new FormulaOneCarRepository();
        }
        public string AddCarToPilot(string pilotName, string carModel)
        {
            IPilot currPilot;
            if (!pilotRepository.Models.Any(x => x.FullName == pilotName))
            {
                throw new InvalidOperationException($"Pilot {pilotName} does not exist or has a car.");
            }
            else
            {
                currPilot = pilotRepository.Models.First(x => x.FullName == pilotName);
                if (currPilot.Car != null)
                {
                    throw new InvalidOperationException($"Pilot {pilotName} does not exist or has a car.");
                }
            }

            IFormulaOneCar currCar;
            if (!carRepository.Models.Any(x => x.Model == carModel))
            {
                throw new NullReferenceException($"Car {carModel} does not exist.");
            }
            currCar=carRepository.Models.First(x=>x.Model==carModel);
            currPilot.AddCar(currCar);
            carRepository.Remove(currCar);
            return $"Pilot {currPilot.FullName} will drive a {currCar.GetType().Name} {currCar.Model} car.";
        }

        public string AddPilotToRace(string raceName, string pilotFullName)
        {
            IRace currRace;
            IPilot currPilot;
            if (!raceRepository.Models.Any(x => x.RaceName == raceName))
            {
                throw new NullReferenceException($"Race {raceName} does not exist.");
            }
            currRace = raceRepository.Models.First(x => x.RaceName == raceName);
            if (!pilotRepository.Models.Any(x => x.FullName == pilotFullName))
            {
                throw new InvalidOperationException($"Can not add pilot {pilotFullName} to the race.");
            }
            currPilot = pilotRepository.Models.First(x => x.FullName == pilotFullName);
            if (currPilot.CanRace == false)
            {
                throw new InvalidOperationException($"Can not add pilot {pilotFullName} to the race.");
            }
            if(currRace.Pilots.Any(x=>x.FullName == pilotFullName))
            {
                throw new InvalidOperationException($"Can not add pilot {pilotFullName} to the race.");
            }

            currRace.AddPilot(currPilot);
            return $"Pilot {pilotFullName} is added to the {currRace.RaceName} race.";
        }

        public string CreateCar(string type, string model, int horsepower, double engineDisplacement)
        {
            if (carRepository.Models.Any(x => x.Model == model))
            {
                throw new InvalidOperationException($"Formula one car {model} is already created.");
            }

            if (type != "Ferrari" && type != "Williams")
            {
                throw new InvalidOperationException($"Formula one car type {type} is not valid.");
            }

            IFormulaOneCar currCar;
            if (type == "Ferrari")
            {
                currCar = new Ferrari(model, horsepower, engineDisplacement);
            }
            else
            {
                currCar = new Williams(model, horsepower, engineDisplacement);
            }
            carRepository.Add(currCar);
            return $"Car {type}, model {model} is created.";
        }

        public string CreatePilot(string fullName)
        {
            if (pilotRepository.Models.Any(x => x.FullName == fullName))
            {
                throw new InvalidOperationException($"Pilot {fullName} is already created.");
            }
            var currPilot = new Pilot(fullName);
            pilotRepository.Add(currPilot);
            return $"Pilot {fullName} is created.";
        }

        public string CreateRace(string raceName, int numberOfLaps)
        {
            if (raceRepository.Models.Any(x => x.RaceName == raceName))
            {
                throw new InvalidOperationException($"Race {raceName} is already created.");
            }

            var currRacer=new Race(raceName, numberOfLaps);
            raceRepository.Add(currRacer);
            return $"Race {raceName} is created.";
        }

        public string PilotReport()
        {
            StringBuilder sb=new StringBuilder();
            foreach (var item in pilotRepository.Models.OrderByDescending(x=>x.NumberOfWins))
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString().TrimEnd();
        }

        public string RaceReport()
        {
            StringBuilder sb=new StringBuilder();
            foreach (var item in raceRepository.Models.Where(x=>x.TookPlace==true))
            {
                sb.AppendLine(item.RaceInfo());
            }
            return sb.ToString().TrimEnd();
        }

        public string StartRace(string raceName)
        {
            IRace currRace;
            if(!raceRepository.Models.Any(x => x.RaceName == raceName))
            {
                throw new NullReferenceException($"Race {raceName} does not exist.");
            }
            currRace = raceRepository.Models.First(x => x.RaceName == raceName);
            int cnt = currRace.Pilots.Count;
            if(currRace.Pilots.Count < 3)
            {
                throw new InvalidOperationException($"Race {raceName} cannot start with less than three participants.");
            }

            if (currRace.TookPlace == true)
            {
                throw new InvalidOperationException($"Can not execute race {raceName}.");
            }
            List<IPilot> result = new List<IPilot>();
            foreach (var item in currRace.Pilots.OrderByDescending(x=>x.Car.RaceScoreCalculator(currRace.NumberOfLaps)))
            {
                result.Add(item);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Pilot {result[0].FullName} wins the {raceName} race.");
            sb.AppendLine($"Pilot {result[1].FullName} is second in the {raceName} race.");
            sb.AppendLine($"Pilot {result[2].FullName} is third in the {raceName} race.");
            result[0].WinRace();
            currRace.TookPlace = true;
            return sb.ToString().TrimEnd();

        }
    }
}
