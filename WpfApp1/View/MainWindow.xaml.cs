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

namespace WpfApp1.View
{
    

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Postgre.OpenConnection();
            InitializeComponent();
            AnotherListItemSelected();
        }

        public void AnotherListItemSelected()
        {
            SPTable.Visibility = Visibility.Collapsed;
            SPTimeTable.Visibility = Visibility.Collapsed;
            btnNew.Click -= CreateNewClient;
            btnNew.Click -= CreateEquipment;
            tTable.MouseLeftButtonUp -= MembershipClicked;
        }



        #region Выбор пункта меню слева
        
        private void ListSessions_Selected(object sender, RoutedEventArgs e)
        {
            AnotherListItemSelected();
        }

        private void ListMemberships_Selected(object sender, RoutedEventArgs e)
        {
            AnotherListItemSelected();
            UpdateTable = null;
            UpdateTable += UpdateTableMemberships;

            SPTable.Visibility = Visibility.Visible;
            Headline.Text = "Абонементы";
            tTable.ItemsSource = Postgre.LoadMembershipsList().DefaultView;
            tTable.MouseLeftButtonUp += MembershipClicked;
        }

        private void ListSchedule_Selected(object sender, RoutedEventArgs e)
        {
            AnotherListItemSelected();
        }

        private void ListTimeTable_Selected(object sender, RoutedEventArgs e)
        {
            AnotherListItemSelected();
            SPTimeTable.Visibility = Visibility.Visible;
            var today = DateTime.Today;
            curmonth = today.Month;
            curyear = today.Year;
            LoadDays(curmonth, curyear);
        }

        int curmonth, curyear;

        private void LoadDays(int month, int year)
        {
            MonthText.Text = new System.Globalization.DateTimeFormatInfo().GetMonthName(curmonth).ToString(); ;

            int startCol = (int)(new DateTime(curyear, curmonth, 1)).DayOfWeek;
            if (startCol == 0) startCol = 7;
            startCol--;
            var q = DateTime.DaysInMonth(curyear, curmonth);
            for (int i = 1, k = 1; i < 7; i++) {
                for (int j = 0; j < 7 && k <= q; j++) {
                    
                    if (k == 1)
                        j = startCol;
                    Button br = new Button();
                    br.Content = k.ToString();
                    br.Foreground = (Brush)(new BrushConverter().ConvertFrom("#FF040404"));
                    Grid.SetRow(br, i);
                    Grid.SetColumn(br, j);
                    if (j == 5 || j == 6) {
                        br.Background = (Brush)(new BrushConverter().ConvertFrom("#FF7AA9D6"));
                    }
                    CalendarGrid.Children.Add(br);
                    k++;
                }
            }
        }

        private void ListEquipment_Selected(object sender, RoutedEventArgs e)
        {
            AnotherListItemSelected();
            UpdateTable = null;
            UpdateTable += UpdateTableEquipment;
            SPTable.Visibility = Visibility.Visible;
            Headline.Text = "Игровые системы";
            Postgre.LoadEquipmentList();
            tTable.ItemsSource = Postgre.Equipments.ToDataTable().DefaultView;
            btnNew.Click += CreateEquipment;
            //tTable.MouseLeftButtonUp += MembershipClicked;
        }

        private void ListClients_Selected(object sender, RoutedEventArgs e)
        {
            AnotherListItemSelected();

            UpdateTable = null;
            UpdateTable += UpdateTableClients;

            SPTable.Visibility = Visibility.Visible;
            Headline.Text = "Клиенты";
            Postgre.LoadClientsList();
            tTable.ItemsSource = Postgre.ClientsInfo.ToDataTable(true).DefaultView;
            btnNew.Click += CreateNewClient;
            //tTable.MouseDoubleClick += ClientClicked;
        }

        #endregion


        #region Обработка нажатия по строке таблицы
        public void MembershipClicked (object sender, RoutedEventArgs e)
        {
            Console.WriteLine("-" + ((DataRowView)tTable.SelectedItem).Row[3] + "-");
            if (((DataRowView)tTable.SelectedItem).Row[3] == null || ((DataRowView)tTable.SelectedItem).Row[3].ToString() == "") {
                var row = ((DataRowView)tTable.SelectedItem).Row;
                MembershipWindow mw = new MembershipWindow(new Membership(row[1], row[2], row[0]));
                mw.Show();
                mw.Activate();
            }
        }
        private void ClientClicked(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine();
            //var selectedClient = Postgre.GetClientById(((DataRowView)tTable.SelectedItem).Row[0].ToString());

        }
        #endregion
        
        public delegate void Updater();
        public static Updater UpdateTable = null;

#region Обновить таблицы после изменения данных в них

        public void UpdateTableClients ()
        {
            Postgre.LoadClientsList();
            tTable.ItemsSource = Postgre.ClientsInfo.ToDataTable(false).DefaultView;
        }
        
        public void UpdateTableMemberships()
        {
            tTable.ItemsSource = Postgre.LoadMembershipsList().DefaultView;
        }

        public void UpdateTableEquipment()
        {
            Postgre.LoadEquipmentList();
            tTable.ItemsSource = Postgre.Equipments.ToDataTable().DefaultView;
        }

        #endregion

        public void CreateNewClient(object sender, RoutedEventArgs e)
        {
            ClientWindow cw = new ClientWindow();
            cw.Show();
            cw.Activate();
        }

        public void CreateEquipment(object sender, RoutedEventArgs e)
        {
            EquipmentWindow cw = new EquipmentWindow();
            cw.Show();
            cw.Activate();
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
