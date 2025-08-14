using System;

namespace Warehouse_Inventory_Management_System
{
    // a. Marker Interface
   
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

   
    // b. ElectronicItem
   
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Quantity { get; set; }
        public string Brand { get; private set; }
        public int WarrantyMonths { get; private set; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    
    // c. GroceryItem
    
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; private set; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }

    
    // e. Custom Exceptions
    
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // -----------------------------
    // d. Generic Inventory Repository
    // -----------------------------
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            _items.Remove(id);
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            _items[id].Quantity = newQuantity;
        }
    }

    // -----------------------------
    // f. WarehouseManager
    // -----------------------------
    public class WareHouseManager
    {
        private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            try
            {
                _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
                _electronics.AddItem(new ElectronicItem(2, "Smartphone", 15, "Samsung", 12));
                _electronics.AddItem(new ElectronicItem(3, "TV", 5, "LG", 36));

                _groceries.AddItem(new GroceryItem(1, "Milk", 50, DateTime.Now.AddDays(7)));
                _groceries.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Now.AddDays(3)));
                _groceries.AddItem(new GroceryItem(3, "Apples", 100, DateTime.Now.AddDays(10)));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Error Seeding Data: {ex.Message}");
            }
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var existingItem = repo.GetItemById(id);
                repo.UpdateQuantity(id, existingItem.Quantity + quantity);
                Console.WriteLine($"Stock updated: {existingItem.Name} new quantity is {existingItem.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Updating Stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Removing Item: {ex.Message}");
            }
        }

        // For testing access in Main
        public InventoryRepository<ElectronicItem> GetElectronicsRepo() => _electronics;
        public InventoryRepository<GroceryItem> GetGroceriesRepo() => _groceries;
    }

    //Main Program
    public class Program
    {
        public static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();

            Console.WriteLine("\n--- Grocery Items ---");
            manager.PrintAllItems(manager.GetGroceriesRepo());

            Console.WriteLine("\n--- Electronic Items ---");
            manager.PrintAllItems(manager.GetElectronicsRepo());

            Console.WriteLine("\n--- Testing Exceptions ---");

            // Try to add duplicate
            try
            {
                manager.GetElectronicsRepo().AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Try to remove non-existent
            manager.RemoveItemById(manager.GetGroceriesRepo(), 99);

            // Try to update with invalid quantity
            try
            {
                manager.GetGroceriesRepo().UpdateQuantity(1, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    
}

