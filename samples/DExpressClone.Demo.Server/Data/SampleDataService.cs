namespace DExpressClone.Demo.Server.Data;

public class SampleDataService
{
    private static readonly string[] FirstNames =
    {
        "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda",
        "David", "Elizabeth", "William", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
        "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Lisa", "Daniel", "Nancy",
        "Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra"
    };

    private static readonly string[] LastNames =
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
        "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson",
        "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson"
    };

    private static readonly string[] Departments =
    {
        "Engineering", "Marketing", "Sales", "Human Resources", "Finance",
        "Operations", "Customer Support", "Research", "Legal", "Product Management"
    };

    private static readonly string[] Categories =
    {
        "Electronics", "Clothing", "Home & Garden", "Sports", "Books",
        "Toys", "Automotive", "Health", "Food", "Office Supplies"
    };

    private static readonly string[] ProductNames =
    {
        "Wireless Mouse", "USB-C Hub", "Mechanical Keyboard", "Monitor Stand", "Webcam HD",
        "Laptop Sleeve", "Phone Charger", "Desk Lamp", "Noise Cancelling Headphones",
        "Portable SSD", "Ergonomic Chair", "Standing Desk Mat", "Cable Organizer",
        "Screen Protector", "Bluetooth Speaker", "Power Bank", "HDMI Cable",
        "Wireless Earbuds", "Tablet Stand", "Smart Watch"
    };

    public List<Employee> GenerateEmployees(int count)
    {
        var random = new Random(42);
        var employees = new List<Employee>(count);

        for (int i = 1; i <= count; i++)
        {
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var department = Departments[random.Next(Departments.Length)];
            var hireDate = DateTime.Today.AddDays(-random.Next(365 * 10));

            employees.Add(new Employee
            {
                Id = i,
                FirstName = firstName,
                LastName = lastName,
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@company.com",
                Department = department,
                Salary = Math.Round(40000 + random.NextDouble() * 120000, 2),
                HireDate = hireDate,
                IsActive = random.NextDouble() > 0.15
            });
        }

        return employees;
    }

    public List<Product> GenerateProducts(int count)
    {
        var random = new Random(123);
        var products = new List<Product>(count);

        for (int i = 1; i <= count; i++)
        {
            products.Add(new Product
            {
                Id = i,
                Name = ProductNames[random.Next(ProductNames.Length)] + (i > ProductNames.Length ? $" v{i}" : ""),
                Category = Categories[random.Next(Categories.Length)],
                Price = Math.Round(5 + random.NextDouble() * 500, 2),
                Stock = random.Next(0, 1000),
                Rating = Math.Round(1 + random.NextDouble() * 4, 1)
            });
        }

        return products;
    }

    public List<SalesData> GenerateMonthlySales()
    {
        var random = new Random(77);
        var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        var sales = new List<SalesData>();

        for (int i = 0; i < 12; i++)
        {
            var revenue = 50000 + random.NextDouble() * 100000;
            var expenses = 30000 + random.NextDouble() * 60000;
            sales.Add(new SalesData
            {
                Month = months[i],
                MonthIndex = i + 1,
                Revenue = Math.Round(revenue, 2),
                Expenses = Math.Round(expenses, 2),
                Profit = Math.Round(revenue - expenses, 2)
            });
        }

        return sales;
    }

    public List<DepartmentSales> GenerateDepartmentSales()
    {
        var random = new Random(99);
        return Departments.Take(6).Select(dept => new DepartmentSales
        {
            Department = dept,
            TotalSales = Math.Round(100000 + random.NextDouble() * 500000, 2)
        }).ToList();
    }

    public static List<string> GetDepartments() => new(Departments);
    public static List<string> GetCategories() => new(Categories);
}

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public double Salary { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Stock { get; set; }
    public double Rating { get; set; }
}

public class SalesData
{
    public string Month { get; set; } = string.Empty;
    public int MonthIndex { get; set; }
    public double Revenue { get; set; }
    public double Expenses { get; set; }
    public double Profit { get; set; }
}

public class DepartmentSales
{
    public string Department { get; set; } = string.Empty;
    public double TotalSales { get; set; }
}
