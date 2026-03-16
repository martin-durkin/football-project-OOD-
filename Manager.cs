using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace football_project
{
    public class Manager: Person
    {
        public string Nationality { get; set; }
        public string TeamName { get; set; }

        public Manager() { }

        public Manager(int id, string name, string nationality, string teamName) 
            : base(id, name)
        {
            Nationality = nationality;
            TeamName = teamName;
        }

        public override string GetSummary()
        {
            return $"{Name} - Manager of {TeamName} ({Nationality})";
        }

        public override string ToString()
        {
            return $"{Name} - ({Nationality} - {TeamName})";
        }
    }
}
