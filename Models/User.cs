using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using scribble.Data;

namespace scribble.Models
{
    public class User
    {
        public int id { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime last_login { get; set; }            
        public string username { get; set; }
        private string password { get; set; }
        public string email { get; set; }
        public int account_type { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string biography { get; set; }

        public User() { }
        public User(int id){
            User u = User.Get(id);
            this.firstname = u.firstname;
            this.lastname = u.lastname;
            this.email = u.email;
            this.biography = u.biography;
        }

        public void SignUp()
        {
            string ssql = "insert into [user](username, password, email) values('" + this.username + "', '" + this.password + "','" + this.email + "') ";
            SqlAccess.ExecuteNonQuery(ssql);
        }

        public void UpdateProfile(){
            string ssql = @"update [user] 
                                set username = '{1}',
                                    firstname = '{2}',
                                    lastname = '{3}',
                                    biography = '{4}',
                                    email = '{5}'
                                where id={0}";

            ssql = string.Format(ssql, this.id, this.username, this.firstname, this.lastname, this.biography, this.email);
            SqlAccess.ExecuteNonQuery(ssql);
        }

        public static int Login(string username, string password)
        {
            string sql = @"select * from [user] where username='" + username + "' and password='" + password + "'";
            var user = Helpers.GetData<User>(sql, Create);
            if (user.Count() > 0)
            {
                return user.First().id;
            }
            else
            {
                return 0;
            }
            
        }

        public static User Get(int id_user)
        {
            var ssql = @"select * from [user] 
                        where id = " + id_user.ToString();

            var user = Helpers.GetData<User>(ssql, Create).First();
            return user;
        }


        public static User Create(IDataRecord dr)
        {
            return new User
            {
                id = (int)dr["id"],
                timestamp = (DateTime)dr["timestamp"],
                username = dr["username"].ToString(),
                email = dr["email"].ToString(),
                firstname = dr["firstname"].ToString(),
                lastname = dr["lastname"].ToString(),
                biography = dr["biography"].ToString()
            };
        }
    }
}