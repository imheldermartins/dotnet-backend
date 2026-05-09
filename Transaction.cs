using System.Globalization;

namespace backend;

public class Transaction
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }

    public Transaction(decimal amount, DateTime date, string description)
    {
        Amount = amount;
        Date = date;
        Description = description;
    }

    public override string ToString()
    {
        return $"{Date}: {Description} - {Amount.ToString("C", new CultureInfo("pt-BR"))}";
    }
}
