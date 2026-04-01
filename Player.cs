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

        //returns a sort order number for a given position string
        // Moved here from MainWindow so it can be unit tested independently
        public static int GetPositionOrder(string position)
        {
            switch (position)
            {
                case "GK": return 1;
                case "DEF": return 2;
                case "MID": return 3;
                case "FWD": return 4;
                default: return 5;
            }
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