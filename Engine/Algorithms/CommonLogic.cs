using ConsoleTables;
using Engine.Extensions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Engine.Algorithms
{
    public static class CommonLogic
    {
        private const string LOG_PATH = "d:\\huristicdata\\{0}.txt";
        private static string FileName = "algosummary";
        private static int FileSuffix { get; set; }

        public static void InitFileSuffix()
        {
            var count = 0;

            while (File.Exists(GetFilePath(FileName, count)))
                count++;

            FileSuffix = count;
        }

        public static bool IsValidToAssign(Schedulare _schedulare, Day _schedulareDay, Shift _schedulareShift, Worker _randomEmployee)
        {
            try
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
            catch (Exception e)
            {
                ApeandToFile(e.ToString());
            }
            return default;
        }

        public static Double GetPercentageOfSatisfaction(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            try
            {
                var numberOfWorkerConstrains = shiftsContainer.EmployeeConstraints.Count * shiftsContainer.ShiftParams.NumberOfDaysOfWork;

                var count = 0;

                foreach (var day in schedulare.Days)
                {
                    foreach (var shift in day.Shifts)
                    {
                        foreach (var worker in shift.Workers)
                        {
                            if (worker.Name.IsNullOrEmpty()) continue;

                            if (IswantedShift(schedulare, shiftsContainer, worker.Name, day.Name, shift))
                            {
                                count++;
                            }
                        }
                    }
                }

                var percentageOfSatisfaction = ((Double)count / (Double)numberOfWorkerConstrains) * 100;

                return percentageOfSatisfaction;
            }
            catch (Exception e)
            {
                ApeandToFile(e.ToString());
            }
            return default;
        }

        public static void PrintSchedulare(Schedulare value=null, ShiftsContainer shiftsContainer=null)
        {
            try
            {
                var columns = value.Days.Select(x => x.Name).ToArray();

                var table = new ConsoleTable(columns);

                var data = value.Days.Select(x => x.Shifts).ToList();

                var rows = new List<Week>();
                for (int i = 0; i < data.FirstOrDefault().Count; i++)
                {

                    var list = new List<string>();

                    for (int dayI = 0; dayI < data.Count; dayI++)
                    {
                        var day = (DayOfWeek)dayI;
                        var dayString = day.ToString();
                        list.Add(string.Join(",", data[dayI][i].Workers.Where(x => !x.Name.IsNullOrEmpty()).Select(y => IswantedShift(value, shiftsContainer, y.Name, dayString, data[dayI][i]) ? y.Name : $"{y.Name} X").ToList()));
                    }

                    var obj = new Week(list);

                    rows.Add(obj);
                }

                ConsoleTable
                    .From(rows)
                    .Configure(o => o.NumberAlignment = Alignment.Right)
                    .Write(Format.Alternative);
            }
            catch (Exception e)
            {
                ApeandToFile(e.ToString());
            }
        }

        private static bool IswantedShift(Schedulare schedulare, ShiftsContainer shiftsContainer, string employeeName, string currDay, Shift shift)
        {
            try
            {
                var currEmpConstraints = shiftsContainer.EmployeeConstraints.FirstOrDefault(x => x.Name.CompareContent(employeeName));

                var dayToValidate = currEmpConstraints.WeeklyConstraints.FirstOrDefault(x => x.Key.CompareContent(currDay));

                return dayToValidate.Value.CompareContent(shift.Name);
            }
            catch (Exception e)
            {
                ApeandToFile(e.ToString());
            }

            return default;
        }

        private static bool IsEmployeeInShift(Shift _schedulareShift, Worker _randomEmployee)
        {
            try
            {
                return _schedulareShift.Workers.Any(x => x.Name.CompareContent(_randomEmployee.Name));

            }
            catch (Exception e)
            {
                ApeandToFile(e.ToString());
            }
            return default;
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
        public static void ApeandToFile(string str , string fileName = "algosummary")
        {
            File.AppendAllText(GetFilePath(fileName, FileSuffix), str + Environment.NewLine);
        }

        private static string GetFilePath(string fileName, int count)
        {
            return string.Format(LOG_PATH, $"{fileName}_{count}");
        }

        private class Week
        {
        public string Sunday { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }


        public Week(List<string> list)
            {
                Sunday = list[0];
                Monday = list[1];
                Tuesday = list[2];
                Wednesday = list[3];
                Thursday = list[4];
                Friday = list[5];
                Saturday = list[6];
            }
        }
    }

    public class SchedulareComparer : IComparer<SchedulareState>
    {
        public int Compare(SchedulareState schedulareA, SchedulareState schedulareB)
        {

            if (schedulareA.Weight == schedulareB.Weight)
            {
                return 0;
            }
            else if (schedulareA.Weight > schedulareB.Weight)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
    public class SchedulareComparerArray : IComparer<SchedulareState>
    {
        public int Compare(SchedulareState schedulareA, SchedulareState schedulareB)
        {
            if (schedulareA == schedulareB)
                return 0;

            if (schedulareA.Weight == schedulareB.Weight)
                return 1;

            else if (schedulareA.Weight > schedulareB.Weight)
                return 1;

            else
                return -1;
        }

    }
 
}

