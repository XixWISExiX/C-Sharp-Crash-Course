using System;
using System.Text;

class Person
{
    public int Age; // 4 bytes
    public double Height; // 8 bytes
}

interface IGreeter { string Greet(); }

class Friendly : IGreeter
{
    public string Greet() => "hi";
}


class Program
{
    // Need to run the code in bash to run unsafe code: dotnet run -p:AllowUnsafeBlocks=true
    static unsafe void pointerTest() // static makes function work normally
    {
        byte* buf = stackalloc byte[5]; // allocated on the stack, contiguous
        buf[0] = 72; buf[1] = 101; buf[2] = 108; buf[3] = 108; buf[4] = 111;
        Console.WriteLine($"Address of stack {(IntPtr)buf}");  // prints an address

        byte[] arr = { 1, 2, 3 };
        fixed (byte* p = arr) // pins arr during the fixed block
        {
            Console.WriteLine($"Pointer was attached to array. p[1]={p[1]}"); // 2
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Console.WriteLine($"Running on: {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
        //TODO 1: Make date types.
        //TODO 2: Look at video and continue to Proj2: https://youtu.be/oZAX_cUTg6U?si=V1Hz0ehLz6WrCS51&t=2442

        //VALUE TYPES (takes up one memory address) -------------------------------------------------------------------------

        //byte - 8 bits (0 to 255)
        byte b = 128;
        Console.WriteLine($"Byte Result: {b}");
        //sbyte - 8 bits (-128 to 127)
        sbyte sb = -6;
        Console.WriteLine($"Signed Byte Result: {sb}");
        //short - 16 bits (-32,768 to 32,767)
        short sh = 2_000;
        Console.WriteLine($"Short Result: {sh}");
        //int - 32 bits (-2,147,483,648 to 2,147,483,647)
        int i = 300_000;
        Console.WriteLine($"Int Result: {i}");
        // char - 16 bits in C# (UTF-16 code unit)  // note: not 8 bits in C#
        char c = 'a';
        Console.WriteLine($"Char Result: {c}");
        //long - 64 bits (-9,223,372,036,854,775,808 to 9,223,372,036,854,775,807)
        long l = 7_000_000_000L;
        Console.WriteLine($"Long Result: {l}");

        //Data Types can be marked as nullable -------------------------------------------------------------------------

        // nullable
        int? myInt = null; // is a class too
        int? aInt = 3; // is a class too
        Int32 myInt32 = new Int32(); // is a class obviously
        //not nullable
        int bInt = 4;

        Console.WriteLine($"Using to string function {aInt.ToString()}");
        Console.WriteLine($"Printing nullable int {myInt}");
        Console.WriteLine($"Printing non-nullable int {bInt}");

        //Signed or Unsigned (this changes the range to not include negative numbers)

        //floats; 32 bits (~7 decimals of precision)
        float f = 3.1415926f;
        Console.WriteLine($"Float: {f}");
        //doubles; 64 bits (~15 decimals of precision)
        double tiny = 1e-10;
        Console.WriteLine($"Double: {tiny}");
        //decimal; 128 bits (~28 decimals of precision)
        decimal price = 19.99m;
        decimal tax = 0.0825m;
        decimal total = price * (1m + tax);
        Console.WriteLine($"Decimal: {total}");

        //booleans 8 bits in reality, but conceptually only 1 bit.
        bool isReady = true;
        Console.WriteLine($"Boolean: isReady={isReady}");

        //REFERENCE or OBJECT (takes up multiple memory addresses [ideally contiguous])
        //String; 16 bits per character
        string s = "Hello";     // 5 chars => ~10 bytes of char data (plus overhead)
        Console.WriteLine($"String: {s}, len={s.Length}");   // 5

        //Objects; 8 bytes worth of overhead on a modern 64 bit computer for reference variable
        Person p = new Person { Age = 23, Height = 1.8 };
        Console.WriteLine($"Person Object: {p}, Age={p.Age}, Height={p.Height}");
        // p (the variable) is just a reference (4 or 8 bytes).
        // The Person object is elsewhere on the heap.
        Console.WriteLine($"Reference Variable Size: {IntPtr.Size}"); // 8 on x64

        //Arrays; 24 bytes worth of over head for array object specifically
        // Example of using array of bytes to make 8-bit characters (overhead is ~24 bytes)
        byte[] asciiBytes = { 72, 101, 108, 108, 111 }; // "Hello" in ASCII
        string text = Encoding.ASCII.GetString(asciiBytes);
        Console.WriteLine($"Hello in ascii bytes array: {text}");

        //Interfaces; a reference object relationship, so an extra 8 bytes of overhead
        IGreeter g = new Friendly();
        //Friendly g = new Friendly(); // This works too obviously
        // g is a reference; the Friendly object is on the heap.
        Console.WriteLine($"Interface forcing Friendly Class to greet: {g.Greet()}");

        //Dynamic; Varies in size, but has a reference object relationship too. Just the object can change or at least the content due to the underlying datatype needed.
        dynamic x = "hello";
        Console.WriteLine($"Dynamic variable call 1: {x.ToUpper()}"); // resolved at runtime
        x = 123;
        Console.WriteLine($"Dynamic variable call 2: {x+7}"); // different runtime behavoir, still dynamic

        //Pointer; 8 bytes on 64 bit systems
        pointerTest();

        //Having User Input
        Console.WriteLine("Please enter your name:");
        string name = Console.ReadLine();
        Console.WriteLine("Please enter your age:");
        int age = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine($"Hello {name} you are {age} years old.");
    }
}
