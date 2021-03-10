//using Engine.Abstracts;
//using Engine.Models;
//using Engine.Services;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Algorithms
//{
//    class GreedyAlgorithm
//    {

//        private List<string> randDay = new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
//        private Dictionary<string, Dictionary<string, List<string>>> schedule = new Dictionary<string, Dictionary<string, List<string>>>();
//        private string[] typesOfShifts = { "Morning", "Afternoon", "Nigth" };
//        private ShiftSetter shiftSetter = new ConstructsShift();
//        private int numberOfEmployess;
//        private int workingDaysPerWeek;
//        private int theAmountOfShiftsPerDay;
//        private List<EmployeeConstraints> listOfEmployess;//change name

//        public GreedyAlgorithm()
//        {
//            this.listOfEmployess = shiftSetter.Excute();

//        }

//        public GreedyAlgorithm(int numberOfEmployess ,int workingDaysPerWeek ,int theAmountOfShiftsPerDay)
//        {
//            this.numberOfEmployess = numberOfEmployess;
//            this.workingDaysPerWeek = workingDaysPerWeek;
//            this.theAmountOfShiftsPerDay = theAmountOfShiftsPerDay;
//            this.listOfEmployess = shiftSetter.Excute();
//        }
//        public void StartAlgorithm()
//        {
//            for (int i = 0; i < listOfEmployess.Count; i++)
//            {
//                Console.WriteLine($"1212Empyloee Number: {i + 1} -------------------------");
//                foreach (var dayOfWork in listOfEmployess[i].WeeklyConstraints)
//                {
//                    switch (dayOfWork.Key)
//                    {
//                        case "Sunday":

//                            break;
//                        case "Monday":
//                            break;
//                        case "Tuesday":
//                            break;
//                        case "Wednesday":
//                            break;
//                        case "Thursday":
//                            break;
//                        case "Friday":
//                            break;
//                        case "Saturday":
//                            break;

//                    }
                        
//                    Console.WriteLine($"{dayOfWork.Key} : {dayOfWork.Value}");
//                }
//            }
//        }


//    }
//}
