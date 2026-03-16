using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace football_project
{
    public abstract class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }


        public Person() { }

        public Person(int id, string name)
        {
            ID = id;
            Name = name;

        }


        // GetSummary() is abstract which means every class that inherits Person is forced to write their own version of it
        public abstract string GetSummary();
        

    }
}
