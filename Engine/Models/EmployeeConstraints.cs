using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Models
{
    public class EmployeeConstraints
    {
        public string Name { get; set; }
        public Dictionary<string, string> WeeklyConstraints { get; set; }

        public EmployeeConstraints(string name)
        {
            Name = name;

            WeeklyConstraints = new Dictionary<string, string>(){
                    {"Sunday","Free day"},
                    {"Monday","Free day"},
                    {"Tuesday","Free day"},
                    {"Wednesday","Free day"},
                    {"Thursday","Free day"},
                    {"Friday","Free day"},
                    {"Saturday","Free day"}
                    };
        }
    }
}
