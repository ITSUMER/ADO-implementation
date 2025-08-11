using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ADO_NET_POS_Console
{
    public class Program
    {
        public string connString = "Server=localhost\\MSSQLSERVER02;Database=POS_DB;Trusted_Connection=True;TrustServerCertificate=True";

        public static void Main()
        {
            Program app = new Program();
            while (true)
            {
                Console.WriteLine("\nPOS ADO.NET Console App");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. View All Products");
                Console.WriteLine("3. Update Product Stock");
                Console.WriteLine("4. Delete Product");
                Console.WriteLine("5. View All Customers");
                Console.WriteLine("6. Get Order Total");
                Console.WriteLine("0. Exit");
                Console.Write("Choose option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": app.AddProduct(); break;
                    case "2": app.ViewAllProducts(); break;
                    case "3": app.UpdateProductStock(); break;
                    case "4": app.DeleteProduct(); break;
                    case "5": app.ViewAllCustomers(); break;
                    case "6": app.GetOrderTotal(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        public void AddProduct()
        {
            Console.Write("Enter Product Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price.");
                return;
            }

            Console.Write("Enter Stock: ");
            if (!int.TryParse(Console.ReadLine(), out int stock))
            {
                Console.WriteLine("Invalid stock.");
                return;
            }

            Console.WriteLine("Select Category (1-3):");
            Console.WriteLine("1 - Electronics");
            Console.WriteLine("2 - Clothing");
            Console.WriteLine("3 - Food");
            Console.Write("Enter choice: ");
            string catChoice = Console.ReadLine();

            string category = "";
            switch (catChoice)
            {
                case "1": category = "Electronics"; break;
                case "2": category = "Clothing"; break;
                case "3": category = "Food"; break;
                default:
                    Console.WriteLine("Invalid category selection.");
                    return;
            }

            Console.WriteLine("Select Size (1-3):");
            Console.WriteLine("1 - Small");
            Console.WriteLine("2 - Medium");
            Console.WriteLine("3 - Large");
            Console.Write("Enter choice: ");
            string sizeChoice = Console.ReadLine();

            string size = "";
            switch (sizeChoice)
            {
                case "1": size = "Small"; break;
                case "2": size = "Medium"; break;
                case "3": size = "Large"; break;
                default:
                    Console.WriteLine("Invalid size selection.");
                    return;
            }

            using SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("AddProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@Stock", stock);
            cmd.Parameters.AddWithValue("@Category", category);
            cmd.Parameters.AddWithValue("@Size", size);

            cmd.ExecuteNonQuery();
            Console.WriteLine("Product added successfully.");
        }

        public void ViewAllProducts()
        {
            using SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("ViewAllProducts", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nAll Products:");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Name: {reader["Name"]}, Price: {reader["Price"]}, Stock: {reader["Stock"]}, Category: {reader["Category"]}, Size: {reader["Size"]}");
            }
        }

        public void UpdateProductStock()
        {
            Console.Write("Enter Product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            Console.Write("Enter New Stock Value: ");
            if (!int.TryParse(Console.ReadLine(), out int newStock))
            {
                Console.WriteLine("Invalid stock.");
                return;
            }

            using SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("UpdateProductStock", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@NewStock", newStock);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Stock updated successfully.");
        }

        public void DeleteProduct()
        {
            Console.Write("Enter Product ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            using SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("DeleteProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductId", id);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Product deleted successfully.");
        }

        public void ViewAllCustomers()
        {
            using SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("ViewAllCustomers", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nAll Customers:");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Name: {reader["Name"]}, Contact: {reader["Contact"]}");
            }
        }

        public void GetOrderTotal()
        {
            Console.Write("Enter Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid Order ID.");
                return;
            }

            using SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT dbo.GetOrderTotal(@OrderId) AS Total", conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            object result = cmd.ExecuteScalar();

            if (result != null)
            {
                Console.WriteLine($"Order Total: {result} PKR");
            }
            else
            {
                Console.WriteLine("Order not found.");
            }
        }
    }
}
