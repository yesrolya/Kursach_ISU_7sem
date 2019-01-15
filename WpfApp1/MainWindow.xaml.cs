using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    //РАБОТА С БАЗОЙ ДАННЫХ
    class BD
    {
        private static SQLiteConnection connection;
        private static User current_user = null;

        public static void CreateConnection()
        {
            connection = new SQLiteConnection("Data Source=bdkursach.db");
            connection.Open();
        }

        //АВТОРИЗАЦИЯ
        public static bool Login(string login, string password)
        {
            int adminId = -1;
            string autorizationRequest = $"SELECT id FROM Autorization WHERE login = '{login}' AND password = '{password}';";
            SQLiteCommand command = new SQLiteCommand(autorizationRequest, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                adminId = reader.GetInt32(0);
            }
            reader.Close();

            var userRequest = $"SELECT id_admin, name_admin, phone_admin FROM Admin WHERE id_admin = '{adminId}';";
            command = new SQLiteCommand(userRequest, connection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                current_user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
            }
            reader.Close();

            if (current_user == null)
                return false;
            return true;
        }

        //РАБОТА СО СКИДОЧНОЙ СИСТЕМОЙ
        public static DataTable DiscountShow()
        {
            string discountRequest = $"SELECT * FROM Discount_system;";
            SQLiteCommand command = new SQLiteCommand(discountRequest, connection);
            DataTable dt = new DataTable();
            using (SQLiteDataAdapter reader = new SQLiteDataAdapter(command))
            {
                reader.Fill(dt);
            }
            dt.Columns[0].ColumnName = "ID";
            dt.Columns[1].ColumnName = "Размер скидки";
            dt.Columns[2].ColumnName = "Минимальная сумма";
            return dt;
        }
        public static void DiscountDelete(int idDiscount)
        {
            string discountRequest = $"DELETE FROM Discount_system WHERE id_discount = '{idDiscount}';";
            SQLiteCommand command = new SQLiteCommand(discountRequest, connection);
            command.ExecuteNonQuery();
        }
        public static void DiscountAdd(int discountSize, int minSumm)
        {
            string discountRequest = $"INSERT INTO Discount_system(size_discount, min_summ) VALUES ('{discountSize}', '{minSumm}');";
            SQLiteCommand command = new SQLiteCommand(discountRequest, connection);
            command.ExecuteNonQuery();
        }

        //РАБОТА С ГРАФИКОМ
        public static DataTable TimeModeShow()
        {
            string dateRequest =
                $"SELECT Date_schedule.date_schedule, " +
                    $"Time_mode.time_start, " +
                    $"Time_mode.time_end, " +
                    $"Time_mode.name_mode " +
                    $"FROM Date_schedule, Time_mode " +
                    $"WHERE Date_schedule.id_schedule = Time_mode.id_schedule;";
            SQLiteCommand command = new SQLiteCommand(dateRequest, connection);
            DataTable dt = new DataTable();
            using (SQLiteDataAdapter reader = new SQLiteDataAdapter(command))
            {
                reader.Fill(dt);
            }
            dt.Columns[0].ColumnName = "Дата";
            dt.Columns[1].ColumnName = "Начало";
            dt.Columns[2].ColumnName = "Окончание";
            dt.Columns[3].ColumnName = "Режим работы";
            return dt;
        }
        public static void TimeModeAdd(string date, int mode)
        {
            string dateRequest = $"INSERT INTO Date_schedule(id_schedule, date_schedule) VALUES ('{mode}', '{date}');";
            SQLiteCommand command = new SQLiteCommand(dateRequest, connection);
            command.ExecuteNonQuery();
        }

        //РАБОТА С ГРАФИКОМ
        public static DataTable EquipmentShow()
        {
            string equipmentRequest =
                $"SELECT Equipment.id_equipment, " +
                    $"Type_equipment.type_name, " +
                    $"Type_equipment.type_description, Type_equipment.cost, " +
                    $"Status_equipment.name_status, " +
                    $"Location.address, " +
                    $"Location.position_code " +
                    $"FROM Equipment, Type_equipment, " +
                    $"Location, Status_equipment " +
                    $"WHERE Status_equipment.id_status = Equipment.id_status AND " +
                    $"Location.id_location = Equipment.id_location AND " +
                    $"Type_equipment.id_type = Equipment.id_type ";
            SQLiteCommand command = new SQLiteCommand(equipmentRequest, connection);
            DataTable dt = new DataTable();
            using (SQLiteDataAdapter reader = new SQLiteDataAdapter(command))
            {
                reader.Fill(dt);
            }
            dt.Columns[0].ColumnName = "ИД";
            dt.Columns[1].ColumnName = "Тип";
            dt.Columns[2].ColumnName = "Описание";
            dt.Columns[3].ColumnName = "Цена";
            dt.Columns[4].ColumnName = "Статус";
            dt.Columns[5].ColumnName = "Адрес";
            dt.Columns[6].ColumnName = "Позиция";
            return dt;
        }
    }

    class User
    {
        int id;
        string name;
        string phone;

        public User(int id, string name, string phone)
        {
            this.id = id;
            this.name = name;
            this.phone = phone;
        }
    }

    class EquipmentType
    {
        public int id;
        public string name;
        public int cost_hour;
        public int cost_day;
        public string description;

        public EquipmentType (int id, string name, int cost_hour, int cost_day, string description)
        {
            this.id = id;
            this.name = name;
            this.cost_hour = cost_hour;
            this.cost_day = cost_day;
            this.description = description;
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            BD.CreateConnection();
            InitializeComponent();
            //AutorizationPanel.Visibility = Visibility.Visible;
            //MenuPanel.Visibility = Visibility.Hidden;
            //InfoTab.Visibility = Visibility.Hidden;
        }
        //AUTORIZATION
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (BD.Login(LoginBox.Text, PasswordBox.Text))
            {
                AutorizationPanel.Visibility = Visibility.Hidden;
                MenuPanel.Visibility = Visibility.Visible;
            }
        }
        //MENU->INFO
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            InfoTab.Visibility = Visibility.Visible;
            //Discount system
            DiscountGrid.ItemsSource = BD.DiscountShow().DefaultView;
            DiscountGrid.AutoGenerateColumns = true;
            DiscountGrid.CanUserAddRows = false;
            //Time mode
            TimeModeGrid.ItemsSource = BD.TimeModeShow().DefaultView;
            TimeModeGrid.AutoGenerateColumns = true;
            TimeModeGrid.CanUserAddRows = false;
            //Equipment
            EquipmentGrid.ItemsSource = BD.EquipmentShow().DefaultView;
            EquipmentGrid.AutoGenerateColumns = true;
            EquipmentGrid.CanUserAddRows = false;
        }
        //DISCOUNT SYSTEM
        private void AddDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BD.DiscountAdd(int.Parse(DiscountSizeBox.Text), int.Parse(DiscountSummBox.Text));
                DiscountGrid.ItemsSource = BD.DiscountShow().DefaultView;
                DiscountSizeBox.Text = "Размер скидки";
                DiscountSummBox.Text = "Сумма";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void DeleteDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BD.DiscountDelete(int.Parse(DiscountIDBox.Text));
                DiscountGrid.ItemsSource = BD.DiscountShow().DefaultView;
                DiscountIDBox.Text = "ID";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void AddDayButton_Click(object sender, RoutedEventArgs e)
        {
            int mode = 1;
            if (LongMode2.IsChecked == true) mode = 2;
            if (ShortMode3.IsChecked == true) mode = 3;
            try
            {
                BD.TimeModeAdd(DateTimeMode.Text, mode);
                DiscountGrid.ItemsSource = BD.TimeModeShow().DefaultView;
                DateTimeMode.Text = "ДД.ММ.ГГГГ";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        //MENU->CLIENTS
        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            InfoTab.Visibility = Visibility.Hidden;
        }
        //MENU->RENT
        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            InfoTab.Visibility = Visibility.Hidden;
        }
        //MENU->SCHEDULE
        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            InfoTab.Visibility = Visibility.Hidden;
        }
    }
}
