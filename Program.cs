namespace backend;

public class Program
{
    public static void Main(string[] args)
    {
        Account calc = new Account("John Doe", 1000m);

        Console.WriteLine($"Account Name: {calc.Name}, Balance: {calc.Balance}");

        calc.AddTransaction(new Transaction(200m, DateTime.Now, "Salary"));
        calc.AddTransaction(new Transaction(-50m, DateTime.Now, "Groceries"));

        calc.ConsultBalance();
        calc.ConsultTransactions();
    }
}