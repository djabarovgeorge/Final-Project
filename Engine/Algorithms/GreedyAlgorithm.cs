using Engine.Extensions;
using Engine.Models;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Algorithms
{
    public class GreedyAlgorithm
    {

        private List<string> week = new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        private List<string> shift = new List<string> { "Morning", "Noon", "Afternoon", "Night" };

        public void Execute(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            StartAlgorithm( schedulare,  shiftsContainer);
        }

        public GreedyAlgorithm()
        {
        }

        public void StartAlgorithm(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            var greedyEmpList = new List<GreedyEmployee>();

            foreach (var emp in shiftsContainer.EmployeeConstraints)
            {
                var currEmpName = new Worker() { Name = emp.Name };

                var greedyEmploee = new GreedyEmployee() { Name = emp.Name };


                foreach (var empDay in emp.WeeklyConstraints)
                {
                    if (empDay.Value.Contains("Free day")) continue;

                    var constraintsDay = empDay.Key;
                    var constraintsShift = empDay.Value;

                    var schedulareDay = schedulare.Days.FirstOrDefault(x => x.Name.Contains(constraintsDay));

                    var schedulareShift = schedulareDay.Shifts.FirstOrDefault(x => x.Name.Contains(constraintsShift));

                    if(schedulareShift.Workers.Count < shiftsContainer.ShiftParams.NumberOfWokersInShift)
                    {
                        greedyEmploee.AddShift();
                        schedulareShift.Workers.Add(currEmpName);
                    }
                    else
                    {
                        var shift = new EmployeeShift() { Day = constraintsDay , ShiftName = constraintsShift };

                        greedyEmploee.ShiftsThatWasNotAssigned.Add(shift);
                    }
                }
                greedyEmpList.Add(greedyEmploee);
            }

                
            var conflicts = greedyEmpList.Where(x => x.ShiftsThatWasNotAssigned.Count > 0).ToList();

            foreach(var employeeCheck in conflicts)
            {
                if(employeeCheck.ShiftsThatWasNotAssigned.Count > 1)
                {
                    foreach(var iDontHaveThisDayList in employeeCheck.ShiftsThatWasNotAssigned)
                    {
                        var shiftDay = schedulare.Days.FirstOrDefault(x => x.Name.ContainsContent(iDontHaveThisDayList.Day));
                        var shift = shiftDay.Shifts.FirstOrDefault(x => x.Name.CompareContent(iDontHaveThisDayList.ShiftName));
                        //schedulare.Days[week.IndexOf(iDontHaveThisDay.Day)];
                        var currShiftGreedyWorkers = shift.Workers.Select(x => greedyEmpList.FirstOrDefault(y => x.Name.ContainsContent(y.Name))).ToList();

                        var luckyWorks = currShiftGreedyWorkers.OrderBy(x => x.ShiftsThatWasNotAssigned.Count()).LastOrDefault();

                        //if greater then some value

                        //if the ShiftsThatWasNotAssigned is the same



                    }


                    // if (newEployw.NumberOfShiftsThatWasAssigned == shiftsContainer.EmployeeConstraints[])
                }
            }

        }
    }




}
