using System;
using System.Collections.Generic;

// See how numbers are displayed differently here: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

List<int> arr = new List<int>();
//var arr = new List<int>(); //same thing as the above line

// var is used when you don't know how to specify the incoming data type. this is STATIC typed, not dynamic

arr.Add(10);
arr.Add(20);
arr.AddRange(new[] { 30, 40 });

//Loops -----------------------------------------------------------------------------------------------------
for (int i = 0; i < arr.Count; i++)
{
    Console.WriteLine($"For Loop: arr[{i}]={arr[i]}");
}

foreach (int item in arr)
{
    Console.WriteLine($"Foreach Loop: {item}");
}

int c = 0;
while (c < arr.Count)
{
    Console.WriteLine($"While Loop: {arr[c]}");
    c++;
}

int e = 100;

do
{
    Console.WriteLine($"The current value of e is {e}");
    e += 1;
} while (e <= 10);

// NOTE: Challange 1, Print 100 98 96 .. 2, DONT PRINT 0!
//Console.WriteLine("Challange 1");
//for (int i = 100; i > 0; i-=2)
//{
//    Console.WriteLine(i);
//}

//Console.WriteLine("Challange 2");
////for (int i = 100; i > 1; i/=2)
//for (float i = 100; i > 1; i/=2)
//{
//    Console.WriteLine(i);
//}



//Conditionals -----------------------------------------------------------------------------------------------------

//int age = 40;
int age = 60;
//int age = 400;

if (age < 50)
{
    Console.WriteLine("You're not over the hill!");
}
else if (age < 75)
{
    Console.WriteLine("You're a hill?");
}
else
{
    Console.WriteLine("You ARE over the hill!");
}

int animalID = 1;
//int animalID = 2;
//int animalID = 5;
//int animalID = 11;

switch (animalID)
{
    case 1:
        Console.WriteLine("Your a dog");
        break;
    case 2:
        Console.WriteLine("Your a cat");
        break;
    case < 10:
        Console.WriteLine("Familiar, but IDK");
        break;
    default:
        Console.WriteLine("Not known animal");
        break;
}


// FizzBuzz
Console.WriteLine("Give upper limit for FizzBuzz?");
//int N = Convert.ToInt32(Console.ReadLine());
int N;
int.TryParse(Console.ReadLine(), out N); // if user input is invalid, value is 0
for (int i = 1; i <= N; i++)
{
    if(i % 3 == 0 && i % 5 == 0) Console.WriteLine("FizzBuzz");
    else if(i % 3 == 0) Console.WriteLine("Fizz");
    else if(i % 5 == 0) Console.WriteLine("Buzz");
    else Console.WriteLine($"{i:c}");
}

//unary
// i++

//binary operator
// i+3

//Conditional Operator (Ternary)

int a = 100;
string result = a < 200 ? "a less than 200" : "greater or equal to 200";
Console.WriteLine(result);

float n = .12f;
Console.WriteLine($"{n:P2}"); // include percentage display
Console.WriteLine(n.ToString("P2")); // same as above line of code
