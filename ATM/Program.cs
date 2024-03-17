using System;
using System.Data.SQLite;
using System.Linq;

namespace ContBancarApp
{
    class Program
    {
        static string dbConnectionString = "Data Source=UserDatabase.db;Version=3;";

        static void Main(string[] args)
        {
            InitializeDatabase();

            while (true)
            {
                Console.WriteLine("1. Creare cont");
                Console.WriteLine("2. Logare");
                Console.WriteLine("3. Ieșire");

                Console.Write("Alegeți opțiunea: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateAccount();
                        break;
                    case "2":
                        Login();
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Opțiune invalidă! Încercați din nou.");
                        break;
                }
            }
        }

        static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                connection.Open();

                string createUserTableQuery = "CREATE TABLE IF NOT EXISTS Users (Username TEXT PRIMARY KEY, Password TEXT, Balance REAL);";
                SQLiteCommand createTableCommand = new SQLiteCommand(createUserTableQuery, connection);
                createTableCommand.ExecuteNonQuery();
            }
        }

        static void CreateAccount()
        {
            Console.Write("Introduceți numele de utilizator: ");
            string username = Console.ReadLine();

            Console.Write("Introduceți parola: ");
            string password = Console.ReadLine();

            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                connection.Open();

                string insertUserQuery = "INSERT INTO Users (Username, Password, Balance) VALUES (@Username, @Password, 0);";
                SQLiteCommand insertCommand = new SQLiteCommand(insertUserQuery, connection);
                insertCommand.Parameters.AddWithValue("@Username", username);
                insertCommand.Parameters.AddWithValue("@Password", password);
                try
                {
                    insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Cont creat cu succes!");
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Eroare la crearea contului: " + e.Message);
                }
            }
        }

        static void Login()
        {
            Console.Write("Introduceți numele de utilizator: ");
            string username = Console.ReadLine();

            Console.Write("Introduceți parola: ");
            string password = Console.ReadLine();

            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                connection.Open();

                var selectUserQuery = $"SELECT * FROM Users WHERE Username = '{username}' AND Password = '{password}';";
                SQLiteCommand selectCommand = new SQLiteCommand(selectUserQuery, connection);

                var reader = selectCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    Console.WriteLine("Autentificare cu succes!");

                    while (true)
                    {
                        Console.WriteLine("1. Alimentare cont");
                        Console.WriteLine("2. Interogare sold");
                        Console.WriteLine("3. Retragere numerar");
                        Console.WriteLine("4. Delogare");

                        Console.Write("Alegeți opțiunea: ");
                        string choice = Console.ReadLine();

                        switch (choice)
                        {
                            case "1":
                                Deposit(username);
                                break;
                            case "2":
                                CheckBalance(username);
                                break;
                            case "3":
                                Withdraw(username);
                                break;
                            case "4":
                                return;
                            default:
                                Console.WriteLine("Opțiune invalidă! Încercați din nou.");
                                break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Nume de utilizator sau parolă incorectă!");
                }
            }
        }

        static void Deposit(string username)
        {
            Console.Write("Introduceți suma de bani de depus: ");
            double amount = Convert.ToDouble(Console.ReadLine());

            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                connection.Open();

                var updateBalanceQuery = $"UPDATE Users SET Balance = Balance + {amount} WHERE Username = '{username}';";
                SQLiteCommand updateCommand = new SQLiteCommand(updateBalanceQuery, connection);
                try
                {
                    updateCommand.ExecuteNonQuery();
                    Console.WriteLine("Suma de bani a fost depusă cu succes!");
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Eroare la depunere: " + e.Message);
                }
            }
        }

        static void CheckBalance(string username)
        {
            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                connection.Open();

                var selectBalanceQuery = $"SELECT Balance FROM Users WHERE Username = '{username}';";
                SQLiteCommand selectCommand = new SQLiteCommand(selectBalanceQuery, connection);

                double balance = Convert.ToDouble(selectCommand.ExecuteScalar());
                Console.WriteLine($"Soldul contului este: {balance} RON");
            }
        }

        static void Withdraw(string username)
        {
            Console.Write("Introduceți suma de bani de retras: ");
            double amount = Convert.ToDouble(Console.ReadLine());

            using (var connection = new SQLiteConnection(dbConnectionString))
            {
                connection.Open();

                var selectBalanceQuery = $"SELECT Balance FROM Users WHERE Username = '{username}';";
                SQLiteCommand selectCommand = new SQLiteCommand(selectBalanceQuery, connection);

                double balance = Convert.ToDouble(selectCommand.ExecuteScalar());

                if (balance >= amount)
                {
                    var updateBalanceQuery = $"UPDATE Users SET Balance = Balance - {amount} WHERE Username = '{username}';";
                    SQLiteCommand updateCommand = new SQLiteCommand(updateBalanceQuery, connection);
                    try
                    {
                        updateCommand.ExecuteNonQuery();
                        Console.WriteLine("Suma de bani a fost retrasă cu succes!");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Eroare la retragere: " + e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Fonduri insuficiente pentru retragere!");
                }
            }
        }
    }
}
