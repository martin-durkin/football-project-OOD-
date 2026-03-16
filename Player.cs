using Newtonsoft.Json;
using System;

namespace football_project
{
    public class Player : Person, IFavouriteable  //Player inherits from Person and also implements the interface
    {

        public string Position { get; set; }
        public string TeamName { get; set; }
        public int TeamID { get; set; }
        public DateTime DateAdded { get; set; }


        public Player()
        {

        }


        public Player(int id, string name, string position, string teamName, int teamId)
            : base(id, name)
        {

            Position = position;
            TeamName = teamName;
            TeamID = teamId;
            DateAdded = DateTime.Now;
        }

        public override string GetSummary()
        {
            return $"{Name} - {Position} ({TeamName})";
        }

        public override string ToString()
        {
            return $"{Name} - {Position} ({TeamName})";
        }
    }
}