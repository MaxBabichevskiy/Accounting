using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Diagnostics;

namespace Accounting {
    internal class Program 
    {

        private static string connectionString = @"Data Source = MSI; Initial Catalog = Accounting; Trusted_Connection=True; Encrypt=False";

            static void Main(string[] args)
            {

                Console.WriteLine("Выберите действие");
                Console.WriteLine("1. Регистрация");
                Console.WriteLine("2. Вход");
                var action = Console.ReadLine();

                switch (action)
                {
                    case "1":
                    CreateUser();
                    break;
                    case "2":
                    Login();
                    break;
                }            
            }

        private static void Login()
        {
            Console.WriteLine("Введите логин");
            var login = Console.ReadLine();

            Console.WriteLine("Введите пароль");
            var password = Console.ReadLine();

            if (login == "admin" && password == "admin") { Admin(); }
            else
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var user = connection.QueryFirstOrDefault<Users>("SELECT * FROM Users WHERE Login = @Login AND Password = @Password", new { Login = login, Password = password });
                    if (user != null)
                    {
                        Console.WriteLine("Вход выполнен успешно.");
                        UserPanel();
                    }
                    else
                    {
                        Console.WriteLine("Неверный логин или пароль.");
                    }
                }
            }

            
        }

        private static void Admin()
        {
            AdminPanelMenu();
        }

        private static void AdminPanelMenu() {

            Console.WriteLine("Выберите действие");
            Console.WriteLine("1. Клиенты");
            Console.WriteLine("2. Товары");
            var action = Console.ReadLine();

            switch (action)
            {
                case "1":
                    ClientsPanel();
                    break;
                case "2":
                    ProductPanel();
                    break;
            }      
        }

        private static void ProductPanel()
        {
            Console.WriteLine("Выберите действие");
            Console.WriteLine("1. Создать новый товар");
            Console.WriteLine("2. Редактировать товар");
            Console.WriteLine("3. Создать заказ");
            Console.WriteLine("4. Создать статус заказа");
            Console.WriteLine("5. Узнать статус заказа");
            Console.WriteLine("6. Обновить статус заказа");
            Console.WriteLine("7. Отобразить все товары");
            var action = Console.ReadLine();

            switch (action)
            {
                case "1":
                    CreateProduct();
                    break;
                case "2":
                    UpdateProduct();
                    break;
                case "3":
                    CreateOrder();
                    break;
                case "4":
                    CreateOrderStatus();
                    break;
                case "5":
                    OrderStatus();
                    break;
                case "6":
                    UpdateOrderStatus();
                    break;                                 
                case "7":
                    Market();
                    break;
            }
        }

        private static void CreateProduct() {
            Console.WriteLine("Введите название товара");
            var productName = Console.ReadLine();

            Console.WriteLine("Введите описание");
            var description = Console.ReadLine();

            Console.WriteLine("Введите цену");
            var price = float.Parse(Console.ReadLine());


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var newProduct = new Product { ProductName = productName, Description = description, Price = price };
                var rows = connection.Execute("INSERT INTO Product VALUES(@ProductName, @Description, @Price)", newProduct);

                if (rows > 0)
                {
                    Console.WriteLine("Товар успешно добавлен");
                }
                else
                {
                    Console.WriteLine("Ошибка при добавлении ");
                }
            }
        }
        private static void UpdateProduct() {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Console.WriteLine("Введите Id");
                var id = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Введите название товара");
                var productName = Console.ReadLine();

                Console.WriteLine("Введите описание");
                var description = Console.ReadLine();

                Console.WriteLine("Введите цену");
                var price = float.Parse(Console.ReadLine());

                int rowsAffected = connection.Execute("UPDATE Product SET ProductName = @productName, Description = @description, Price = @price WHERE Id = @Id", new { ProductName = productName, Description = description, Price = price, Id = id });

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Обновление выполнено успешно.");
                }
                else
                {
                    Console.WriteLine("Товар не найден или обновление не выполнено.");
                }

                var product = connection.QueryFirstOrDefault<Product>("SELECT * FROM Product WHERE Id = @Id", new { Id = id });

                if (product != null)
                {
                    Console.WriteLine($"Id: {product.Id}");
                    Console.WriteLine($"ProductName: {product.ProductName}");
                    Console.WriteLine($"Description: {product.Description}");
                    Console.WriteLine($"Price: {product.Price}");
                    Console.WriteLine("---------------------");
                }
                else
                {
                    Console.WriteLine("Товар не найден.");
                }
            }
        }
        private static void CreateOrder() {
            Console.WriteLine("Введите ID пользователя");
            var userId = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите ID товара");
            var productId = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите количество");
            var quantity = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите адрес");
            var adress = Console.ReadLine();


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var newOrder = new Orders { UserId = userId, ProductId = productId, Quantity = quantity, DeliveryAddress = adress };
                var rows = connection.Execute("INSERT INTO Orders VALUES(@UserId, @ProductId, @Quantity, @DeliveryAddress)", newOrder);

                if (rows > 0)
                {
                    Console.WriteLine("Заказ успешно добавлен");
                }
                else
                {
                    Console.WriteLine("Ошибка при добавлении ");
                }
            }

        }

        private static void CreateOrderStatus()
        {
            Console.WriteLine("Введите ID заказа");
            var orderId = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите статус");
            var status = Console.ReadLine();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var newOrderStatus = new OrderStatus { OrderId = orderId, Status = status};
                var rows = connection.Execute("INSERT INTO OrderStatus VALUES(@OrderId, @Status)", newOrderStatus);

                if (rows > 0)
                {
                    Console.WriteLine("Статус успешно добавлен");
                }
                else
                {
                    Console.WriteLine("Ошибка при добавлении ");
                }
            }
        }

            private static void OrderStatus() {
            Console.WriteLine("Введите ID заказа");
            var orderId = int.Parse(Console.ReadLine());

            using (var connection = new SqlConnection(connectionString))
            {
                var status = connection.QueryFirstOrDefault<OrderStatus>("SELECT * FROM OrderStatus WHERE Id = @Id", new { Id = orderId });

                if (status != null)
                {
                    Console.WriteLine($"Id: {status.Id}");
                    Console.WriteLine($"OrderId: {status.OrderId}");
                    Console.WriteLine($"Status: {status.Status}");
                    Console.WriteLine("---------------------");
                }
                else
                {
                    Console.WriteLine("Статус не найден.");
                }
            }
        }

        private static void UpdateOrderStatus() {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Console.WriteLine("Введите Id");
                var id = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Введите ID заказа");
                var orderId = int.Parse(Console.ReadLine());

                Console.WriteLine("Введите статус");
                var status = Console.ReadLine();

                int rowsAffected = connection.Execute("UPDATE OrderStatus SET OrderId = @orderId, Status = @status WHERE Id = @Id", new { OrderId = orderId, Status = status, Id = id });

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Обновление выполнено успешно.");
                }
                else
                {
                    Console.WriteLine("Заказ не найден или обновление не выполнено.");
                }

                var orderStatus = connection.QueryFirstOrDefault<OrderStatus>("SELECT * FROM OrderStatus WHERE Id = @Id", new { Id = id });

                if (orderStatus != null)
                {
                    Console.WriteLine($"Id: {orderStatus.Id}");
                    Console.WriteLine($"ProductName: {orderStatus.OrderId}");
                    Console.WriteLine($"Description: {orderStatus.Status}");                  
                    Console.WriteLine("---------------------");
                }
                else
                {
                    Console.WriteLine("Заказ не найден.");
                }
            }
        }

        private static void ClientsPanel()
        {
            Console.WriteLine("Выберите действие");
            Console.WriteLine("1. Создать нового клиента");
            Console.WriteLine("2. Получить список всех клиентов");
            Console.WriteLine("3. Получить клиента по ID");
            Console.WriteLine("4. Обновить клиента");
            Console.WriteLine("5. Удалить клиента");
            var action = Console.ReadLine();

            switch (action)
            {
                case "1":
                    CreateUser();
                    break;
                case "2":
                    UserList();
                    break;
                case "3":
                    GetClientWithId();
                    break;
                case "4":
                    UpdateClient();
                    break;
                case "5":
                    DeleteClient();
                    break;
            }

        }


        private static void UserPanel()
        {
            Console.WriteLine("Выберите действие");
            Console.WriteLine("1. Посмотреть товары");
            Console.WriteLine("2. Купить");
            Console.WriteLine("3. Статус");
            
            var action = Console.ReadLine();

            switch (action)
            {
                case "1":
                    Market();
                    break;
                case "2":
                    CreateOrder();
                    break;
                case "3":
                    OrderStatus();
                    break;
            }
        }


        private static void Market() {

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var products = connection.Query<Product>("SELECT * FROM Product").ToList();
                foreach (var product in products)
                {
                    Console.WriteLine($"Id: {product.Id}");
                    Console.WriteLine($"ProductName: {product.ProductName}");
                    Console.WriteLine($"Description: {product.Description}");
                    Console.WriteLine($"Price: {product.Price}");           
                    Console.WriteLine("---------------------");
                }
            }
        }           

        private static void CreateUser()
        {
            //Console.WriteLine("Введите Id");
            //var id = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Введите логин");
            var login = Console.ReadLine();

            Console.WriteLine("Введите имя");
            var name = Console.ReadLine();

            Console.WriteLine("Введите фамилию");
            var lastName = Console.ReadLine();

            Console.WriteLine("Введите пароль");
            var password = Console.ReadLine();

            Console.WriteLine("Введите город");
            var city = Console.ReadLine();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var newCustomer = new Users {Login = login, Name = name, LastName = lastName, Password = password, City = city };
                var rows = connection.Execute("INSERT INTO Users VALUES(@Login, @Name, @LastName, @Password, @City)", newCustomer);

                if (rows > 0)
                {
                    Console.WriteLine("Клиент успешно добавлен");
                }
                else
                {
                    Console.WriteLine("Ошибка при добавлении ");
                }
            }
        }

       
        private static void UserList()
        {       
                using (var connection = new SqlConnection(connectionString))
                {
                connection.Open();
                var users = connection.Query<Users>("SELECT * FROM Users").ToList();
                foreach (var user in users)
                {
                    Console.WriteLine($"Id: {user.Id}");
                    Console.WriteLine($"Login: {user.Login}");
                    Console.WriteLine($"Name: {user.Name}");
                    Console.WriteLine($"LastName: {user.LastName}");
                    Console.WriteLine($"Password: {user.Password}");
                    Console.WriteLine($"City: {user.City}");
                    Console.WriteLine("---------------------");
                }
                }           
        }

        private static void GetClientWithId()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Введите Id");
                var id = Convert.ToInt32(Console.ReadLine());               
                var user = connection.QueryFirstOrDefault<Users>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });

                if (user != null)
                {
                    Console.WriteLine($"Id: {user.Id}");
                    Console.WriteLine($"Login: {user.Login}");
                    Console.WriteLine($"Name: {user.Name}");
                    Console.WriteLine($"LastName: {user.LastName}");
                    Console.WriteLine($"Password: {user.Password}");
                    Console.WriteLine($"City: {user.City}");
                }
                else
                {
                    Console.WriteLine("Пользователь не найден.");
                }
            }
        }

        private static void UpdateClient()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Console.WriteLine("Введите Id");
                var id = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Введите логин");
                var login = Console.ReadLine();

                Console.WriteLine("Введите имя");
                var name = Console.ReadLine();

                Console.WriteLine("Введите фамилию");
                var lastName = Console.ReadLine();

                Console.WriteLine("Введите пароль");
                var password = Console.ReadLine();

                Console.WriteLine("Введите город");
                var city = Console.ReadLine();
              
                int rowsAffected = connection.Execute("UPDATE Users SET Login = @Login, Name = @Name, LastName = @LastName, Password = @Password, City = @City WHERE Id = @Id", new { Login = login, Name = name, LastName = lastName, Password = password, City = city, Id = id });

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Обновление выполнено успешно.");
                }
                else
                {
                    Console.WriteLine("Пользователь не найден или обновление не выполнено.");
                }     
                
                var user = connection.QueryFirstOrDefault<Users>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });

                if (user != null)
                {
                    Console.WriteLine($"Id: {user.Id}");
                    Console.WriteLine($"Login: {user.Login}");
                    Console.WriteLine($"Name: {user.Name}");
                    Console.WriteLine($"LastName: {user.LastName}");
                    Console.WriteLine($"Password: {user.Password}");
                    Console.WriteLine($"City: {user.City}");
                }
                else
                {
                    Console.WriteLine("Пользователь не найден.");
                }
            }
        }

        private static void DeleteClient()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Console.WriteLine("Введите Id");
                var id = Convert.ToInt32(Console.ReadLine());

                int rowsAffected = connection.Execute("DELETE FROM Users WHERE Id = @Id", new { Id = id });

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Запись успешно удалена.");
                }
                else
                {
                    Console.WriteLine("Запись не найдена.");
                }
            }
        }





























    }
}