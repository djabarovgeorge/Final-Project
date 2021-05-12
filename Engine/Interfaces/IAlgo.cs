using Engine.Models;
using System;

namespace Engine.Interfaces
{
    public interface IAlgo
    {
        SchedulareState Execute(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer = null);
    }
}
