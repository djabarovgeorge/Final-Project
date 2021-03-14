using Engine.Extensions;
using Engine.Models;
using System.Linq;

namespace Engine.Algorithms
{
    public static class CommonLogic
    {
        public static bool IsValidToAssign(Schedulare _schedulare, Day _schedulareDay, Shift _schedulareShift, Worker _randomEmployee)
        {
            // if employee is in the shift
            if (IsEmployeeInShift(_schedulareShift, _randomEmployee)) return false;
            

            // if employee is in prev shift
            var shiftIndex = _schedulareDay.Shifts.FindIndex(x => x.Name.ContainsContent(_schedulareShift.Name));
            Shift shiftToValidate;
            if (shiftIndex > 0)
            {
                shiftToValidate = _schedulareDay.Shifts[shiftIndex - 1];
                if (IsEmployeeInShift(shiftToValidate, _randomEmployee)) return false;
 
            }
            else if (!_schedulareDay.Name.ContainsContent("sunday"))
            // shiftIndex == 0 and we need to check the prev day last shift 
            // at current logic we do not support 2 week therefore is its sunday we aprove
            {
                var dayIndex = _schedulare.Days.FindIndex(x => x.Name.ContainsContent(_schedulareDay.Name));
                Day dayToValidate = _schedulare.Days[dayIndex - 1];
                shiftToValidate = dayToValidate.Shifts.LastOrDefault();
                if (IsEmployeeInShift(shiftToValidate, _randomEmployee)) return false;
            }


            // if employee is in next shift
            shiftIndex = _schedulareDay.Shifts.FindIndex(x => x.Name.ContainsContent(_schedulareShift.Name));
            if (shiftIndex < _schedulareDay.Shifts.Count() - 1 )
            {
                shiftToValidate = _schedulareDay.Shifts[shiftIndex + 1];
                if (IsEmployeeInShift(shiftToValidate, _randomEmployee)) return false;
            }
            else if (!_schedulareDay.Name.ContainsContent("saturday"))
            // shiftIndex == last shift and we need to check the next day first shift 
            // at current logic we do not support 2 week therefore is its saturday we aprove
            {
                var dayIndex = _schedulare.Days.FindIndex(x => x.Name.ContainsContent(_schedulareDay.Name));
                Day dayToValidate = _schedulare.Days[dayIndex + 1];
                shiftToValidate = dayToValidate.Shifts.FirstOrDefault();
                if (IsEmployeeInShift(shiftToValidate, _randomEmployee)) return false;
            }
            return true;
        }

        private static bool IsEmployeeInShift(Shift _schedulareShift, Worker _randomEmployee)
        {
            return _schedulareShift.Workers.Any(x => x.Name.CompareContent(_randomEmployee.Name));
        }
    }
}
