using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public static class Postgre
    {
        public static NpgsqlConnection Connection = null;
        public static User UserInfo = null;
        public static DiscountSystem Discounts = null;
        public static EquipmentTypes Types = null;
        public static EquipmentList Equipments = null;
        public static Clients ClientsInfo = null;


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
            string query = $"SELECT \"UserInfo\".\"Id\" FROM \"UserInfo\" WHERE \"Login\"='{login}' AND \"Password\"='{password}'";
            NpgsqlCommand Command = new NpgsqlCommand(query, Connection);
            //NpgsqlDataReader reader;
            var reader = Command.ExecuteScalar();
            if (reader == null)
                return false;
            else
                return true;
        }

        public static void ExecuteNonQuery(string query)
        {
            NpgsqlCommand Command = new NpgsqlCommand(query, Connection);
            Command.ExecuteNonQuery();
        }

        public static void LoadClientsList()
        {
            if (Discounts == null) {
                Discounts = new DiscountSystem();
                string queryDiscounts = "SELECT \"Sum\", \"Size\" FROM \"Discount\";";
                NpgsqlCommand CommandDiscounts = new NpgsqlCommand(queryDiscounts, Connection);
                var readerDiscounts = CommandDiscounts.ExecuteReader();
                if (readerDiscounts.HasRows) {
                    while (readerDiscounts.Read()) {
                        Discounts.AddDiscount(readerDiscounts.GetDouble(0), readerDiscounts.GetDouble(1));
                    }
                }
                readerDiscounts.Close();
            }

            ClientsInfo = new Clients();
            string queryClients = "SELECT \"Id\", \"Name\", \"DateBirth\", \"Phone\", \"Sum\" FROM \"Client\";";
            NpgsqlCommand CommandClients = new NpgsqlCommand(queryClients, Connection);
            var readerClients = CommandClients.ExecuteReader();
            if (readerClients.HasRows) {
                while (readerClients.Read()) {
                    ClientsInfo.AddClient(readerClients["Id"].ToString(), readerClients["Name"].ToString(), readerClients["DateBirth"].ToString(), readerClients["Phone"].ToString(), readerClients["Sum"].ToString(), Discounts);
                    Console.WriteLine(readerClients["DateBirth"].ToString());
                }
            }
            readerClients.Close();
        }

        public static void LoadEquipmentList()
        {
            if (Types == null) {
                Types = new EquipmentTypes();
                string queryTypes = "SELECT \"Id\", \"TypeName\", \"EPrice\", \"Description\" FROM \"EquipmentType\";";
                NpgsqlCommand Command1 = new NpgsqlCommand(queryTypes, Connection);
                var reader1 = Command1.ExecuteReader();
                if (reader1.HasRows) {
                    while (reader1.Read()) {
                        Types.AddType(reader1.GetInt32(0), reader1.GetString(1), reader1.GetDouble(2), reader1.GetString(3));
                    }
                }
                reader1.Close();
            }

            Equipments = new EquipmentList();
            string queryEquipment = "SELECT \"Id\", \"Status\", \"Location\", \"IdType\" FROM \"Equipment\";";
            NpgsqlCommand Command2 = new NpgsqlCommand(queryEquipment, Connection);
            var reader2 = Command2.ExecuteReader();
            if (reader2.HasRows) {
                while (reader2.Read()) {
                    Equipments.AddEquipment(reader2["Id"].ToString(), reader2["Status"].ToString(), reader2["Location"].ToString(), reader2["IdType"].ToString(), Types);
                }
            }
            reader2.Close();
        }

        public static DataTable LoadMembershipsList()
        {
            DataTable dt = new DataTable();
            string query = "SELECT \"Membership\".\"Id\", \"MembershipType\".\"Price\", \"MembershipType\".\"Duration\", \"Membership\".\"Activated\", \"Membership\".\"Used\" FROM \"Membership\", \"MembershipType\" WHERE \"Membership\".\"Type\" = \"MembershipType\".\"Id\";";
            NpgsqlCommand command = new NpgsqlCommand(query, Connection);
            using (NpgsqlDataAdapter reader = new NpgsqlDataAdapter(command)) {
                reader.Fill(dt);
            }

            dt.Columns[0].ColumnName = "ID";
            dt.Columns[1].ColumnName = "Цена (руб.)";
            dt.Columns[2].ColumnName = "Сеанс (мин.)";
            dt.Columns[3].ColumnName = "Дата продажи";
            dt.Columns[4].ColumnName = "Дата использования";
            
            return dt;
        }

        //public static DataTable LoadEquipmentList()
        //{
        //    DataTable dt = new DataTable();
        //    string query =
        //        "SELECT \"Equipment\".\"Id\", \"EquipmentType\".\"EPrice\", \"Equipment\".\"Status\", \"EquipmentType\".\"TypeName\", \"Equipment\".\"Location\" FROM \"Equipment\", \"EquipmentType\" WHERE \"Equipment\".\"IdType\" = \"EquipmentType\".\"Id\";";
        //    NpgsqlCommand command = new NpgsqlCommand(query, Connection);
        //    using (NpgsqlDataAdapter reader = new NpgsqlDataAdapter(command)) {
        //        reader.Fill(dt);
        //    }

        //    dt.Columns[0].ColumnName = "ID";
        //    dt.Columns[1].ColumnName = "Цена (руб.)";
        //    dt.Columns[2].ColumnName = "Статус";
        //    dt.Columns[3].ColumnName = "Тип";
        //    dt.Columns[4].ColumnName = "Расположение";

        //    return dt;
        //}
        
        public static void CloseConnection()
        {
            Connection.Close(); 
        }
    }
}
