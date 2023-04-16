using Formula1.Models.Contracts;
using Formula1.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Formula1.Models.ForPilots
{
    public class Pilot : IPilot
    {
        private string fullName;
        private bool canRace = false;
        private IFormulaOneCar formulaOneCar;
        private int numbersOfWin = 0;
        public Pilot(string fullName)
        {
            this.FullName = fullName;
            
            
        }
        public string FullName {get { return fullName;}private set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 5)
                {
                    throw new ArgumentException(String.Format(ExceptionMessages.InvalidPilot,value));
                }
                fullName = value;
            }
        }

        public IFormulaOneCar Car
        {
            get { return formulaOneCar; }
            private set
            {
                if (value == null)
                {
                    throw new NullReferenceException(String.Format(ExceptionMessages.InvalidCarForPilot));
                }
                formulaOneCar = value; 
            }
        }

        public int NumberOfWins { get { return numbersOfWin; } }

        public bool CanRace { get { return canRace; } }

        public void AddCar(IFormulaOneCar car)
        {
            this.Car=car;
            this.canRace = true;
        }

        public void WinRace()
        {
            numbersOfWin++;
        }

        public override string ToString()
        {
            return $"Pilot {fullName} has {numbersOfWin} wins.";
        }
    }
}
