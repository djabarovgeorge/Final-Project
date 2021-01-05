using Engine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Interfaces
{
    public interface IShiftSetter
    {
        void SetNumberOfShiftsInDay(int num);
        void SetNumberOfDaysOfWork(int num);
        void SetNumberOfWorkersInShift(int num);
        void SetNumberOfWorkers(int num);

        List<Employee> Excute();
    }
}
