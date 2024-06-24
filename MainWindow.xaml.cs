using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using static SignInAndSignUpForm.RegistrationWindow;

namespace SignInAndSignUpForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private List<User> LoadUsersFromJson()
        {
            // Получаем директорию исполняемого файла
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Сочетаем путь к файлу users.json с текущей директорией
            string filePath = Path.Combine(directoryPath, "../../../users.json");

            List<User> users = new List<User>();

            // Читаем существующих пользователей из файла
            if (File.Exists(filePath))
            {
                try
                {
                    string existingJson = File.ReadAllText(filePath);
                    users = JsonConvert.DeserializeObject<List<User>>(existingJson) ?? new List<User>();
                }
                catch (JsonSerializationException ex)
                {
                    MessageBox.Show($"Ошибка чтения существующих пользователей: {ex.Message}");
                    // Опционально: запись ошибки в лог или её обработка
                }
            }

            return users;
        }
        public bool AuthenticateUser(string email, string password)
        {
            List<User> users = LoadUsersFromJson();

            foreach (var user in users)
            {
                if (user.email == email)
                {
                    if (user.password == password)
                        return true; // Пароль совпадает, пользователь аутентифицирован
                    else
                        return false; // Пароль не совпадает
                }
            }

            return false; // Пользователь с таким email не найден
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика обработки логина
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Успешно");
                // Переход на другую страницу/окно после успешного входа
            }
            else
            {
                MessageBox.Show("Неправильная почта или пароль.");
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }
    }
}