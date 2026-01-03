using System.Globalization;
using Microsoft.Data.Sqlite;
using Dapper;

namespace to_do;

class Program
{
    private const string ConnectionString = @"Data Source=to-do.db";

    static void Main()
    {
        InitializeDatabase();
        GetUserInput();
    }

    // 1. Setup Database
    private static void InitializeDatabase()
    {
        ExecuteNonQuery(
            @" CREATE TABLE IF NOT EXISTS todo_items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                IsCompleted INTEGER NOT NULL DEFAULT 0
            )");
    }

    public static void ExecuteNonQuery(string query, object parameters = null)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Execute(query, parameters);
    }

    // 2. Get User Input, Call the functions accordingly
    static void GetUserInput()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\nTODO LIST\n");
            Console.WriteLine("0 to Close\n1 to View To-Do Items\n2 to Add To-Do Item\n3 to Delete To-Do Item\n4 to Mark Item as Completed");

            string input = Console.ReadLine();

            switch (input)
            {
                case "0": Environment.Exit(0); break;
                case "1": ViewAllTasks(); break;
                case "2": AddTasks(); break;
                case "3": DeleteTasks(); break;
                case "4": MarkCompleted(); break;
                default: Console.WriteLine("Invalid Command."); break;
            }
        }
    }

    // Read, Create, Delete, Update functions

    // 3. View All To-Do Items
    public static void ViewAllTasks()
    { }

    // 4. Add To-Do Item
    public static void AddTasks()
    { }

    // 5. Delete To-Do Item
    public static void DeleteTasks()
    { }

    // 6. Mark To-Do Item as Completed
    public static void MarkCompleted()
    {

    }


    public class ToDoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }