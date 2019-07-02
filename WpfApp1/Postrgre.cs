using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public static class Postrgre
    {
        public static NpgsqlConnection Connection = null;

        public static void OpenConnection()
        {
            string conn_param = "Server=localhost;Port=5432;User Id=username;Password=password;Database=postgres;";
            //string sql = "текст запроса к базе данных";
            Connection = new NpgsqlConnection(conn_param);
            //NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            Connection.Open(); 
            
        }

        public static bool AutorizeUser (string login, string password)
        {
            OpenConnection();
            string query = $"SELECT \"UserInfo\".\"Id\" FROM \"UserInfo\" WHERE \"Login\"='{login}' AND \"Password\"='{password}'";
            NpgsqlCommand Command = new NpgsqlCommand(query, Connection);
            //NpgsqlDataReader reader;
            var reader = Command.ExecuteScalar();
            CloseConnection();
            if (reader == null)
                return false;
            else
                return true;
        }

        public static void CloseConnection()
        {
            Connection.Close(); 
        }
    }
}
