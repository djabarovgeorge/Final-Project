using Engine.Models;
using System;

namespace Engine.Interfaces
{
    public interface IAlgorithm
    {
        Double Execute(Schedulare schedulare, ShiftsContainer shiftsContainer);
    }
}
