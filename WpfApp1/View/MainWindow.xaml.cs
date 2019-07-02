using Npgsql;
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
            string conn_param = "Server=localhost;Port=5432;User Id=username;Password=password;Database=postgres;";  
            //string sql = "текст запроса к базе данных";
            NpgsqlConnection conn = new NpgsqlConnection(conn_param);
            //NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            conn.Open(); //Открываем соединение.
            //result = comm.ExecuteScalar().ToString(); //Выполняем нашу команду.
            conn.Close(); //Закрываем соединение.
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
        public static DataTable EquipmentShowSchedule()
        {

            string equipmentRequest =
                $"SELECT Equipment.id_equipment, " +
                    $"Type_equipment.type_name, " +
                    $"Type_equipment.cost, " +
                    $"Location.position_code " +
                    $"FROM Equipment, Type_equipment, " +
                    $"Location, Status_equipment " +
                    $"WHERE Status_equipment.id_status = Equipment.id_status AND " +
                    $"Location.id_location = Equipment.id_location AND " +
                    $"Type_equipment.id_type = Equipment.id_type AND " +
                    $"Status_equipment.name_status = 'Доступно для использования'";
            SQLiteCommand command = new SQLiteCommand(equipmentRequest, connection);
            DataTable dt = new DataTable();
            using (SQLiteDataAdapter reader = new SQLiteDataAdapter(command))
            {
                reader.Fill(dt);
            }
            dt.Columns[0].ColumnName = "ИД";
            dt.Columns[1].ColumnName = "Тип";
            dt.Columns[2].ColumnName = "Цена";
            dt.Columns[3].ColumnName = "Позиция";
            return dt;
        }

        //РАБОТА С РАСПИСАНИЕМ
        public static void ScheduleCreate(string Date, 
            string Hour, string Duration, string Name, 
            string Phone, bool NewClient, string Card, string Equipment)
        {
            string idDate = "1";
            string idClient = "1";
            string idCard = Card;

            string dateRequest =
                $"SELECT id_date " +
                    $"FROM Date_schedule " +
                    $"WHERE date_schedule = '{Date}';";
            SQLiteCommand command = new SQLiteCommand(dateRequest, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                idDate = reader.GetInt32(0).ToString();
            }
            reader.Close();

            if (NewClient)
            {
                string cardRequest = $"INSERT INTO Card(id_card, summary_cost, discount_size) VALUES ('{Card}', '0', '0');";
                command = new SQLiteCommand(cardRequest, connection);
                command.ExecuteNonQuery();

                string clientRequest = $"INSERT INTO Client(name_client, phone_client, id_card) VALUES ('{Name}', '{Phone}', '{idCard}');";
                command = new SQLiteCommand(clientRequest, connection);
                command.ExecuteNonQuery();

                clientRequest =
                $"SELECT id_client " +
                    $"FROM Client " +
                    $"WHERE id_card = '{idCard}';";
                command = new SQLiteCommand(clientRequest, connection);
                SQLiteDataReader rd = command.ExecuteReader();
                while (rd.Read())
                {
                    idClient = rd.GetInt32(0).ToString();
                }
                reader.Close();

            } else
            {
                string clientRequest =
                $"SELECT id_client " +
                    $"FROM Client " +
                    $"WHERE id_card = '{idCard}';";
                command = new SQLiteCommand(clientRequest, connection);
                SQLiteDataReader rd = command.ExecuteReader();
                while (rd.Read())
                {
                    idClient = rd.GetInt32(0).ToString();
                }
                reader.Close();
            }
            string sessionRequest = 
                $"INSERT INTO Schedule_session(id_date, id_client, id_card, " +
                $"id_equipment, time_session, duration) " +
                $"VALUES ('{idDate}', '{idClient}', '{idCard}', '{Equipment}', '{Hour}', '{Duration}');";
            command = new SQLiteCommand(sessionRequest, connection);
            command.ExecuteNonQuery();
        }

        public static void ClientCreate(string Name, string Phone, string Date, string Card)
        {
            string cardRequest = $"INSERT INTO Card(id_card, summary_cost, discount_size) VALUES ('{Card}', '0', '0');";
            var command = new SQLiteCommand(cardRequest, connection);
            command.ExecuteNonQuery();

            string clientRequest = $"INSERT INTO Client(name_client, phone_client, date_birth, id_card) VALUES ('{Name}', '{Phone}', '{Date}', '{Card}');";
            command = new SQLiteCommand(clientRequest, connection);
            command.ExecuteNonQuery();
        }
        
        public static DataTable ClientsShow()
        {
            string clientsRequest =
                $"SELECT " +
                    $"Client.id_client, " +
                    $"Client.name_client, " +
                    $"Client.date_birth, " +
                    $"Client.phone_client, " +
                    $"Client.id_card, " +
                    $"Card.summary_cost, " +
                    $"Card.discount_size " +
                $"FROM " +
                    $"Client, Card " +
                $"WHERE " +
                $"Client.id_card = Card.id_card";
            SQLiteCommand command = new SQLiteCommand(clientsRequest, connection);
            DataTable dt = new DataTable();
            using (SQLiteDataAdapter reader = new SQLiteDataAdapter(command))
            {
                reader.Fill(dt);
            }
            dt.Columns[0].ColumnName = "ИД";
            dt.Columns[1].ColumnName = "Имя";
            dt.Columns[2].ColumnName = "Дата рождения";
            dt.Columns[3].ColumnName = "Номер телефона";
            dt.Columns[4].ColumnName = "Карта";
            dt.Columns[5].ColumnName = "Стоимость услуг";
            dt.Columns[6].ColumnName = "Размер скидки";
            return dt;
        }

        public static DataTable ScheduleShow()
        {
            string scheduleRequest = 
                $"SELECT " +
                    $"Schedule_session.id_session, " +
                    $"Schedule_session.id_equipment, " +
                    $"Date_schedule.date_schedule, " +
                    $"Schedule_session.time_session, " +
                    $"Schedule_session.duration, " +
                    $"Schedule_session.completed " +
                $"FROM " +
                    $"Schedule_session, Date_schedule " +
                $"WHERE " +
                $"Schedule_session.id_date = Date_schedule.id_date " +
                $"AND Schedule_session.completed = 'false'";
            SQLiteCommand  command = new SQLiteCommand(scheduleRequest, connection);
            DataTable dt = new DataTable();
            using (SQLiteDataAdapter reader = new SQLiteDataAdapter(command))
            {
                reader.Fill(dt);
            }
            dt.Columns[0].ColumnName = "ИД";
            dt.Columns[1].ColumnName = "ИД Оборудования";
            dt.Columns[2].ColumnName = "Дата";
            dt.Columns[3].ColumnName = "Время";
            dt.Columns[4].ColumnName = "Длительность";
            dt.Columns[5].ColumnName = "Статус";
            return dt;
        }
        public static void ScheduleUpdate(string id)
        {
            string scheduleRequest = $"UPDATE Schedule_session SET completed='true' WHERE id_session = '{id}'";
            var command = new SQLiteCommand(scheduleRequest, connection);
            command.ExecuteNonQuery();
        }

        public static DataTable ScheduleShowCompleted()
        {
            string scheduleRequest =
                $"SELECT " +
                    $"Schedule_session.id_session, " +
                    $"Schedule_session.id_equipment, " +
                    $"Date_schedule.date_schedule, " +
                    $"Schedule_session.time_session, " +
                    $"Schedule_session.duration, " +
                    $"Schedule_session.completed " +
                $"FROM " +
                    $"Schedule_session, Date_schedule " +
                $"WHERE " +
                $"Schedule_session.id_date = Date_schedule.id_date " +
                $"AND Schedule_session.completed = 'true'";
            SQLiteCommand command = new SQLiteCommand(scheduleRequest, connection);
            DataTable dt = new DataTable();
            using (SQLiteDataAdapter reader = new SQLiteDataAdapter(command))
            {
                reader.Fill(dt);
            }
            dt.Columns[0].ColumnName = "ИД";
            dt.Columns[1].ColumnName = "ИД Оборудования";
            dt.Columns[2].ColumnName = "Дата";
            dt.Columns[3].ColumnName = "Время";
            dt.Columns[4].ColumnName = "Длительность";
            dt.Columns[5].ColumnName = "Статус";
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
            //SchedulePanel.Visibility = Visibility.Hidden;
            //ClientPanel.Visibility = Visibility.Hidden;
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {

        }
        ////AUTORIZATION
        //private void AcceptButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //if (BD.Login(LoginBox.Text, PasswordBox.Text))
        //    //{
        //        AutorizationPanel.Visibility = Visibility.Hidden;
        //        MenuPanel.Visibility = Visibility.Visible;
        //    //}
        //}
        ////MENU->INFO
        //private void InfoButton_Click(object sender, RoutedEventArgs e)
        //{
        //    InfoTab.Visibility = Visibility.Visible;
        //    SchedulePanel.Visibility = Visibility.Hidden;
        //    ClientPanel.Visibility = Visibility.Hidden;
        //    //Discount system
        //    DiscountGrid.ItemsSource = BD.DiscountShow().DefaultView;
        //    DiscountGrid.AutoGenerateColumns = true;
        //    DiscountGrid.CanUserAddRows = false;
        //    //Time mode
        //    TimeModeGrid.ItemsSource = BD.TimeModeShow().DefaultView;
        //    TimeModeGrid.AutoGenerateColumns = true;
        //    TimeModeGrid.CanUserAddRows = false;
        //    //Equipment
        //    EquipmentGrid.ItemsSource = BD.EquipmentShow().DefaultView;
        //    EquipmentGrid.AutoGenerateColumns = true;
        //    EquipmentGrid.CanUserAddRows = false;
        //}
        ////DISCOUNT SYSTEM
        //private void AddDiscountButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        BD.DiscountAdd(int.Parse(DiscountSizeBox.Text), int.Parse(DiscountSummBox.Text));
        //        DiscountGrid.ItemsSource = BD.DiscountShow().DefaultView;
        //        DiscountSizeBox.Text = "Размер скидки";
        //        DiscountSummBox.Text = "Сумма";
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}

        //private void DeleteDiscountButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        BD.DiscountDelete(int.Parse(DiscountIDBox.Text));
        //        DiscountGrid.ItemsSource = BD.DiscountShow().DefaultView;
        //        DiscountIDBox.Text = "ID";
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}

        //private void AddDayButton_Click(object sender, RoutedEventArgs e)
        //{
        //    int mode = 1;
        //    if (LongMode2.IsChecked == true) mode = 2;
        //    if (ShortMode3.IsChecked == true) mode = 3;
        //    try
        //    {
        //        BD.TimeModeAdd(DateTimeMode.Text, mode);
        //        DiscountGrid.ItemsSource = BD.TimeModeShow().DefaultView;
        //        DateTimeMode.Text = "ДД.ММ.ГГГГ";
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}

        ////MENU->CLIENTS
        //private void ClientsButton_Click(object sender, RoutedEventArgs e)
        //{
        //    InfoTab.Visibility = Visibility.Hidden;
        //    SchedulePanel.Visibility = Visibility.Hidden;
        //    ClientPanel.Visibility = Visibility.Visible;

        //    ClientsGrid.ItemsSource = BD.ClientsShow().DefaultView;
        //    ClientsGrid.AutoGenerateColumns = true;
        //    ClientsGrid.CanUserAddRows = false;

        //}
        //private void CreateClientButton_Click(object sender, RoutedEventArgs e)
        //{
        //    BD.ClientCreate(NameCl.Text, NumberCl.Text, DateCl.Text, CardCl.Text);
        //    ClientsGrid.ItemsSource = BD.ClientsShow().DefaultView;
        //    NameCl.Text = "Имя";
        //    NumberCl.Text = "Номер телефона";
        //    DateCl.Text = "Дата рождения";
        //    CardCl.Text = "ИД Карты";
        //}
        ////MENU->SCHEDULE
        //private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        //{
        //    InfoTab.Visibility = Visibility.Hidden;
        //    ClientPanel.Visibility = Visibility.Hidden;
        //    SchedulePanel.Visibility = Visibility.Visible;
        //    EquipmentList.ItemsSource = BD.EquipmentShowSchedule().DefaultView;
        //    EquipmentGrid.AutoGenerateColumns = true;
        //    EquipmentGrid.CanUserAddRows = false;

        //    ScheduleGid.ItemsSource = BD.ScheduleShow().DefaultView;
        //    ScheduleGid.AutoGenerateColumns = true;
        //    ScheduleGid.CanUserAddRows = false;

        //    ScheduleGidCompleted.ItemsSource = BD.ScheduleShowCompleted().DefaultView;
        //    ScheduleGidCompleted.AutoGenerateColumns = true;
        //    ScheduleGidCompleted.CanUserAddRows = false;
        //}
        //private void MakeCompletedButton_Click(object sender, RoutedEventArgs e)
        //{
        //    BD.ScheduleUpdate(SessionID.Text);
        //    SessionID.Text = "ID";
        //    ScheduleGid.ItemsSource = BD.ScheduleShow().DefaultView;
        //    ScheduleGidCompleted.ItemsSource = BD.ScheduleShowCompleted().DefaultView;

        //}
        //private void CreateSessionButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var NewClient = (CheckNewClient.IsChecked == true? true: false);
        //    BD.ScheduleCreate(SessionDate.Text, 
        //        SessionHour.Text,
        //        SessionDuration.Text,
        //        SessionName.Text,
        //        SessionPhone.Text,
        //        NewClient,
        //        SessionCard.Text,
        //        SessionEquipmentID.Text);

        //    SessionDate.Text = "ДД.ММ.ГГГГ";
        //    SessionHour.Text = "ЧЧ";
        //    SessionDuration.Text = "Пролоджительность";
        //    SessionName.Text = "Имя";
        //    SessionPhone.Text = "Телефон";
        //    CheckNewClient.IsChecked = false;
        //    SessionCard.Text = "ID Карты";
        //    SessionEquipmentID.Text = "ID";
        //}
    }
}
