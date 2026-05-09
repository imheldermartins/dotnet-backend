using System.Collections.Generic;
using System.Globalization;

namespace backend;

public class Account
{
    public string Name { get; set; }
    public decimal Balance { get; private set; }

    private List<Transaction> historyTransactions;

    public Account(string name, decimal balance)
    {
        Name = name;
        Balance = balance;

        historyTransactions = new List<Transaction>();
    }

    public void AddTransaction(Transaction transaction)
    {
        Balance += transaction.Amount;

        historyTransactions.Add(transaction);
    }

    public void RemoveTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;

        historyTransactions.Remove(transaction);
    }

    public void ConsultBalance()
    {
        Console.WriteLine($"Current Balance: {Balance.ToString("C", new CultureInfo("pt-BR"))}");
    }

    public void ConsultTransactions()
    {
        foreach (var transaction in historyTransactions)
        {
            Console.WriteLine(transaction);
        }
    }
}
