using Engine.Interfaces;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Abstracts
{
    public abstract class ShiftSetter : IShiftSetter
    {
        public readonly int DaysOfWork = 7;
        public int NumberOfDaysOfWork { get; set; } = 5;
        public int NumberOfShiftsInDay { get; set; } = 3;
        public int NumberOfWokersInShift { get; set; } = 2;
        public int NumberOfWorkers { get; set; } = 5;

        public abstract void SetNumberOfShiftsInDay(int num);
        public abstract void SetNumberOfDaysOfWork(int num);
        public abstract void SetNumberOfWorkersInShift(int num);
        public abstract void SetNumberOfWorkers(int num);
        public abstract List<Employee> Excute();
    }
}
