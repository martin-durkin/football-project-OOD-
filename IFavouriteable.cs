using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace football_project
{
    public interface IFavouriteable
    {
        int ID { get; set; }
        string Name { get; set; }
        string GetSummary();
    }
}
