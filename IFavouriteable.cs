using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace football_project
{
    //any class that can be favourited must implement this interface
    public interface IFavouriteable
    {
        int ID { get; set; }
        string Name { get; set; }
        string GetSummary();
    }
}
