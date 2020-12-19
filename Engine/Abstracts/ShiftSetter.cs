using Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Abstracts
{
    public abstract class ShiftSetter : IShiftSetter
    {
        public readonly int DaysOfWork = 7;
        public int WorketNumberOfDaysOfWork { get; set; } = 5;
        public int NumberOfShiftsInDay { get; set; } = 3;
        public int NumberOfWokersInShift { get; set; } = 2;
        public int NumberOfWorkers => SetNumberOfWorkers();

        public abstract void SetConstraints();
        public virtual void SetNumberOfDaysOfWork() {}
        public virtual void SetNumberOfShiftsInDay() {}
        public virtual void SetNumberOfWorkersInShift() {}
        public virtual void Excute(){}
        public int SetNumberOfWorkers()
        {
           // TODO need to validate the result 
           // there maybe a float result

            var res = (DaysOfWork * NumberOfShiftsInDay * NumberOfWokersInShift) / WorketNumberOfDaysOfWork;
            return res;
        }
    }
}
