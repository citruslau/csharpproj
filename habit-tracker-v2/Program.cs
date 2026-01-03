using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System; // Add this for Console class

namespace habit_tracker;

// 1. Define the Entity (Model)
public class DrinkingWater
{
    [Key]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
}

// 2. Define the Database Context
public class HabitContext : DbContext
{
    public DbSet<DrinkingWater> DrinkingWaterRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(@"Data Source=habit-Tracker-ef.db");
}

class Program
{
    static void Main()
    {
        // Create database if it doesn't exist
        using (var db = new HabitContext())
        {
            // Ensures database and table exist
            db.Database.EnsureCreated();
        }
        GetUserInput();
    }

    static void GetUserInput()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\nMAIN MENU (EF Core version)\n");
            Console.WriteLine("0 to Close");
            Console.WriteLine("1 to View Records");
            Console.WriteLine("2 to Insert");
            Console.WriteLine("3 to Delete");
            Console.WriteLine("4 to Update");

            string command = Console.ReadLine();

            switch (command)
            {
                case "0":
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
                    Console.WriteLine("Invalid Command.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static void GetAllRecords()
    {
        Console.Clear();
        using (var db = new HabitContext())
        {
            var tableData = db.DrinkingWaterRecords.ToList();

            if (!tableData.Any())
            {
                Console.WriteLine("No records found.");
            }
            else
            {
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Date:dd-MMM-yyyy} - Quantity: {dw.Quantity}");
                }
            }
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void Insert()
    {
        DateTime date = GetDateInput();
        int quantity = GetNumberInput("Enter quantity:");

        using (var db = new HabitContext())
        {
            db.DrinkingWaterRecords.Add(new DrinkingWater { Date = date, Quantity = quantity });
            db.SaveChanges();
            Console.WriteLine("Record added successfully!");
        }
    }

    private static void Delete()
    {
        GetAllRecords();
        var recordId = GetNumberInput("Enter Id to delete (0 to return):");
        if (recordId == 0) return;

        using (var db = new HabitContext())
        {
            var record = db.DrinkingWaterRecords.Find(recordId);

            if (record != null)
            {
                db.DrinkingWaterRecords.Remove(record);
                db.SaveChanges();
                Console.WriteLine("Record deleted successfully!");
            }
            else
            {
                Console.WriteLine("Record not found.");
            }
        }
        Console.ReadKey();
    }

    private static void Update()
    {
        GetAllRecords();
        var recordId = GetNumberInput("Enter Id to update (0 to return):");
        if (recordId == 0) return;

        using (var db = new HabitContext())
        {
            var record = db.DrinkingWaterRecords.Find(recordId);

            if (record == null)
            {
                Console.WriteLine("Record not found.");
                Console.ReadKey();
                return;
            }

            record.Date = GetDateInput();
            record.Quantity = GetNumberInput("Enter new quantity:");
            db.SaveChanges();
            Console.WriteLine("Record updated successfully!");
        }
        Console.ReadKey();
    }

    private static DateTime GetDateInput()
    {
        Console.WriteLine("Enter date (dd-mm-yy) or 0 for menu:");
        string input = Console.ReadLine();

        if (input == "0")
        {
            GetUserInput();
            return DateTime.MinValue; // This will never be reached due to GetUserInput loop
        }

        DateTime date;
        while (!DateTime.TryParseExact(input, "dd-MM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            Console.WriteLine("Invalid format. Try again (dd-mm-yy):");
            input = Console.ReadLine();
        }
        return date;
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