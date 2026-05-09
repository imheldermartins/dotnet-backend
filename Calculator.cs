using System.Data;

namespace backend;

public class Calculator
{
    private string MathOperation { get; set; } = "";

    public void Input(string value)
    {
        MathOperation += $" {value.Trim()} ";
    }

    public void Execute()
    {
        var result = new DataTable().Compute(MathOperation, null);

        Console.WriteLine($"> {MathOperation} = {result}");
    }
}
