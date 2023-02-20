
using Microsoft.Data.Sqlite;
using System.Globalization;

class Program
{
    static string connectionString = @"Data Source=habit-Logger.db";

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
            Console.WriteLine("Type 5 to Create new habit data");

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
                //case "5":
                //    CreateNewHabit();
                //    break;
                default:
                    Console.Clear();
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
                            Habit = reader.GetString(2),
                            Quantity = reader.GetInt32(3),
                            Unit =  reader.GetString(4)
                        });
                }
            }
            else
            {
                Console.WriteLine("No rows found");
            }

            connection.Close();

            Console.WriteLine("----------------------------------------------------------------------------");
            foreach (var hb in tableData)
            {
                Console.WriteLine($"{hb.Id} - {hb.Date.ToString("dd-MMM-yy")} - {hb.Habit} - Quantity: {hb.Quantity} - {hb.Unit}");
            }
            Console.WriteLine("-----------------------------------------------------------------------------");
        }
    }
     
   

    private static void Insert()
    {
        Console.Clear();
        string date = GetDateInput();
        string habit = GetHabitInput();
        int quantity = GetNumberInput("Please Enter the quantity/duration or the number of times you performed this habit");
        string unit = GetUnitInput();

        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"INSERT INTO habits(habit,unit,date,quantity) VALUES('{habit}' ,'{unit}', '{date}' ,{quantity})";
            tableCmd.ExecuteNonQuery();

            connection.Close();
        } 
             
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
                $"SELECT EXISTS(SELECT 1 FROM  WHERE Id = {dataId} )";
                 int checkQuery = Convert.ToInt32(editCmd.ExecuteScalar());

                 if (checkQuery == 0)
                 {
                    Console.WriteLine($"Record with Id {dataId} does not exist");
                    connection.Close();
                    Update();
                 }


                string habit = GetHabitInput();
                string unit = GetUnitInput();
                string date = GetDateInput();
                int quantity = GetNumberInput("Please enter the number of glasses");

                var tableCmd = connection.CreateCommand();
            
                tableCmd.CommandText = $"UPDATE habits SET date = '{date}', quantity = {quantity} WHERE Id = {dataId}";

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
                $"DELETE from habits WHERE Id = '{dataId}'";
            
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

    internal static string GetHabitInput()
    {
        Console.WriteLine("Enter the habit you want to track Ex. Drink Water");

        var habitInput = Console.ReadLine();
        if (habitInput == null) GetHabitInput();
        
        
        return habitInput;
    }

    internal static string GetUnitInput()
    {
        Console.WriteLine("Enter the measuring unit of the habit you want to track Ex. Liters");
        var unitInput = Console.ReadLine(); 

        if(unitInput == null) GetUnitInput();

        return unitInput;
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

}

public class HabitLog
{
    public int Id { get; set; }

    public string? Habit { get; set; }

    public string? Unit { get; set; }

    public DateTime Date { get; set; }

    public int Quantity { get; set; }

}


