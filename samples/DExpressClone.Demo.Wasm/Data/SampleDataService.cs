namespace DExpressClone.Demo.Wasm.Data;

public class SampleDataService
{
    private static readonly string[] FirstNames =
    {
        "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda",
        "David", "Elizabeth", "William", "Barbara", "Richard", "Susan", "Joseph", "Jessica"
    };

    private static readonly string[] LastNames =
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas"
    };

    private static readonly string[] Departments =
    {
        "Engineering", "Marketing", "Sales", "Human Resources", "Finance",
        "Operations", "Customer Support", "Research"
    };

    public List<Employee> GenerateEmployees(int count)
    {
        var random = new Random(42);
        var employees = new List<Employee>(count);
        for (int i = 1; i <= count; i++)
        {
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            employees.Add(new Employee
            {
                Id = i,
                FirstName = firstName,
                LastName = lastName,
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@company.com",
                Department = Departments[random.Next(Departments.Length)],
                Salary = Math.Round(40000 + random.NextDouble() * 120000, 2),
                HireDate = DateTime.Today.AddDays(-random.Next(365 * 10)),
                IsActive = random.NextDouble() > 0.15
            });
        }
        return employees;
    }

    public List<SalesData> GenerateMonthlySales()
    {
        var random = new Random(77);
        var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        return months.Select((m, i) =>
        {
            var revenue = 50000 + random.NextDouble() * 100000;
            var expenses = 30000 + random.NextDouble() * 60000;
            return new SalesData { Month = m, MonthIndex = i + 1, Revenue = Math.Round(revenue, 2), Expenses = Math.Round(expenses, 2), Profit = Math.Round(revenue - expenses, 2) };
        }).ToList();
    }

    public static List<string> GetDepartments() => new(Departments);
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

public class SalesData
{
    public string Month { get; set; } = string.Empty;
    public int MonthIndex { get; set; }
    public double Revenue { get; set; }
    public double Expenses { get; set; }
    public double Profit { get; set; }
}
