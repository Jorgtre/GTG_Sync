using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Data.SqlTypes;
using System.Data;

namespace SQL_Sync
{
    

    class Program
    {
        /*
         * Status Values:
         * 0 = Recently Added / not watched
         * 1 = Watched
         * 2 = Error, Something is wrong!
         */

        static void Main(string[] args)
        {
            string[] targets = { @"\\server1\Private\Anime" };
            Syncronizer sync = new Syncronizer(targets);
            
        }
    }
}




