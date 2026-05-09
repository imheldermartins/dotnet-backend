using System.Globalization;

CultureInfo culture = new CultureInfo("pt-BR");

string name = "Helder";
int age = 20;
float height = 1.75f;
double weight = 70.5;
decimal balance = 400.00m;

bool isStudent = true;

Console.WriteLine($"Meu nome é {name}, tenho {age} anos, {height}m de altura e peso {weight}kg.");
Console.WriteLine($"Meu saldo bancário é {balance.ToString("C", culture)}.");

if (isStudent)
{
    Console.WriteLine("Sou um estudante.");
}
else
{
    Console.WriteLine("Não sou um estudante.");
}

for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"Contagem: {i}");
}

int contador = 0;
while (contador < 3)
{
    Console.WriteLine(contador);
    contador++;
}

int[] numbers = { 1, 2, 3, 4, 5 };

foreach (int n in numbers)
{
    Console.WriteLine($"Número: {n}");
}