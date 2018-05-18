using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System;

namespace scribble.Data
{
    public class Helpers
    {
        public static string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];

        public static IEnumerable<T> GetData<T>(string SQLQuery, Func<IDataRecord, T> BuildObject)
        {
            try
            {
                SqlAccess.Log(SQLQuery);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SQLQuery, conn))
                    {
                        using (IDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                yield return BuildObject(rdr);
                            }
                        }

                    }
                    conn.Close();
                }
            }
            finally
            {
                SqlAccess.Log(SQLQuery);
            }
        }
    }

    public class SqlAccess
    {
        public static string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];

        public static IDataReader GetReader(string SQLQuery)
        {
            Log(SQLQuery);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(SQLQuery, conn))
                {
                    using (IDataReader rdr = cmd.ExecuteReader())
                    {
                        conn.Close();
                        return rdr;
                    }
                }
            }
        }

        public static void ExecuteNonQuery(string SQLQuery)
        {
            Log(SQLQuery);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(SQLQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static object ExecuteScalar(string SQLQuery)
        {
            Log(SQLQuery);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(SQLQuery, conn))
                {
                    return cmd.ExecuteScalar();
                }
            }
        }

        public static void Log(string SQLQuery)
        {
            //write out sql
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    conn.Open();
            //    using (SqlCommand cmd = new SqlCommand("insert into queries(query) values('" + Common.Utilities.CleanSQL(SQLQuery) + "')", conn))
            //    {
            //        cmd.ExecuteNonQuery();
            //        conn.Close();
            //    }               
            //}          
        }
    }
}