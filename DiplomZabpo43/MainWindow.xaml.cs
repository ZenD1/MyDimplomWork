using DiplomZabpo43.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public HomePageUser HomePageInstance;
        public BuscketPage busyPage;

        public string _enteredEmail;
        public int user_Id;
        public int role_id;
        public Frame mainFrame; // Объявление переменной mainFrame
        public LoginPage loginPage; // Объявление переменной loginPage
        public string valuecate;
        private SpisokPage spisokPage; // Объявление переменной spisokPage
        public int visiblecatalog;
        public int visibleseatch;
        public int visiblebuscet;
        public int visibleclock;


        public bool _isLoggedIn = false;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetIsLoggedIn(bool value)
        {
            IsLoggedIn = value;
        }

        public event EventHandler SuccessfulLogin;


        public MainWindow()
        {
            LoginPage loginPage = new LoginPage(this, user_Id, role_id);

            // Перемещаем инициализацию _enteredEmail сюда
            _enteredEmail = loginPage.EnteredEmail;
            InitializeComponent();
            DataContext = this;
            spisokPage = new SpisokPage(this, user_Id);
            MainFrame.NavigationService.Navigate(spisokPage);
            visiblecatalog = 0;
            visibleseatch = 0;
            visiblebuscet = 0;
            visibleclock = 0;
            CheackBuscket();
            colorMenusUser();
        }




        //----------------------------------------------------------------


        private void BuscketButton_Click(object sender, RoutedEventArgs e)
        {
            BuscketPage buscketPage = new BuscketPage(this, user_Id, role_id);
            buscketPage.role_id = role_id;

            if (MainFrame.NavigationService != null)
            {
                if (busyPage == null)
                {
                    busyPage = buscketPage;
                }

                MainFrame.NavigationService.Navigate(buscketPage);
            }
            else
            {
                MessageBox.Show("Не удалось найти сервис навигации.");
            }

        }


        private void OpenProfilenPage_Click(object sender, RoutedEventArgs e)
        {
            // Создаем экземпляр HomePageUser с передачей MainWindow
            HomePageUser homePageUser = new HomePageUser(this, user_Id, role_id);

            // Передаем данные пользователя
            homePageUser.useremail = _enteredEmail;
            homePageUser.user_Id = user_Id;
            homePageUser.role_id = role_id;
            // Отображаем адрес электронной почты пользователя в MessageBox

            // Переходим на страницу HomePageUser, но перед этим сохраняем ссылку на экземпляр
            if (MainFrame.NavigationService != null)
            {
                // Если еще не создан экземпляр HomePageUser, то создаем его и сохраняем ссылку
                if (HomePageInstance == null)
                {
                    HomePageInstance = homePageUser;
                }

                // Переходим на страницу HomePageUser
                MainFrame.NavigationService.Navigate(homePageUser);
            }
            else
            {
                MessageBox.Show("Не удалось найти сервис навигации.");
            }
        }

        //----------------------------------------------------------------


        public void colorMenusUser()
        {
            if (IsLoggedIn == true)
            {
                UserButton.Background = Brushes.LightGreen;
                // Установка светло-зеленого цвета, если пользователь залогинен
            }
            else
            {
                UserButton.Background = Brushes.White;
                // Установка светло-серого цвета, если пользователь не залогинен
            }

        }
       
        public void CheackLogingUser()
        {
            if (user_Id > 0)
            {
                MenuItemUserLOG.IsSubmenuOpen = true;
                UnLogUserMenu.Visibility = Visibility.Visible;
                // Установка светло-зеленого цвета, если пользователь залогинен
            }
            else
            {
                MenuItemUserUNLOG.IsSubmenuOpen = true;
                LogUserMenu.Visibility = Visibility.Visible;
                // Установка светло-серого цвета, если пользователь не залогинен
            }
        }
        private void OpenLoginPage_Click(object sender, RoutedEventArgs e)
        {
            CheackLogingUser();
        }
      
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Отображаем диалоговое окно подтверждения
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Проверяем результат диалогового окна
            if (result == MessageBoxResult.Yes)
            {

                Obnilim();
                colorMenusUser();
            }
        }
        public void Obnilim()
        {
            user_Id = 0;
            _isLoggedIn = false;
        }


        //----------------------------------------------------------------


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (spisokPage.SearchTexttt.Visibility == Visibility.Collapsed)
            {
                spisokPage.SearchTexttt.Visibility = Visibility.Visible;
                spisokPage.SearchTextt.Visibility = Visibility.Visible;
                visibleseatch = 1;

            }
            else
            {

                // Если панель видима, скрываем ее
                spisokPage.SearchTexttt.Visibility = Visibility.Collapsed;
                spisokPage.SearchTextt.Visibility = Visibility.Visible;
                visibleseatch = 0;

            }

            // Установка цвета кнопки в зависимости от значения visiblecatalog
            if (visibleseatch == 0)
            {
                SearchButton.Background = Brushes.White; // Установка светло-серого цвета
            }
            else
            {
                SearchButton.Background = Brushes.LightGreen; // Установка светло-зеленого цвета
            }
        }
        private void CatalogButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем текущую видимость панели
            if (spisokPage.PanelFilter.Visibility == Visibility.Collapsed)
            {
                // Если панель скрыта, показываем ее
                spisokPage.PanelFilter.Visibility = Visibility.Visible;
                spisokPage.PanelFilter2.Visibility = Visibility.Visible;
                visiblecatalog = 1;
            }
            else
            {
                // Если панель видима, скрываем ее
                spisokPage.PanelFilter.Visibility = Visibility.Collapsed;
                spisokPage.PanelFilter2.Visibility = Visibility.Collapsed;
                visiblecatalog = 0;
            }

            // Установка цвета кнопки в зависимости от значения visiblecatalog
            if (visiblecatalog == 0)
            {
                CatalogButton.Background = Brushes.White; // Установка светло-серого цвета
            }
            else
            {
                CatalogButton.Background = Brushes.LightGreen; // Установка светло-зеленого цвета
            }
        }
        public void CheackCat()
        {
            // Проверяем текущую видимость панели
            if (spisokPage.PanelFilter.Visibility == Visibility.Collapsed)
            {
                // Если панель скрыта, показываем ее
                spisokPage.PanelFilter.Visibility = Visibility.Visible;
                spisokPage.PanelFilter2.Visibility = Visibility.Visible;
                visiblecatalog = 1;
            }
            else
            {
                // Если панель видима, скрываем ее
                spisokPage.PanelFilter.Visibility = Visibility.Collapsed;
                spisokPage.PanelFilter2.Visibility = Visibility.Collapsed;
                visiblecatalog = 0;
            }

            // Установка цвета кнопки в зависимости от значения visiblecatalog
            if (visiblecatalog == 0)
            {
                CatalogButton.Background = Brushes.LightGray; // Установка светло-серого цвета
            }
            else
            {
                CatalogButton.Background = Brushes.LightGreen; // Установка светло-зеленого цвета
            }
        }

      
      
        public void CheackBuscket()
        {
            if (_isLoggedIn)
            {
                CartButton.Visibility = Visibility.Visible;
            }
            else
            {
                CartButton.Visibility = Visibility.Collapsed;

            }
        }
        
            private void CategoryItem_Expanded(object sender, RoutedEventArgs e)
        {
            // Ваш код обработки события
        }

      
        private void Precent_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(spisokPage.user_id.ToString());
            MessageBox.Show(_enteredEmail);
            MessageBox.Show(user_Id.ToString());
            if (user_Id > 0)
            {
                UserButton.Background = Brushes.White;
            }
            else
            {
                UserButton.Background = Brushes.White;
            }
        }

    

        private void LoginAut_Clock(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new LoginPage(this, user_Id, role_id));
        }

        private void Registr_Clock(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new RegistrPage(this));

          
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }
      
    }
}