using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Threading;

namespace SQL_Sync
{
    class Syncronizer : SQLUtil
    {
        string[] targets;
        List<AnimeSerie> series = new List<AnimeSerie>();
        

        public Syncronizer(string[] targets)
        {
            this.targets = targets;
            //dropAllTables();
            validateTargets();
            createSeriesList();
            updateDataBase();

            
        }


        //fills up the "series" list with objects
        private void createSeriesList()
        {
            foreach (string target in targets)
            {
                foreach (string dirPath in Directory.GetDirectories(target))
                {
                    if (!(dirPath.Contains("_Finished Airing")))
                    {
                        series.Add(new AnimeSerie(dirPath));
                    }
                }
            }
        }
        private void updateDataBase()
        {
            foreach (AnimeSerie a in series)
            {
                if (!tableExists(a.getName()))
                {
                    createTable(a.getName());
                }

                foreach (episode ep in a.getEpisodes())
                {
                    if (!ep.Exists() && ep.getName() != "Thumbs.db")
                    {
                        ep.addToTable();
                    }
                }
            }
        }
        private void validateTargets()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (!(Directory.Exists(targets[i])))
                {
                    Console.WriteLine("Removing Target: " + targets[i]);
                    targets[i] = "";
                }
            }


        }

        /*##############- SQL -#################*/

        /*
        private void createTable(string name)
        {
            query("CREATE TABLE [" + name + "]" + tableFormat_With_Type);

            //"CREATE TABLE [" + folderName + "](Episode int, Name varchar(255) , Path varchar(255), Added varchar(255) , Status BIT)"
        }
        private void insert(string table, string name, string path, string status)
        {
            query("INSERT INTO [" + table + "] " + tableFormat_Without_Type + "VALUES ('" + name + "' ," + " '" + path + "' , " + "'" + status + "')");
        }
        
        private Boolean tableExists(string tableName)
        {
            //Console.WriteLine("Checking if following Table Exists: " + tableName);
            foreach (string s in getColumn("sys.Tables", "name"))
            {
                if (s == tableName) 
                {
                    Console.WriteLine("The Following Table Already Exists: " + tableName);
                    return true;
                }
            }
            return false;
        }
        private Boolean EpExists(string EpName, string table)
        {
            Console.WriteLine("Checking if " + EpName + " Exists in " + table);
            foreach (string s in getColumn( "dbo.[" + table + "]" , "Name"))
            {
                if (EpName == s) { return true; }
            }
            return false;
        }
        private string[] getColumn(string from, string column)
        {
            SqlConnection con = createConnection();
            List<string> tableNames = new List<string>();
            con.Open();
            Console.WriteLine("Performing Query: " + "SELECT * FROM " + from);
            SqlDataReader reader = new SqlCommand("SELECT * FROM " + from, con).ExecuteReader();
            while (reader.Read())
            {
                tableNames.Add(reader[column].ToString());
            }
            con.Close();
            return tableNames.ToArray();
        }
        private SqlConnection createConnection()
        {
            SqlConnection con = new SqlConnection("server=10.47.1.174;Integrated Security=SSPI; database=GTG_Website");
            if (con == null)
            {
                Console.WriteLine("Could not Crate SQL Connection");
                return null;
            }
            return con;
        }
        private void insertEpisode(string table, int EpNr, string Name, string Path, string DateAdded, string Status)
        {
            string[] arr = { EpNr.ToString(), Name, Path, DateAdded, Status };
            string values = "";
            for (int i = 0; i < arr.Length; i++)
            {
                values += "'" + arr[i] + "' , ";
            }
            values = values.Substring(0, values.Length - 2);
            string cmd = "INSERT INTO [" + table + "] (Episode , Name , Path , Added , Status) VALUES" + "(" + values + ")";
            query(cmd);
        }
        public void query(string query)
        {
            SqlConnection con = createConnection();
            con.Open();
            new SqlCommand(query, con).ExecuteNonQuery();
            Console.WriteLine("Performing Query: " + query);
            con.Close();
        }
        

        



        
        private void printArray(string[] arr)
        {
            foreach (string s in arr)
            {
                Console.WriteLine(s);
            }
        }
    }
 */

    }
    class AnimeSerie
    {
        private string name = ""; //name of the folder
        private string path = ""; //path to that folder
        private List<episode> episodes = new List<episode>();

        public AnimeSerie(string folderLocation)
        {
            this.path = folderLocation;
            this.name = path.Substring(path.LastIndexOf(@"\") + 1);
            addEpisodes();
            
        }


        /*#############- Public Functions -##############*/
        public string getName()
        {
            return name;
        }
        public string getPath()
        {
            return path;
        }
        public List<episode> getEpisodes()
        {
            return episodes;
        }
        /*#############- Private Functions -##############*/
        private void addEpisodes()
        {
            foreach (string EpPath in Directory.GetFiles(path))
            {
                string EpName = EpPath.Substring(EpPath.LastIndexOf(@"\") + 1);
                //Console.WriteLine("Adding Episode: " + EpName);
                episodes.Add(new episode(EpName , EpPath , "N/A" , name));
            }
        }
        /*#############- SubClasses -##############*/
    }
    class episode : SQLUtil
    {
        private string name = "";
        private string path = "";
        private string quality = "";
        private string table = "";

        public episode(string name, string path, string quality , string table)
        {
            this.name = name;
            this.path = path;
            this.quality = quality;
            this.table = table;
        }

        /*---Methodes---*/
        public void addToTable()
        {
            insert(table, this.name, this.path, "0");
        }
        
        //---Getters
        public string getName()
        {
            return name;
        }
        public string getPath()
        {
            return path;
        }
        public string getQuality()
        {
            return quality;
        }
        public string getTable()
        {
            return table;
        }
        public Boolean Exists()
        {
            return EpExists(name, table);
        }

    }

    /*---Abstract Classes--*/
    abstract class SQLUtil
    {
        string tableFormat_With_Type = "(Name varchar(150) , Path varchar(500) , Status BIT)";
        string tableFormat_Without_Type = "(Name , Path , Status )";

        protected SqlConnection createConnection()
        {
            SqlConnection con = new SqlConnection("server=10.47.1.174;Integrated Security=SSPI; database=GTG_Website");
            if (con == null)
            {
                Console.WriteLine("Could not Crate SQL Connection");
                return null;
            }
            return con;
        }
        protected void createTable(string name)
        {
            query("CREATE TABLE [" + name + "]" + tableFormat_With_Type);

            //"CREATE TABLE [" + folderName + "](Episode int, Name varchar(255) , Path varchar(255), Added varchar(255) , Status BIT)"
        }
        protected void insert(string table, string name, string path, string status)
        {
            query("INSERT INTO [" + table + "] " + tableFormat_Without_Type + "VALUES ('" + name + "' ," + " '" + path + "' , " + "'" + status + "')");
        }
        protected void query(string query)
        {
            SqlConnection con = createConnection();
            con.Open();
            new SqlCommand(query, con).ExecuteNonQuery();
            //Console.WriteLine("Performing Query: " + query);
            con.Close();
        }
        protected string[] getColumn(string from, string column)
        {
            SqlConnection con = createConnection();
            List<string> tableNames = new List<string>();
            con.Open();
            Console.WriteLine("Performing Query: " + "SELECT * FROM " + from);
            SqlDataReader reader = new SqlCommand("SELECT * FROM " + from, con).ExecuteReader();
            while (reader.Read())
            {
                tableNames.Add(reader[column].ToString());
            }
            con.Close();
            return tableNames.ToArray();
        }
        protected Boolean tableExists(string tableName)
        {
            //Console.WriteLine("Checking if following Table Exists: " + tableName);
            foreach (string s in getColumn("sys.Tables", "name"))
            {
                if (s == tableName)
                {
                    Console.WriteLine("The Following Table Already Exists: " + tableName);
                    return true;
                }
            }
            return false;
        }
        protected Boolean EpExists(string EpName, string table)
        {
            Console.WriteLine("Checking if " + EpName + " Exists in " + table);
            foreach (string s in getColumn("dbo.[" + table + "]", "Name"))
            {
                if (EpName == s) { return true; }
            }
            return false;
        }
        protected void dropAllTables()
        {
            foreach(string table in getColumn("sys.Tables" , "name"))
            {
                query("DROP TABLE dbo.[" + table + "]");
            }
        }

    }
}

