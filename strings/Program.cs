// STRINGS

string name1 = "Bob";
String name2 = "bob"; // Same as above

// No difference between "string" and "String"
String name3 = new string("Bob");

// This method works in C#, but should be avoided
if (name1 == name3) Console.WriteLine("Names are equal (shallow comparison)");

if (name1.ToLower().Equals(name2.ToLower())) Console.WriteLine("Names are equal (deep comparison)");

// Go through each char in string
foreach (char letter in name1)
{
    Console.WriteLine(letter);
}

Console.WriteLine();

// Reversing a string
string name = "Evan";
for (int i = name.Length - 1; i > -1; i--)
{
    Console.WriteLine(name[i]);
}

bool condition = true;
char res = condition ? 't' : 'f';
Console.WriteLine($"Ternary result: {res}");

string? word1 = null;
string? word2 = "";

// Instance Method Example - identifier.methodName()
Console.WriteLine(word2.ToUpper());

// Static Method Example - Classname.methodName()
if (String.IsNullOrEmpty(word1)) Console.WriteLine("Word1 is null");

do
{
    Console.WriteLine("Enter a word");
    word1 = Console.ReadLine();
} while (String.IsNullOrEmpty(word1));
