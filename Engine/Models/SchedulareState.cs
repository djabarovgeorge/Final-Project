using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Models
{
    [Serializable]
    public class SchedulareState
    {
        //public Schedulare Schedulare { get; set; }
        public int Weight { get; set; }
        public TreeNode<Schedulare> Node { get; set; }
    }
}
