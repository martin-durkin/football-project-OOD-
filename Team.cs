using System.Collections.Generic;

namespace football_project
{
    public class Team
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public List<Player> Players { get; set; }

        public Team() { }

        public Team(int id, string name, string gender)
        {
            ID = id;
            Name = name;
            Gender = gender;
            Players = new List<Player>();
        }

       
    }
}