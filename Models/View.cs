using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scribble.Data;

namespace scribble.Models
{
    public class View
    {
        public int id { get; set; }
        public DateTime timestamp { get; set; }
        public int id_scribble { get; set; }
        public string ip { get; set; }
        public string useragent { get; set; }
        public string sessionid { get; set; }

        public View() { }

        public void Save(){
            string ssql = "insert into views(id_scribble, ip, useragent, sessionid) values('" + this.id_scribble.ToString() + "', '" + this.ip + "','" + this.useragent + "','" + this.sessionid + "') ";
            SqlAccess.ExecuteNonQuery(ssql);
        }

        public static int GetViewCount(int id_scribble)
        {
            var ssql = @"select count(distinct sessionid) from views 
                        where id_scribble = " + id_scribble.ToString();

            int viewcount = (int)SqlAccess.ExecuteScalar(ssql);
            return viewcount;
        }        
    }
}