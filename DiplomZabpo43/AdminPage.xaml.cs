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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiplomZabpo43
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    /// 
    public partial class AdminPage : Page
    {
        private MainWindow mainWindow;
        public int user_Id;
        public int role_id;

        public AdminPage(MainWindow mainWindow, int user_Id, int role_id)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.user_Id = user_Id;
            this.role_id = role_id;
            if (role_id == 1) // Если пользователь - администратор
            {
                ShowAdmin();
            }
            else // Если пользователь не является администратором
            {
                HideAdmin();
            }

        }
        private void Btn_back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        public void HideAdmin()
        {
            UsersButton.Visibility = Visibility.Collapsed;
        }

        public void ShowAdmin()
        {
            UsersButton.Visibility = Visibility.Visible;
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            SetUserPage setUserPage = new SetUserPage(mainWindow, user_Id);
            setUserPage.user_id = user_Id;

            mainWindow.MainFrame.Navigate(setUserPage);
        }

        private void MedButton_Click(object sender, RoutedEventArgs e)
        {
            SetMedikamentPage setMedikamentPage = new SetMedikamentPage(mainWindow, user_Id);
            mainWindow.MainFrame.Navigate(setMedikamentPage);
        }

        private void RceptButton_Click(object sender, RoutedEventArgs e)
        {
            SetRecieptPage setRecieptPage = new SetRecieptPage();
            mainWindow.MainFrame.Navigate(setRecieptPage);
        }
    }
}