using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Models
{

    public class GreedyEmployee
    {
        public string Name { get; set; }

        public int NumberOfShiftsThatWasAssigned { get; set; }

        public List<EmployeeShift> ShiftsThatWasNotAssigned = new List<EmployeeShift>();

        public void AddShift()
        {
            NumberOfShiftsThatWasAssigned++;
        }
    }

    public class EmployeeShift
    {
        public string Day { get; set; }
        public string ShiftName { get; set; }
    }
 
}
