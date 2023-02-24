
using Microsoft.Data.Sqlite;
using System.Globalization;

class Program
{
    static string connectionString = @"Data Source=habit-Tracker.db";

    static void Main(string[] args)
    {

       

        using (var connection = new SqliteConnection(connectionString))
        {

            connection.Open();
            var tableCmd = connection.CreateCommand();


            tableCmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS habits (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Name TEXT,
                        Habit TEXT,
                        Quantity INTEGER,
                        Unit TEXT
                        )";

            tableCmd.ExecuteNonQuery();

            connection.Close();
        }

        GetUserInput();
    }

    static void GetUserInput()
    {
        Console.Clear();
        bool closeApp = false;
        while(closeApp == false)
        {
            Console.WriteLine("\n\nMAIN MENU");
            Console.WriteLine("\nWhat would you like to do");
            Console.WriteLine("\nType 0 to close App");
            Console.WriteLine("Type 1 to View All Data");
            Console.WriteLine("Type 2 to Insert data");
            Console.WriteLine("Type 3 to Delete data");
            Console.WriteLine("Type 4 to Update data");
            Console.WriteLine("--------------------------------------");


            var command = Console.ReadLine();

            switch (command)
            {
                case "0":
                    Console.WriteLine("Goodbye!");
                    closeApp = true;
                    Environment.Exit(0);
                    break;
                case "1":
                    GetAllRecords();
                    break;
                case "2":
                    Insert();
                    break;
                case "3":
                    Delete();
                    break;
                case "4":
                    Update();
                    break;
                default:
                    Console.WriteLine("Invalid Command. Please type a number from 0 to 4.");
                    break;
            }
        }
        
        
    }

    private static void GetAllRecords()
    {
        Console.Clear();
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"SELECT * FROM habits";

            List<HabitLog> tableData = new List<HabitLog>();
            SqliteDataReader reader = tableCmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    tableData.Add(
                        new HabitLog
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        });
                }
            }
            else
            {
                Console.WriteLine("No rows found");
            }

            connection.Close();

            Console.WriteLine("----------------------------------------------------------------------------");
            foreach (var dw in tableData)
            {
                Console.WriteLine($"{dw.Id} - {dw.Name}  - {dw.Habit} - {dw.Date.ToString("dd-MMM-yy")} - Quantity: { dw.Quantity}");
            }
            Console.WriteLine("-----------------------------------------------------------------------------");
        }
    }
    private static void Insert()
    {
        Console.Clear();
        string date = GetDateInput();
        int quantity = GetNumberInput("Please insert the amount of times you performed the habit");
        string unit = GetMeasurementInput();
        string name = GetNameInput();
        string habit = GetHabitInput();

        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"INSERT INTO habits(date, name, habit, quantity) VALUES('{date}','{name}','{habit}',{quantity},'{unit}')";
            tableCmd.ExecuteNonQuery();

            connection.Close();
        } 
             
    }

    private static string GetMeasurementInput()
    {
        throw new NotImplementedException();
    }

    private static void Update()
    {
        GetAllRecords();

        var dataId = GetNumberInput("Please type the Id of the number you want to update");
        
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var editCmd = connection.CreateCommand();

                editCmd.CommandText =
                $"SELECT EXISTS(SELECT 1 FROM drink_water WHERE Id = {dataId} )";
                 int checkQuery = Convert.ToInt32(editCmd.ExecuteScalar());

                 if (checkQuery == 0)
                 {
                    Console.WriteLine($"Record with Id {dataId} does not exist");
                    connection.Close();
                    Update();
                 }

                string date = GetDateInput();
                int quantity = GetNumberInput("Please enter the number of times you performed the habit");
                string name = GetNameInput();
                string habit = GetHabitInput();

                var tableCmd = connection.CreateCommand();
            
                tableCmd.CommandText = $"UPDATE drink_water SET date = '{date}',name = '{name}', habit = '{habit}', quantity = {quantity} WHERE Id = {dataId}";

                tableCmd.ExecuteNonQuery();

                connection.Close();

            }
    }

    private static void Delete()
    {
        GetAllRecords();
        var dataId = GetNumberInput("Enter the Id of the number you want to delete or type 0 to go back to the main menu");
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"DELETE from drink_water WHERE Id = '{dataId}'";
            
            int rowCount = tableCmd.ExecuteNonQuery();

            if (rowCount==0)
            {
                Console.WriteLine($"Data with Id {dataId} doesn't exist");
                Delete();
            }

            Console.WriteLine($"Data with Id {dataId} was deleted");

            GetUserInput();
        }
    }

    internal static string GetDateInput()
    {
        Console.WriteLine("Please insert date.(Format: dd-mm-yy). Press 0 to return to main menu");

        var dateInput = Console.ReadLine();

        if (dateInput == "0") GetUserInput();


        while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
        {
            Console.WriteLine("Invalid date. (Format: dd-mm-yy). Type 0 to return to main menu");
            dateInput = Console.ReadLine();
        }

        return dateInput;
    }

    internal static int GetNumberInput(string message)
    {

        Console.WriteLine(message);

        var numberInput = Console.ReadLine();
        if (numberInput == "0") GetUserInput();

        while(!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
        {
            Console.WriteLine("Invalid number. Try again.");
            numberInput = Console.ReadLine();
        }
       

       int finalInput = Convert.ToInt32(numberInput);

        return finalInput;

    }

    private static string GetHabitInput()
    {
        Console.WriteLine("Enter habit here");
        var habitInput = Console.ReadLine();    
        if(habitInput == "0") GetUserInput();

        return habitInput;

    }

    private static string GetNameInput()
    {
        Console.WriteLine("What is your name?");
        var nameInput = Console.ReadLine();
        if(nameInput == "0") GetNameInput();

        return nameInput;
    }


}

public class HabitLog
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Name { get; set; }

    public string Habit { get; set; }

    public int Quantity { get; set; }

}
