using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfApp1.View
{
    /// <summary>
    /// Логика взаимодействия для MembershipWindow.xaml
    /// </summary>
    public partial class MembershipWindow : Window
    {
        Membership m = null;

        public MembershipWindow(Membership membership)
        {
            InitializeComponent();
            this.m = membership;
            IdText.Text = m.Id;
            PriceText.Text = m.Price;
            DurationText.Text = m.Duration;
        }

        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            Postgre.ExecuteNonQuery(m.ToQueryActivate());
            MainWindow.UpdateTable();
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
