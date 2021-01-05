using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Models
{
    public class Employee
    {
        public Dictionary<string, string> WeeklyConstraints { get; set; }

        public Employee()
        {
            WeeklyConstraints = new Dictionary<string, string>(){
                    {"Sundey","Free day"},
                    {"Monday","Free day"},
                    {"Tuesday","Free day"},
                    {"Wednesday","Free day"},
                    {"Thursday","Free day"},
                    {"Friday","Free day"},
                    {"Saturdaty","Free day"}
                    };
        }
    }
}
