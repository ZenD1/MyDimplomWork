using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomZabpo43.classes
{
    public class DatabaseHelper
    {
        private Connection connection = new Connection(); // Создание экземпляра класса Connection

        public bool RegisterUser(string email, string repeatedEmail, string phone, string password, string repeatedPassword, DateTime birthday)
        {
            // Проверка совпадения введенных почт и паролей
            if (email != repeatedEmail || password != repeatedPassword)
            {
                // Пароли или почты не совпадают
                return false;
            }

            // Создание подключения к базе данных
            using (SqlConnection sqlConnection = new SqlConnection(connection.ConnString))
            {
                try
                {
                    sqlConnection.Open();

                    // Запрос для вставки нового пользователя в базу данных
                    string query = @"INSERT INTO User_table (user_role, email, phone, [password], birthday) 
                                     VALUES (@role, @email, @phone, @password, @birthday)";

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        // Здесь предполагается, что у вас есть какой-то механизм для определения роли пользователя
                        // Например, вы можете добавить метод, который будет возвращать id роли по ее имени
                        int roleId = GetRoleId("some_role_name");

                        // Параметры для запроса
                        command.Parameters.AddWithValue("@role", roleId);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@birthday", birthday);

                        // Выполнение запроса
                        int rowsAffected = command.ExecuteNonQuery();

                        // Проверка успешности выполнения запроса
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    // Обработка исключений
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }

        private int GetRoleId(string roleName)
        {
            // Здесь вы можете реализовать логику получения id роли по ее имени из базы данных
            // В данном примере просто возвращается какое-то значение
            return 1;
        }
    }
}