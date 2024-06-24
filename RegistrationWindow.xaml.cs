using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Reflection;
using System.Net.Mail;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace SignInAndSignUpForm
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            // Сброс стилей
            ResetFieldStyles();

            // Логика обработки регистрации
            string username = RegUsernameTextBox.Text;
            string password = RegPasswordBox.Password;
            string confirmPassword = RegConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                isValid = false;
                RegUsernameTextBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Имя пользователя не может быть пустым.");
            }
            else if (!IsValidEmail(username))
            {
                isValid = false;
                RegUsernameTextBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Неправильный формат почты.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                isValid = false;
                RegPasswordBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Пароль не может быть пустым.");
            }
            else if (password.Length < 6)
            {
                isValid = false;
                RegPasswordBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Пароль должен содержать как минимум 6 символов.");
            }

            if (password != confirmPassword)
            {
                isValid = false;
                RegConfirmPasswordBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Пароли не совпадают.");
            }

            if (!IsEmailUnique(username))
            {
                isValid = false;
                MessageBox.Show("Эта почта уже зарегистрирована.");
            }

            if (isValid)
            {
                // Простая проверка и регистрация
                MessageBox.Show("Регистрация успешно завершена!");

                // Сохранение пользователя в файл JSON
                SaveUserToJson(new User { email = username, password = password });

                // Переход обратно на страницу входа
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        public class User
        {
            public string email { get; set; }
            public string password { get; set; }
        }

        private bool IsEmailUnique(string email)
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

                    // Проверяем, существует ли уже такая почта
                    foreach (var user in users)
                    {
                        if (user.email == email)
                        {
                            return false; // Почта уже существует
                        }
                    }
                }
                catch (JsonSerializationException ex)
                {
                    MessageBox.Show($"Ошибка чтения существующих пользователей: {ex.Message}");
                    // Опционально: запись ошибки в лог или её обработка
                }
            }

            return true; // Почта уникальна
        }

        private void SaveUserToJson(User user)
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
                    //MessageBox.Show($"Ошибка чтения существующих пользователей: {ex.Message}");
                    // Опционально: запись ошибки в лог или её обработка
                }
            }

            // Добавляем нового пользователя в список
            users.Add(user);

            // Записываем обновлённый список обратно в файл
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

       

        private void ResetFieldStyles()
        {
            RegUsernameTextBox.BorderBrush = Brushes.Gray;
            RegPasswordBox.BorderBrush = Brushes.Gray;
            RegConfirmPasswordBox.BorderBrush = Brushes.Gray;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
