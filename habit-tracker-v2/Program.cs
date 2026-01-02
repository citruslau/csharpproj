using System.Globalization;
using Microsoft.Data.Sqlite;
using Dapper; // Recommended: Install via NuGet for cleaner data mapping

namespace habit_tracker;

class Program
{
    private const string ConnectionString = @"Data Source=habit-Tracker.db";

    static void Main()
    {
        InitializeDatabase();
        GetUserInput();
    }

    private static void InitializeDatabase()
    {
        ExecuteNonQuery(
            @"CREATE TABLE IF NOT EXISTS drinking_water (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                Quantity INTEGER
            )");
    }

    private static void ExecuteNonQuery(string query, object parameters = null)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Execute(query, parameters);
    }

    static void GetUserInput()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\nMAIN MENU\n");
            Console.WriteLine("0 to Close\n1 to View Records\n2 to Insert\n3 to Delete\n4 to Update");

            string command = Console.ReadLine();

            switch (command)
            {
                case "0": Environment.Exit(0); break;
                case "1": GetAllRecords(); break;
                case "2": Insert(); break;
                case "3": Delete(); break;
                case "4": Update(); break;
                default: Console.WriteLine("Invalid Command."); break;
            }
        }
    }

    private static void GetAllRecords()
    {
        Console.Clear();
        using var connection = new SqliteConnection(ConnectionString);
        var tableData = connection.Query<DrinkingWaterRaw>("SELECT * FROM drinking_water").ToList();

        if (!tableData.Any())
        {
            Console.WriteLine("No records found.");
            return;
        }

        foreach (var dw in tableData)
        {
            DateTime parsedDate = DateTime.ParseExact(dw.Date, "dd-MM-yy", CultureInfo.InvariantCulture);
            Console.WriteLine($"{dw.Id} - {parsedDate:dd-MMM-yyyy} - Quantity: {dw.Quantity}");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void Insert()
    {
        string date = GetDateInput();
        int quantity = GetNumberInput("Enter quantity:");

        ExecuteNonQuery("INSERT INTO drinking_water (Date, Quantity) VALUES (@Date, @Quantity)",
            new { Date = date, Quantity = quantity });
    }

    private static void Delete()
    {
        GetAllRecords();
        var recordId = GetNumberInput("Enter Id to delete (0 to return):");
        if (recordId == 0) return;

        using var connection = new SqliteConnection(ConnectionString);
        int rowsAffected = connection.Execute("DELETE FROM drinking_water WHERE Id = @Id", new { Id = recordId });

        if (rowsAffected == 0) Console.WriteLine("Record not found.");
    }

    private static void Update()
    {
        GetAllRecords();
        var recordId = GetNumberInput("Enter Id to update (0 to return):");
        if (recordId == 0) return;

        string date = GetDateInput();
        int quantity = GetNumberInput("Enter new quantity:");

        ExecuteNonQuery("UPDATE drinking_water SET Date = @Date, Quantity = @Quantity WHERE Id = @Id",
            new { Date = date, Quantity = quantity, Id = recordId });
    }

    private static string GetDateInput()
    {
        Console.WriteLine("Enter date (dd-mm-yy) or 0 for menu:");
        string input = Console.ReadLine();
        if (input == "0") GetUserInput();

        while (!DateTime.TryParseExact(input, "dd-MM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            Console.WriteLine("Invalid format. Try again:");
            input = Console.ReadLine();
        }
        return input;
    }

    private static int GetNumberInput(string message)
    {
        Console.WriteLine(message);
        string input = Console.ReadLine();
        int result;
        while (!int.TryParse(input, out result) || result < 0)
        {
            Console.WriteLine("Invalid number. Try again:");
            input = Console.ReadLine();
        }
        return result;
    }
}

public class DrinkingWaterRaw
{
    public int Id { get; set; }
    public string Date { get; set; }
    public int Quantity { get; set; }
}