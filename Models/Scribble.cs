using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scribble.Data;
using System.Data;
using System.Data.SqlClient;
using scribble.Common;
using Dapper;

namespace scribble.Models
{
    public class Scribble
    {
        public int id { get; set; }
        public int id_author { get; set; }
        public User author { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string identifier { get; set; }
        public IList<Version> versions { get; set; }
        public int viewcount { get; set; }
        public int likecount { get; set; }

        public Scribble() { }

        public int Add()
        {
            var ssql = @"insert into scribble(id_user, title, description) values({0},'{1}','{2}') 
                         select @@identity";

            ssql = string.Format(ssql, this.id_author.ToString(), this.title.CleanSQL(), this.description.CleanSQL());
            return int.Parse(SqlAccess.ExecuteScalar(ssql).ToString());            
        }

        public static Scribble Get(string id_scribble)
        {
            var ssql = @"select * from scribble 
                        where id = " + id_scribble.ToString();

            var scribble = Helpers.GetData<Scribble>(ssql, Create).First();
            return scribble;
        }


        public static IList<Scribble> GetScribblesByUser(int id_user)
        {
            var ssql = @"select * from scribble 
                        where id_user = " + id_user.ToString() + " order by id desc";

            var scribbles = Helpers.GetData<Scribble>(ssql, Create).ToList();
            return scribbles;

        }
        public static Scribble Create(IDataRecord dr)
        {
            return new Scribble
            {
                id = (int)dr["id"],
                id_author = (int)dr["id_user"],
                author = User.Get((int)dr["id_user"]),
                title = dr["title"].ToString(),
                description = dr["description"].ToString(),
                identifier = String.Format("{0:X}", (int)dr["id"].ToString().GetHashCode()),
                versions = Version.GetVersions((int)dr["id"]),
                viewcount = View.GetViewCount((int)dr["id"]),
                likecount = Like.GetLikeCount((int)dr["id"])
            };
        }       

    }
}