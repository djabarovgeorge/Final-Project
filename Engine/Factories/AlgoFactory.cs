using Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Factories
{
    public class AlgoFactory : IFactory
    {

        private readonly Dictionary<string, IAlgo> _handlers;

        public AlgoFactory(IEnumerable<IAlgo> handlers)
        {
            _handlers = new Dictionary<string, IAlgo>();

            foreach (var handler in handlers)
            {
                _handlers[handler.GetType().Name] = handler;
            }
        }

        public IAlgo GetAlgo(string key)
        {
            return _handlers[key];
        }

        public Dictionary<string, IAlgo> GetAll()
        {
            return _handlers;
        }

    }
}

