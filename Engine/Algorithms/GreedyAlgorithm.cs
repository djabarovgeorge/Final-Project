using Engine.Extensions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Algorithms
{
    public class GreedyAlgorithm
    {

        private Schedulare schedulare;
        private bool ifWorkerInThisShift;
        private ShiftsContainer shiftsContainer;
        private List<GreedyEmployee> greedyEmpList;
        private List<GreedyEmployee> conflicts;
        private List<string> week = new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };



        public GreedyAlgorithm(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {

            this.schedulare = schedulare;
            this.shiftsContainer = shiftsContainer;
            greedyEmpList = new List<GreedyEmployee>();


        }

        public void StartAlgorithm()
        {
            PartOne();
            PartTwo();
            PartThree();
            PrintAll();

        }

        //Creating the system//Work!
        private void PartOne()
        {


            foreach (var emp in shiftsContainer.EmployeeConstraints)
            {
                var currEmpName = new Worker() { Name = emp.Name };

                var greedyEmploee = new GreedyEmployee() { Name = emp.Name };

                Console.WriteLine("**1**");
                foreach (var empDay in emp.WeeklyConstraints)
                {
                    if (empDay.Value.Contains("Free day")) continue;

                    var constraintsDay = empDay.Key;
                    var constraintsShift = empDay.Value;

                    var schedulareDay = schedulare.Days.FirstOrDefault(x => x.Name.Contains(constraintsDay));

                    var schedulareShift = schedulareDay.Shifts.FirstOrDefault(x => x.Name.Contains(constraintsShift));

                    if (schedulareShift.Workers.Count < shiftsContainer.ShiftParams.NumberOfWokersInShift)
                    {
                        greedyEmploee.AddShift();
                        Worker newEmp = new Worker();
                        newEmp.Name = currEmpName.Name;
                        schedulareShift.Workers.Add(newEmp);
                    }
                    else
                    {
                        var shift = new EmployeeShift() { Day = constraintsDay, ShiftName = constraintsShift };

                        greedyEmploee.ShiftsThatWasNotAssigned.Add(shift);
                    }
                    Console.WriteLine("**2**");
                }
                greedyEmpList.Add(greedyEmploee);
            }
            PrintAll();

        }




        private void PartTwo()
        {

            conflicts = greedyEmpList.Where(x => x.ShiftsThatWasNotAssigned.Count > 1).ToList();
            var iterations = 0;

            //As long as there are employees who have more than one shift they have not received will continue
            while (conflicts.Count != 0 && iterations != shiftsContainer.ShiftParams.NumberOfWorkers)
            {
                Console.WriteLine("**3**");

                foreach (var employeeCheck in conflicts)
                {
                    Console.WriteLine("**4**");

                    foreach (var iDontHaveThisDayList in employeeCheck.ShiftsThatWasNotAssigned.ToList())
                    {
                        Console.WriteLine("**5**");
                        //Accept the day the constraint was not accepted.
                        var shiftDay = schedulare.Days.FirstOrDefault(x => x.Name.ContainsContent(iDontHaveThisDayList.Day));

                        //Take from the selected day the shift that the employee did not receive
                        var shift = shiftDay.Shifts.FirstOrDefault(x => x.Name.ContainsContent(iDontHaveThisDayList.ShiftName));

                        //All employees on the same shift1
                        var currShiftGreedyWorkers = shift.Workers.Select(x => greedyEmpList.FirstOrDefault(y => x.Name == y.Name)).ToList();

                        //Take the fact that most of the constraints have come true
                        var luckyWorks = currShiftGreedyWorkers.OrderBy(x => x.ShiftsThatWasNotAssigned.Count()).LastOrDefault();





                        //if greater then some value
                        if (luckyWorks.ShiftsThatWasNotAssigned.Count < employeeCheck.ShiftsThatWasNotAssigned.Count)
                        {
                            for (int i = 0; i < shift.Workers.Count; i++)
                            {
                                Console.WriteLine("**6**");
                                var checkIfIn = schedulare.Days[week.IndexOf(shiftDay.Name)].Shifts.Find(x => x.Name == shift.Name).Workers.Find(x => x.Name == employeeCheck.Name);
                                var repleceWorker = schedulare.Days[week.IndexOf(shiftDay.Name)].Shifts.Find(x => x.Name == shift.Name).Workers[i].Name == luckyWorks.Name;


                                if (repleceWorker && checkIfIn == null)
                                {

                                    //Name of worker

                                    this.PrintAll();

                                    var thisDay = week.IndexOf(shiftDay.Name);

                                    var schedulare1 = schedulare.Days[thisDay];

                                    var select2 = schedulare1.Shifts.Find(x => x.Name == shift.Name);

                                    var getNumber = select2.Workers[i].Name;

                                    var temp = schedulare.Days[thisDay].Shifts.Find(x => x.Name == shift.Name).Workers[i].Name;
                                    //Change in the shift the name of the worker//i have problem here//
                                    foreach (var item in schedulare.Days[thisDay].Shifts)
                                    {

                                        if (item.Name == shift.Name)
                                        {
                                            Console.WriteLine("**1**");
                                            item.Workers[i].Name = employeeCheck.Name;
                                            break;
                                        }

                                    }
                                    //schedulare.Days[thisDay].Shifts.Find(x => x.Name == shift.Name).Workers[i].Name = employeeCheck.Name;


                                    Console.WriteLine("***********************************After change****************");

                                    this.PrintAll();
                                    //Subtract from the amount of constraints not received
                                    greedyEmpList.FirstOrDefault(x => x.Name == employeeCheck.Name).ShiftsThatWasNotAssigned.Remove(iDontHaveThisDayList);

                                    //Will add to the amount of constraints
                                    greedyEmpList.FirstOrDefault(x => x.Name == employeeCheck.Name).NumberOfShiftsThatWasAssigned++;

                                    //Add the shift to employee constraints
                                    greedyEmpList.FirstOrDefault(x => x.Name == luckyWorks.Name).ShiftsThatWasNotAssigned.Add(iDontHaveThisDayList);

                                    //Down number of shift 
                                    greedyEmpList.FirstOrDefault(x => x.Name == luckyWorks.Name).NumberOfShiftsThatWasAssigned--;


                                }
                            }
                        }
                    }

                }
                //That there are no more conflicts between employees means that there is at least one constraint
                conflicts = greedyEmpList.Where(x => x.ShiftsThatWasNotAssigned.Count > 1).ToList();
                iterations++;
            }
            this.PrintAll();
        }



        private void PartThree()
        {

            var CheckIfFinish = 0;
            //Take one day in the schedulare
            for (int RunningOnTheDay = 0; RunningOnTheDay < schedulare.Days.Count; RunningOnTheDay++)
            {
                Console.WriteLine("**7**");
                //The number of shifts per day
                for (int RunOnShifts = 0; RunOnShifts < schedulare.Days[RunningOnTheDay].Shifts.Count; RunOnShifts++)
                {
                    Console.WriteLine("**8**");
                    //Is the number of shifts of employees smaller than the number of employees who are supposed to be on shift
                    while (schedulare.Days[RunningOnTheDay].Shifts[RunOnShifts].Workers.Count < shiftsContainer.ShiftParams.NumberOfWokersInShift)
                    {
                        if (CheckIfFinish == 9)
                        {
                            CheckIfFinish++;
                        }
                        Console.WriteLine("**9**");
                        PartTwoNumTwo(RunningOnTheDay, RunOnShifts);
                        CheckIfFinish++;

                    }

                }
            }



        }

        private void PartTwoNumTwo(int RunningOnTheDay, int RunOnShifts)
        {

            //need to put all the worker
            for (int getEmployeesWithShift = 0; getEmployeesWithShift < greedyEmpList.Count; getEmployeesWithShift++)
            {
                Console.WriteLine("**10**");
                var indexOfWorker = schedulare.Days[RunningOnTheDay].Shifts[RunOnShifts].Workers.FindIndex(x => x.Name == greedyEmpList[getEmployeesWithShift].Name);

                if (indexOfWorker == -1 &&
                    schedulare.Days[RunningOnTheDay].Shifts[RunOnShifts].Workers.Count < shiftsContainer.ShiftParams.NumberOfWokersInShift &&
                    greedyEmpList[getEmployeesWithShift].NumberOfShiftsThatWasAssigned < shiftsContainer.ShiftParams.NumberOfDaysOfWork)
                {
                    schedulare.Days[RunningOnTheDay].Shifts[RunOnShifts].Workers.Add(new Worker() { Name = greedyEmpList[getEmployeesWithShift].Name });
                    greedyEmpList[getEmployeesWithShift].NumberOfShiftsThatWasAssigned++;

                }
                else if (indexOfWorker != -1 &&
                    schedulare.Days[RunningOnTheDay].Shifts[RunOnShifts].Workers.Count < shiftsContainer.ShiftParams.NumberOfWokersInShift &&
                    greedyEmpList[getEmployeesWithShift].NumberOfShiftsThatWasAssigned < shiftsContainer.ShiftParams.NumberOfDaysOfWork)
                {
                    //need to switch users
                    if (0 < RunningOnTheDay)
                        for (int getSiz = RunningOnTheDay; 0 < getSiz; getSiz--)
                        {
                            var newGet = schedulare.Days[getSiz].Shifts[RunOnShifts].Workers.FindIndex(x => x.Name == greedyEmpList[getEmployeesWithShift].Name);
                            if (newGet == -1)
                            {
                                schedulare.Days[RunningOnTheDay].Shifts[RunOnShifts].Workers.Add(new Worker() { Name = schedulare.Days[getSiz].Shifts[RunOnShifts].Workers[0].Name });
                                schedulare.Days[getSiz].Shifts[RunOnShifts].Workers[0].Name = greedyEmpList[getEmployeesWithShift].Name;

                                greedyEmpList[getEmployeesWithShift].NumberOfShiftsThatWasAssigned++;
                                break;
                            }
                        }


                }
            }
        }



        private void PrintAll()
        {
            foreach (var x in schedulare.Days)
            {
                Console.WriteLine(x.Name + " : ");
                Console.WriteLine("-----------");
                foreach (var y in x.Shifts)
                {
                    Console.WriteLine(y.Name + " : ");

                    foreach (var z in y.Workers)
                    {
                        Console.WriteLine(z.Name);
                    }

                }
            }
        }



    }
}
