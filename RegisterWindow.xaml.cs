using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.Json;
using System.Windows;

namespace FinalADO
{
    public partial class RegisterWindow : Window
    {
        private dynamic? emailConfig;
        private readonly HttpClient httpClient = new HttpClient();
        private HttpClient _httpClient = new();
        string password;
        private const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Admin\source\FinalADO\FinalADO\PasswordCheck.mdf;Integrated Security=True";
        public RegisterWindow()
        {
            InitializeComponent();
            httpClient = new();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            String configFilename = "emailconfig.json";
            try
            {
                emailConfig = JsonSerializer.Deserialize<dynamic>(
                    System.IO.File.ReadAllText(configFilename)
                );
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show($"Не найден файл конфигурации '{configFilename}'");
                this.Close();
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"Ошибка преобразования конфигурации '{ex.Message}'");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обработки конфигурации '{ex.Message}'");
                this.Close();
            }
            if (emailConfig is null)
            {
                MessageBox.Show("Ошибка получения конфигурации");
                this.Close();
            }
        }
        private void CloseRegWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FinishRegWindow_Click(object sender, RoutedEventArgs e)
        {
            string enteredPassword = ConfirmPasswordTextBox.Text;
            string email = LoginTextBox.Text;
            string passwordString = ConfirmPasswordTextBox.Text;
            string confirmPasswordString = ConfirmYPasswordTextBox.Text;
            string login = ConfirmYLoginTextBox.Text;
            string name = NameTextBox.Text;

            if (string.IsNullOrEmpty(passwordString) || string.IsNullOrEmpty(confirmPasswordString) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Проверка пароля из базы данных
            string checkQuery = "SELECT COUNT(*) FROM NpRegistration WHERE password = @EnteredPassword";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(checkQuery, connection))
                {
                    command.Parameters.AddWithValue("@EnteredPassword", enteredPassword);
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Password is correct. Access granted.");

                        // Отправка запроса на добавление нового пользователя в базу данных
                        string addUserQuery = "INSERT INTO NpUsers (Id, Name, Login, Email, Password) SELECT @Id, @Name, @Login, @Email, @Password WHERE NOT EXISTS (SELECT 1 FROM NpUsers WHERE Id = @Id);";

                        using (SqlCommand addUserCommand = new SqlCommand(addUserQuery, connection))
                        {
                            Guid newId = Guid.NewGuid(); // Генерация нового уникального идентификатора
                            addUserCommand.Parameters.AddWithValue("@Id", newId);
                            addUserCommand.Parameters.AddWithValue("@Name", name);
                            addUserCommand.Parameters.AddWithValue("@Login", login);
                            addUserCommand.Parameters.AddWithValue("@Email", email);
                            addUserCommand.Parameters.AddWithValue("@Password", confirmPasswordString);
                            int rowsAffected = addUserCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("User added successfully.");
                            }
                            else
                            {
                                MessageBox.Show("Failed to add user.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Password is incorrect. Access denied.");
                    }
                }
            }
        }

        private SmtpClient GetSmtpClient()
        {
            if (emailConfig is null) { return null!; }
            JsonElement gmail = emailConfig.GetProperty("smtp").GetProperty("gmail");

            String host = gmail.GetProperty("host").GetString()!;
            int port = gmail.GetProperty("port").GetInt32();
            String mailbox = gmail.GetProperty("email").GetString()!;
            String password = gmail.GetProperty("password").GetString()!;
            bool ssl = gmail.GetProperty("ssl").GetBoolean();

            return new SmtpClient(host)
            {
                Port = port,
                EnableSsl = ssl,
                Credentials = new NetworkCredential(mailbox, password)
            };
        }
        private void CheckMail_Click(object sender, RoutedEventArgs e)
        {
            password = GenerateRandomPassword();
            using (SmtpClient smtpClient = GetSmtpClient())
            {
                if (smtpClient is null)
                {
                    MessageBox.Show("SMTP client configuration is missing.");
                    return;
                }

                JsonElement gmail = emailConfig.GetProperty("smtp").GetProperty("gmail");
                string mailbox = gmail.GetProperty("email").GetString();

                try
                {
                    MailMessage mailMessage = new MailMessage()
                    {
                        From = new MailAddress(mailbox),
                        Body = "Your password: " + password,
                        IsBodyHtml = false,
                        Subject = "Test Message"
                    };
                    mailMessage.To.Add(new MailAddress(LoginTextBox.Text));

                    smtpClient.Send(mailMessage);

                    MessageBox.Show("Password sent to the email address.");

                    /// Получение Id самого старого пароля
                    string selectQuery = "SELECT TOP 1 Id FROM NpRegistration ORDER BY Id ASC";

                    int oldestPasswordId;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                        connection.Open();
                        oldestPasswordId = (int)selectCommand.ExecuteScalar();
                    }

                    // Удаление самого старого пароля
                    string deleteQuery = $"DELETE FROM NpRegistration WHERE Id = {oldestPasswordId}";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                        connection.Open();
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Запись нового пароля в базу данных
                    string insertQuery = $"INSERT INTO NpRegistration (password) VALUES ('{password}')";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Password saved to the database.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending email:{ex.Message}");
                }
            }
        }


        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}