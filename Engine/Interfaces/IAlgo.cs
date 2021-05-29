using Engine.Models;
using System;

namespace Engine.Interfaces
{
    public interface IAlgo
    {
        int ALGORITHM_RUN_TIME_SECONDS { get; }

        SchedulareState Execute(Schedulare schedulare, ShiftsContainer shiftsContainer);
    }
}
