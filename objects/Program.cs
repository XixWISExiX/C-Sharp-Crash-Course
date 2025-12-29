// Classes are Blueprints for objects
using objs;

// Create an instance of a person
Person p1 = new Person(); // default constructor - no arguments

Console.WriteLine($"The first person's object first name: {p1.GetFirstName()}");

p1.SetFirstName("Joe");
p1.SetLastName("Bamba");

Console.WriteLine($"The first person's object first name: {p1.GetFirstName()} {p1.GetLastName()}");

Person p2 = new Person("Jane", "Smith");

Console.WriteLine($"The second person's object first name: {p2.GetFirstName()} {p2.GetLastName()}");

Person p3 = new Person("Jon", "Jones", 38);

Console.WriteLine(p3.ToString());

Console.WriteLine(p3.CalcDaysOld());

// ------------------------

Cart c1 = new Cart("1234");
c1.AddItem("Chicken", 9.99);
c1.AddItem("Rice", 4.99);
Console.WriteLine(c1);
c1.RemoveItem("Rice");
Console.WriteLine(c1);

Cart c2 = new Cart("4321");
c2.AddItem("Milk", 3.99);
c2.AddItem("Bread", 3.99);
c2.AddItem("Cheese", 4.99);
Console.WriteLine(c2);

Student s1 = new Student("Jon", "Jones", 38, 2.1);
Console.WriteLine(s1);

Book b1 = new Book();
Console.WriteLine(b1.NumPages); // Getter
b1.NumPages = 100; // Setter
Console.WriteLine(b1.NumPages); // Getter
