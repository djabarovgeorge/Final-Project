using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Interfaces
{
    public interface IFactory
    {
        IAlgo GetAlgo(string key);
        Dictionary<string, IAlgo> GetAll();
    }
}
