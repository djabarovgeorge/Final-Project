using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Models
{
    public class ShiftsContainer
    {
        public List<EmployeeConstraints> EmployeeConstraints { get; set; }
        public ShiftParams ShiftParams{ get; set; }
    }

    public class ShiftParams
    {
        public int DaysOfWork { get; set; }
        public int NumberOfDaysOfWork { get; set; }
        public int NumberOfShiftsInDay { get; set; }
        public int NumberOfWokersInShift { get; set; }
        public int NumberOfWorkers { get; set; }
    }
}
