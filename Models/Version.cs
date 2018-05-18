using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using scribble.Data;
using scribble.Common;

namespace scribble.Models
{
    public class Version
    {
        public int id { get; set; }
        public DateTime timestamp { get; set; }
        public int scribble_id { get; set; }
        public string description { get; set; }
        public string filename { get; set; }

        public Version() { }

        public void Add()
        {
            var ssql = @"insert into version(id_scribble, description, filename) values({0},'{1}','{2}')";

            ssql = string.Format(ssql, this.scribble_id.ToString(), this.description.CleanSQL(), this.filename.CleanSQL());
            SqlAccess.ExecuteNonQuery(ssql);
        }

        public static Version Get(string id)
        {
            var ssql = @"select * from version 
                        where id = " + id.ToString();

            var version = Helpers.GetData<Version>(ssql, Create).First();
            return version;
        }


        public static IList<Version> GetVersions(int id_scribble)
        {
            var ssql = @"select * from version 
                        where id_scribble = " + id_scribble.ToString() + " order by id desc";

            var versions = Helpers.GetData<Version>(ssql, Create).ToList();
            return versions;
        }

        public static Version Create(IDataRecord dr)
        {
            return new Version
            {
                id = (int)dr["id"],
                timestamp = (DateTime)dr["timestamp"],
                scribble_id = (int)dr["id_scribble"],
                description = dr["description"].ToString(),
                filename = dr["filename"].ToString()
            };
        }
    }
}