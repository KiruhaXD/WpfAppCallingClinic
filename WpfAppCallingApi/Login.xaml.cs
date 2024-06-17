using System;
using System.Collections.Generic;
using System.Data;
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
using MaterialDesignThemes.Wpf;

namespace WpfAppCallingApi
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {

       //public MainWindowModel mainWindowModel { get; set; }

        public Login(/*MainWindowModel mainWindowModel*/)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //this.mainWindowModel = mainWindowModel;
        }

        

        public bool IsDarkTheme { get; set; }
        private readonly PaletteHelper paletteHelper = new PaletteHelper();

        private void toggleTheme(object sender, RoutedEventArgs e)
        {
            /*ITheme theme = paletteHelper.GetTheme();

            if (IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark)
            {
                IsDarkTheme = false;
                theme.SetBaseTheme(Theme.Light);
            }

            else 
            {
                IsDarkTheme = true;
                theme.SetBaseTheme(Theme.Dark);
            }
            paletteHelper.SetTheme(theme);*/
        }

        private void exitApp(object sender, RoutedEventArgs e)
        {
             Application.Current.Shutdown();
        }

        private void LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            // это нужно для того чтобы мы могли двигать это окно на левую кнопку мыши
            if (e.ChangedButton == MouseButton.Left) 
            {
                DragMove();
            }

        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string userName = txtUsername.Text;
            string password = txtPassword.Password;

            string sqlConnection = "server=DESKTOP-HIOKBS7;Trusted_Connection=Yes;DataBase=Drug_treatment_clinic;";

            using (SqlConnection connection = new SqlConnection(sqlConnection))
            {
                connection.Open();

                string query = $"SELECT * FROM [dbo].[enterForUsers1] WHERE [login] = @username AND [password] = @password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", userName);
                command.Parameters.AddWithValue("@password", password);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    if (txtUsername.Text.Length > 0)
                    {
                        if (txtPassword.Password.Length > 0)
                        {
                            MessageBox.Show("Пользователь успешно авторизовался! 😊 ");
                            MainWindowModel model = new MainWindowModel();
                            model.Show();

                            Window window = Window.GetWindow(this);
                            if (window != null) 
                            {
                                window.Close();
                            }
                                
                           
                        }

                        else
                        {
                            MessageBox.Show("Заполните пустые поля, пожалуйста");
                        }

                    }

                    else 
                    {
                        MessageBox.Show("Заполните пустые поля, пожалуйста");
                    }

                }

                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль. Пожалуйста, попробуйте снова. ❌ ");
                    txtUsername.Clear();
                    txtPassword.Clear();
                }
            }


        }

        private void btnSugnUp_Click(object sender, RoutedEventArgs e)
        {
            Regin regin = new Regin();
            regin.Show();

            Window login = Window.GetWindow(this);
            if (login != null)
            {
                login.Close();
            }

        }
    }
}
