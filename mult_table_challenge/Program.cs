int rows, cols;
bool isValid;
do
{
    Console.WriteLine("How many rows should the table have?");
    isValid = int.TryParse(Console.ReadLine(), out rows); // if user input is invalid, value is 0
} while (!isValid);

do
{
    Console.WriteLine("How many columns should the table have?");
    isValid = int.TryParse(Console.ReadLine(), out cols); // if user input is invalid, value is 0
} while (!isValid);

Console.WriteLine("\nMultiplication Table:\n");

Console.Write($"{"",6}|");
for (int c = 1; c <= cols; c++)
{
    Console.Write($"{c,6}|");
}

Console.WriteLine($"\n{new string('-', 7 * (cols+1))}");

for (int r = 1; r <= rows; r++)
{
    for (int c = 1; c <= cols; c++)
    {
        if (c == 1) Console.Write($"{r,6}|");
        Console.Write($"{r*c,6}|");
    }
    Console.WriteLine();
}
