using Engine.Extensions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Engine.Algorithms
{
    public static class CommonLogic
    {
        public static bool IsValidToAssign(Schedulare _schedulare, Day _schedulareDay, Shift _schedulareShift, Worker _randomEmployee)
        {
            // if employee is in the shift
            if (IsEmployeeInShift(_schedulareShift, _randomEmployee)) return false;


            // if employee is in prev shift
            var shiftIndex = _schedulareDay.Shifts.FindIndex(x => x.Name.CompareContent(_schedulareShift.Name));
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
            shiftIndex = _schedulareDay.Shifts.FindIndex(x => x.Name.CompareContent(_schedulareShift.Name));
            if (shiftIndex < _schedulareDay.Shifts.Count() - 1)
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

        public static bool TryWithRetries(int numberOfRetries, Func<bool> method)
        {
            var exceptions = new List<Exception>();

            for (int attempted = 0; attempted < numberOfRetries; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        Thread.Sleep(500);
                    }
                    var result = method();

                    if (!result) continue;

                    return result;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            return false;
        }
    }

    //public static class TryWithRetries
    //{
    //    //public static void Do(
    //    //    Action action,
    //    //    TimeSpan retryInterval,
    //    //    int maxAttemptCount = 3)
    //    //{
    //    //    Do<object>(() =>
    //    //    {
    //    //        action();
    //    //        return null;
    //    //    }, retryInterval, maxAttemptCount);
    //    //}

    //    //public static T Do<T>(
    //    //    Func<T> action,
    //    //    TimeSpan retryInterval,
    //    //    int maxAttemptCount = 3)
    //    //{
    //    //    var exceptions = new List<Exception>();

    //    //    for (int attempted = 0; attempted < maxAttemptCount; attempted++)
    //    //    {
    //    //        try
    //    //        {
    //    //            if (attempted > 0)
    //    //            {
    //    //                Thread.Sleep(retryInterval);
    //    //            }
    //    //            return action();
    //    //        }
    //    //        catch (Exception ex)
    //    //        {
    //    //            exceptions.Add(ex);
    //    //        }
    //    //    }
    //    //    throw new AggregateException(exceptions);
    //    //}

    //    //public static bool RetryTwo(int numberOfRetries, Func<bool> method)
    //    //{
    //    //    if (numberOfRetries > 0)
    //    //    {
    //    //        try
    //    //        {
    //    //            var test = method();
    //    //            return test;
    //    //        }
    //    //        catch (Exception e)
    //    //        {
    //    //            // Log the exception
    //    //            Console.WriteLine(e);

    //    //            // wait half a second before re-attempting. 
    //    //            // should be configurable, it's hard coded just for the example.
    //    //            Thread.Sleep(500);

    //    //            // retry
    //    //            return RetryTwo(--numberOfRetries, method);
    //    //        }
    //    //    }
    //    //    return false;
    //    //}

    
    //}
}
