namespace Engine.Models
{
    public class WeightContainer
    {
        public int TwoShiftsInARowWeight { get; set; }
        public int LackSatisfactionConstraintsWeight { get; set; }
        public int ShiftsInARow { get; set; }
        public int UnwantedShift { get; set; }
    }
}