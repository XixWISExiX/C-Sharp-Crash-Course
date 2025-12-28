// C# Methods

// Access Modifier - public, protected, internal, private
// Static or nothing
// Return Data Type - int, Student, String, double, void
// Identifier - method name
// Optional Parameter List in params ()

// Method Signature - Name and param list
static void Foo(int bar=5)
{
    Console.WriteLine($"Foo Called {bar}");    
}

Foo();
Foo(2);


static int Foo2(int bar, int barr)
{
    return bar + barr;
}
Console.WriteLine($"Named Parameters passed: {Foo2(barr:6,bar:10)}"); // Named Parameters


static int Foo3(ref int bar, out int barr)
{
    bar = 33;
    barr = 22;
    return bar + barr;
}
int num1 = 1; // ref needs to have a value assigned
int num2; // out doesn't need to have value assigned
Console.WriteLine($"Ref and Out used in Foo: {Foo3(ref num1, out num2)}");
Console.WriteLine($"Ref: {num1}");
Console.WriteLine($"Out: {num2}");


Console.WriteLine($"Overload method 2 params: {Adder.Add(3,4)}");
Console.WriteLine($"Overload method 3 params: {Adder.Add(3,4,3)}");

// Overloaded Methods have the SAME method NAME but a different param list
public class Adder
{
    public static int Add(int a, int b)
    {
        return a + b;
    }
    public static int Add(int a, int b, int c)
    {
        return a + b + c;
    }   
}


