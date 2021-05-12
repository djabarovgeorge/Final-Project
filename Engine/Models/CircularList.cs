using Engine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Models
{
    public class CircularList
    {
        public List<Schedulare> ItemList { get; set; }
        private int _size;

        public CircularList(int size)
        {
            ItemList = new List<Schedulare>();
            _size = size;
        }

        public void Add(Schedulare item)
        {
            if (ItemList.Count.Equals(_size))
                ItemList.Remove(ItemList.FirstOrDefault());

            ItemList.Add(item);
        }

        public bool Contains(Schedulare schedulare)
        {
            var isContains = true;

            foreach (var itemSchedulare in ItemList)
            {
                isContains = true;

                foreach (var day in itemSchedulare.Days)
                {
                    foreach (var shift in day.Shifts)
                    {
                        bool isDiff = IfDifferenceInShift(schedulare, day, shift);
                        if (isDiff)
                        {
                            isContains = false;
                            break;
                        }
                    }
                    if (!isContains)
                        break;
                }
                if (isContains)
                    return true;
            }
            return false;
        }

        private static bool IfDifferenceInShift(Schedulare schedulare, Day day, Shift shift)
        {
            var inputDay = schedulare.Days.FirstOrDefault(x => x.Name.CompareContent(day.Name));
            var inputShift = inputDay.Shifts.FirstOrDefault(x => x.Name.CompareContent(shift.Name));
            var isDiff = !shift.Workers.Count.Equals(inputShift.Workers.Count) ||
                !shift.Workers.All(x => inputShift.Workers.Any(y => y.Name.CompareContent(x.Name)));
            return isDiff;
        }
    }
}
