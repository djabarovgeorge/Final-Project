using Engine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Services
{
    public class ConstraintsHandler
    {
        Random rnd;

        private List<Employee> employeesConstraints = new List<Employee>();

        //To save number of employees
        private int numberOfShiftsInWeek;

        //Number of employess in shift

        //Dict of Emplyoess
        private Dictionary<int, Dictionary<String, int>> emplyoessDict;

        //random number for chooese employess from the list
        private int chooseRandomEmployee;

        //List of random people
        private int numberOfEployessInshift;

        //Number of shifts in one day;
        private int numberOfShiftInDay;

        //Total number of Employess in this week
        private int totalNumberOfEmployess;

        //Dict of all week : shift in the morning 1 , Noon 2 , afternoon 3 ......
        private Dictionary<String, int> week;

        //List to run on the dictionary (WEEK);
        private String[] thisWeek = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        //If i don't have 7 days , I randomly choose the days
        //I build list for choose (The size of the array will be about the size of a week)
        private int[] chooseDayInWeek = new int[7];

        private Dictionary<int, List<string>> shiftsTypes = new Dictionary<int, List<string>>();
        private List<string> currShiftType = new List<string>();

        //C'tor
        public ConstraintsHandler(int totalNumberOfEmployess, int numberOfShiftsInWeek, int numberOfShiftInDay, int numberOfEployessInshift)
        {
            this.numberOfShiftInDay = numberOfShiftInDay;
            this.numberOfShiftsInWeek = numberOfShiftsInWeek;
            this.totalNumberOfEmployess = totalNumberOfEmployess;
            this.numberOfEployessInshift = numberOfEployessInshift;

            //emplyoessDict = new Dictionary<int, Dictionary<String, int>>();
            rnd = new Random();

            shiftsTypes.Add(2, new List<string> { "Morning", "Afternoon" });
            shiftsTypes.Add(3, new List<string> { "Morning", "Noon", "Afternoon", });
            shiftsTypes.Add(4, new List<string> { "Morning", "Noon", "Afternoon", "Night" });

            shiftsTypes.TryGetValue(numberOfShiftInDay, out currShiftType);
        }


        public List<Employee> MakeListOfEmployess()
        {
            for (int empNum = 0; empNum < totalNumberOfEmployess; empNum++)
            {
                var emp = new Employee();
                for (int shiftInit = 0; shiftInit < numberOfShiftsInWeek; shiftInit++)
                {
                    var randDay = GenerateRandomDay();
                    emp.WeeklyConstraints.TryGetValue(randDay, out string checkIfFree);
                    if (!checkIfFree.Contains("Free day")) // check if the day is free if no try again
                    {
                        shiftInit--;
                        continue;
                    }
                    else
                    {
                        // generate rand shift 
                        var randShift = currShiftType[rnd.Next(0, currShiftType.Count)];

                        emp.WeeklyConstraints[randDay] = randShift;
                    }
                }

                employeesConstraints.Add(emp);
            }
            return employeesConstraints;
        }


        public string GenerateRandomDay()
        {
            var randDay = new List<string> { "Sundey", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturdaty" };
            var randIndex = rnd.Next(0, 7);

            return randDay[randIndex];
        }


    }
}

