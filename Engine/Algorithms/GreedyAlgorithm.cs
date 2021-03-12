using Engine.Models;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Algorithms
{
    public class GreedyAlgorithm
    {

        public void Execute(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            StartAlgorithm( schedulare,  shiftsContainer);
        }

        public GreedyAlgorithm()
        {
        }

        public void StartAlgorithm(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            List<GreedyEmployee> greedyEmpList = new List<GreedyEmployee>();

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

        }
    }
}
