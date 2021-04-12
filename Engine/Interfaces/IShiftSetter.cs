using Engine.Models;

namespace Engine.Interfaces
{
    public interface IShiftSetter
    {
        void SetNumberOfShiftsInDay(int num);
        void SetNumberOfDaysOfWork(int num);
        void SetNumberOfWorkersInShift(int num);
        void SetNumberOfWorkers(int num);

        ShiftsContainer Excute();
    }
}
