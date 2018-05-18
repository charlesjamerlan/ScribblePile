using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scribble.Data;

namespace scribble.Models
{
    public class Like
    {
        public DateTime timestamp { get; set; }
        public int id_scribble { get; set; }
        public int id_user { get; set; }

        public Like() { }

        public static bool IsLiked(int id_scribble, int id_user)
        {
            string ssql = "select count(*) from likes where id_scribble = " + id_scribble.ToString() + " and id_user=" + id_user.ToString();
            return ((int)SqlAccess.ExecuteScalar(ssql) == 0) ? false : true;
        }

        public static int GetLikeCount(int id_scribble)
        {
            var ssql = @"select count(*) from likes 
                        where id_scribble = " + id_scribble.ToString();

            int likecount = (int)SqlAccess.ExecuteScalar(ssql);
            return likecount;
        }      
        public static void SaveLike(int id_scribble, int id_user)
        {
            string ssql = "if not exists (select id_scribble from likes where id_scribble = " + id_scribble.ToString() + " and id_user=" + id_user.ToString() + ") insert into likes(id_scribble, id_user) values(" + id_scribble.ToString() + ", " + id_user.ToString() + ") ";
            SqlAccess.ExecuteNonQuery(ssql);
        }

        public static void RemoveLike(int id_scribble, int id_user)
        {
            string ssql = "delete from likes where id_scribble = " + id_scribble.ToString() + " and id_user=" + id_user.ToString();
            SqlAccess.ExecuteNonQuery(ssql);
        }       

    }
}