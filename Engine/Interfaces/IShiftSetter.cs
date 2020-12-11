using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Interfaces
{
    protected interface IShiftSetter
    {
        void SetNumberOfShiftsInWeek();
        void SetNumberOfDaysOfWork();
        void SetConstraints();
    }
}
