using polymorphism;
using challange;

Patient p1 = new Patient();
FootballPlayer p2 = new FootballPlayer();
Upholsterer p3 = new Upholsterer();

// Polymorphism states that a methods behavior will change based on the object that it references
Recovery(p1);
Recovery(p2);
Recovery(p3);
void Recovery(IRecoverable r) // Any class that implements IRecoverable can be passed in
{
    Console.WriteLine(r.Recover());
}

Foo(p2);

void Foo(Object o) // Any child of the data tyhpe Object
{
    Console.WriteLine(o.ToString());
}


// -----------------
Console.WriteLine("\nChallange Start:");

List<ITurnable> turnList = new List<ITurnable>();
turnList.Add(new Page());
turnList.Add(new Corner());
turnList.Add(new Pancake());
turnList.Add(new Leaf());

turnAll(turnList);

void turnAll(List<ITurnable> list)
{
    foreach (ITurnable i in list) Console.WriteLine(i.Turn());
}
