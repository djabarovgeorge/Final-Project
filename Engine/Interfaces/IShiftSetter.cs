using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Interfaces
{
    public interface IShiftSetter
    {
        void SetNumberOfShiftsInDay();
        void SetNumberOfDaysOfWork();
        void SetNumberOfWorkersInShift();
        int SetNumberOfWorkers();


        void SetConstraints();
    }
}
