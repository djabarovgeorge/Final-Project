using Engine.Models;
using System;

namespace Engine.Interfaces
{
    public interface IHeuristicAlgo
    {
        Schedulare Execute(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer = null);
    }
}
