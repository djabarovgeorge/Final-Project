using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Models
{
    [Serializable]
    public class SchedulareState
    {
        public int Weight { get; set; }
        public TreeNode<Schedulare> Node { get; set; }
        public double ExecuteTime { get; set; }
        public double MostUnfortunateWorkerPer { get; set; }
    }
}
