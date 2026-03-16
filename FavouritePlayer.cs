using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace football_project
{
    public class FavouritePlayer
    {
        public int FavouritePlayerID { get; set; }  // EF needs a primary key
        public int PlayerID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string TeamName { get; set; }
        public int TeamID { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
