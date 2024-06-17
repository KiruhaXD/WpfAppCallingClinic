using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WpfAppCallingApi
{
    /// <summary>
    /// Логика взаимодействия для Regin.xaml
    /// </summary>
    public partial class Regin : Window
    {
        public Regin()
        {
            InitializeComponent();

            /*showPasswordCheckBox.Checked += (sender, e) =>
            {
                txtPasswordRegin.Visibility = System.Windows.Visibility.Collapsed;

                TextBox tb = new TextBox();
                tb.Text = txtPasswordRegin.Password;
                tb.TextChanged += (ss, ee) => txtPasswordRegin.Password = tb.Text;
                tb.Margin = txtPasswordRegin.Margin;
                tb.Padding = txtPasswordRegin.Padding;
                tb.HorizontalAlignment = txtPasswordRegin.HorizontalAlignment;
                tb.VerticalAlignment = txtPasswordRegin.VerticalAlignment;
                tb.Width = txtPasswordRegin.Width;
                tb.Height = txtPasswordRegin.Height;
                tb.Background = txtPasswordRegin.Background;
                tb.BorderBrush = txtPasswordRegin.BorderBrush;
                tb.FontSize = txtPasswordRegin.FontSize;
                tb.FontFamily = txtPasswordRegin.FontFamily;
                tb.FontStyle = txtPasswordRegin.FontStyle;
                tb.FontWeight = txtPasswordRegin.FontWeight;

                StackPanel parent = (StackPanel)txtPasswordRegin.Parent;
                int index = parent.Children.IndexOf(txtPasswordRegin);
                parent.Children.Remove(txtPasswordRegin);
                parent.Children.Insert(index, tb);
            };

            showPasswordCheckBox.Unchecked += (sender, e) =>
            {
                txtPasswordRegin.Visibility = System.Windows.Visibility.Visible;
                TextBox tb = (TextBox)txtPasswordRegin.Parent;

                txtPasswordRegin.Password = tb.Text;
                tb.Text = txtPasswordRegin.Password;
                txtPasswordRegin.Margin = tb.Margin;
                txtPasswordRegin.Padding = tb.Padding;
                txtPasswordRegin.HorizontalAlignment = tb.HorizontalAlignment;
                txtPasswordRegin.VerticalAlignment = tb.VerticalAlignment;
                txtPasswordRegin.Width = tb.Width;
                txtPasswordRegin.Height = tb.Height;
                txtPasswordRegin.Background = tb.Background;
                txtPasswordRegin.BorderBrush = tb.BorderBrush;
                txtPasswordRegin.FontSize = tb.FontSize;
                txtPasswordRegin.FontFamily = tb.FontFamily;
                txtPasswordRegin.FontStyle = tb.FontStyle;
                txtPasswordRegin.FontWeight = tb.FontWeight;


                StackPanel parent = (StackPanel)tb.Parent;
                int index = parent.Children.IndexOf(tb);
                parent.Children.Remove(tb);
                parent.Children.Insert(index, txtPasswordRegin);
            };*/
        }

        private void exitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

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

        private void LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            // это нужно для того чтобы мы могли двигать это окно на левую кнопку мыши
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }

        }

        private void btnSugnUpRegin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsernameRegin.Text;
            string password = txtPasswordRegin.Password;

            string sqlconnection = "server=DESKTOP-HIOKBS7;Trusted_Connection=Yes;DataBase=Drug_treatment_clinic;";

            using (SqlConnection connection = new SqlConnection(sqlconnection)) 
            {
                connection.Open();

                string query = "INSERT INTO [dbo].[enterForUsers1] ([login], [password]) VALUES (@username, @password)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("password", password);

                if (txtUsernameRegin.Text.Length > 0)
                {
                    if (txtPasswordRegin.Password.Length > 0)
                    {
                        if (Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*\W)(?=.*\d).{8,}$"))
                        {
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Пользователь успешно зарегистрирован! Теперь вы можете использовать свои учетные данные для входа. 🎉");
                                Login login = new Login();
                                login.Show();
                            }
                            else
                            {
                                MessageBox.Show("Произошла ошибка при регистрации пользователя.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Пароль слишком слабый. Пожалуйста, используйте как минимум одну заглавную букву, один специальный символ и как минимум одну цифру.");
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
                    txtUsernameRegin.Clear();
                    txtPasswordRegin.Clear();
                }

            }
        }

        private void txtUsernameRegin_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ReturnInWindowLogin_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();

            Window regin = Window.GetWindow(this);
            if (regin != null) 
            {
                regin.Close();
            }

        }

        private void showPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void showPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
