namespace backend;

public class Program
{
    public static void Main(string[] args)
    {
        Calculator calc = new Calculator();

        calc.Input("1");
        calc.Input("+");
        calc.Input("1");
        calc.Execute();
    }
}