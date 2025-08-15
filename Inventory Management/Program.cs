using System.Text.Json;
using System.Text.Json.Serialization;

namespace Inventory_Management
{
    // b. Marker Interface for Logging
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // a. Immutable Inventory Record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // c. Generic Inventory Logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new List<T>();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
            Console.WriteLine($"Added item: {item}");
        }

        public List<T> GetAll()
        {
            return _log;
        }

        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                };

                string jsonString = JsonSerializer.Serialize(_log, options);

                using (var writer = new StreamWriter(_filePath))
                {
                    writer.Write(jsonString);
                }

                Console.WriteLine($"Successfully saved {_log.Count} items to {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("No existing data file found. Starting with empty inventory.");
                    return;
                }

                using (var reader = new StreamReader(_filePath))
                {
                    string jsonString = reader.ReadToEnd();
                    _log = JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
                }

                Console.WriteLine($"Successfully loaded {_log.Count} items from {_filePath}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON data: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    // f. Integration Layer - InventoryApp
    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp()
        {
            _logger = new InventoryLogger<InventoryItem>("inventory.json");
        }

        public void SeedSampleData()
        {
            Console.WriteLine("\nSeeding sample data...");
            _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now.AddDays(-30)));
            _logger.Add(new InventoryItem(2, "Monitor", 15, DateTime.Now.AddDays(-15)));
            _logger.Add(new InventoryItem(3, "Keyboard", 25, DateTime.Now.AddDays(-7)));
            _logger.Add(new InventoryItem(4, "Mouse", 50, DateTime.Now.AddDays(-3)));
            _logger.Add(new InventoryItem(5, "Headphones", 20, DateTime.Now));
        }

        public void SaveData()
        {
            Console.WriteLine("\nSaving data to file...");
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            Console.WriteLine("\nLoading data from file...");
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            Console.WriteLine("\nCurrent Inventory:");
            Console.WriteLine("------------------");
            var items = _logger.GetAll();

            if (items.Count == 0)
            {
                Console.WriteLine("No items in inventory.");
                return;
            }

            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Added: {item.DateAdded:yyyy-MM-dd}");
            }
        }
    }

    // g. Main Application
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inventory Management System");
            Console.WriteLine("==========================");

            // Create app instance
            var app = new InventoryApp();

            // First session - create and save data
            Console.WriteLine("\n=== Session 1: Creating and Saving Data ===");
            app.SeedSampleData();
            app.SaveData();

            // Simulate new session by creating a new instance
            Console.WriteLine("\n=== Session 2: Loading and Displaying Data ===");
            var newAppInstance = new InventoryApp();
            newAppInstance.LoadData();
            newAppInstance.PrintAllItems();
        }
    }
}