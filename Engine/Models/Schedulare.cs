using Engine.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Models
{
    [Serializable]
    public class Schedulare
    {
        private Dictionary<int, List<string>> shiftsTypes = new Dictionary<int, List<string>>
            {{ 2, new List<string> { "Morning", "Afternoon" }},
            { 3, new List<string> { "Morning", "Noon", "Afternoon" }},
            { 4, new List<string> { "Morning", "Noon", "Afternoon", "Night" }}};

        private readonly List<string> week = new List<string>{ "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        public List<Day> Days { get; set; }






        public Schedulare(ShiftsContainer shiftsContainer)
        {
            InitializeParams(shiftsContainer);
        }

        private void InitializeParams(ShiftsContainer shiftsContainer)
        {
            //List<Shift> shifts = CreateNewShift(shiftsContainer);

            List<Day> days = new List<Day>();
            for (int i = 0; i < shiftsContainer.ShiftParams.DaysOfWork; i++)
            {
                days.Add(new Day() { Name = week[i], Shifts = CreateNewShift(shiftsContainer) });
            }
            Days = days;
        }

        private List<Shift> CreateNewShift(ShiftsContainer shiftsContainer)
        {
            //List<Worker> workers = new List<Worker>();
            //for (int i = 0; i < shiftsContainer.ShiftParams.NumberOfWokersInShift; i++)
            //{
            //    workers.Add(new Worker());
            //}

            List<Shift> shifts = new List<Shift>();
            var numOfShifts = shiftsContainer.ShiftParams.NumberOfShiftsInDay;
            List<string> shiftsType;
            shiftsTypes.TryGetValue(numOfShifts, out shiftsType);
            for (int i = 0; i < numOfShifts; i++)
            {
                shifts.Add(new Shift() { Name = shiftsType[i], Workers = new List<Worker>() });
            }

            return shifts;
        }
    }
    [Serializable]
    public class Day
    {
        public string Name { get; set; }
        public List<Shift> Shifts { get; set; }
    }
    [Serializable]
    public class Shift
    {
        public string Name { get; set; }
        public List<Worker> Workers { get; set; }    
    }
    [Serializable]
    public class Worker
    {
        public string Name { get; set; }
    }


    public class SchedulareSatisfaction
    {
        public Schedulare Schedulare { get; set; }
        public double Satisfaction { get; set; }
        public ShiftsContainer ShiftsContainer { get; }
        public double Weight { get; set; }
        public double ExecuteTime { get; set; }

        public SchedulareSatisfaction(Schedulare schedulare, double satisfaction, ShiftsContainer shiftsContainer, double weight, double executeTime)
        {
            Schedulare = schedulare;
            Satisfaction = satisfaction;
            ShiftsContainer = shiftsContainer;
            Weight = weight;
            ExecuteTime = executeTime;
        }
    }

    public class SchedulareListStatistics
    {
        public List<SchedulareSatisfaction> SchedulareSatisfactionList { get; set; }
        public double SatisfactionAvarage { get; set; }
        public double WeightAvarage { get; set; }
        public double ExecuteTimeAvarage { get; set; }
        public SchedulareListStatistics()
        {
            SchedulareSatisfactionList = new List<SchedulareSatisfaction>();
            SatisfactionAvarage = 0;
            WeightAvarage = 0;
            ExecuteTimeAvarage = 0;
        }
    }
}
