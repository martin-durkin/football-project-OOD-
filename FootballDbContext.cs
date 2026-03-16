using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace football_project
{
    public class FootballDbContext : DbContext
    {
        // FootballFavourites is the name of the database file that gets created
        public FootballDbContext() : base("FootballFavourites") { }

        public DbSet<FavouritePlayer> FavouritePlayers { get; set; }
    }
}
