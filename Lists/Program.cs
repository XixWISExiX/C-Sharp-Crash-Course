List<int> list = new List<int>();
list.Add(1);
list.Add(2);

Console.WriteLine($"Count: {list.Count}");
Console.WriteLine($"Capacity: {list.Capacity}");
list.TrimExcess();
Console.WriteLine($"Count: {list.Count}");
Console.WriteLine($"Capacity: {list.Capacity}");

foreach (int num in list) Console.WriteLine(num);

Dictionary<int,string> students = new Dictionary<int,string>();
students.Add(222, "Joe");
students.Add(111, "Guy");

Console.WriteLine(students[111]);

if (students.ContainsValue("Bob")) Console.WriteLine("Bob is in the students list");

HashSet<int> group1 = new HashSet<int> {1,2,3};
HashSet<int> group2 = new HashSet<int> {3,4,5};
//group1.UnionWith(group2); // outer join
//group1.IntersectWith(group2); // inner join
group1.ExceptWith(group2);
foreach (int num in group1) Console.Write($"{num} ");


// Contact Project -------------------------------------------------------------------------------


Console.WriteLine();
Dictionary<string, string> contactList = new Dictionary<string, string>();
// John, 111-111-1111
// Joe, 222-222-2222
int option = 0;
do{
    Console.WriteLine();
    Console.WriteLine("Please Enter 1 of the following options:");
    Console.WriteLine("1. Add Contact");
    Console.WriteLine("2. View Contact");
    Console.WriteLine("3. Update Contact");
    Console.WriteLine("4. Delete Contact");
    Console.WriteLine("5. List All Contacts");
    Console.WriteLine("6. Exit");
    int.TryParse(Console.ReadLine(), out option);
    Console.WriteLine();
    switch (option)
    {
        case 1:
            Console.WriteLine("Enter Person's Name");
            string name = Console.ReadLine();
            Console.WriteLine("Enter Person's Phone Number");
            string number = Console.ReadLine();
            contactList.Add(name, number);
            Console.WriteLine($"Added {name} and associated phone number of {number}");
            break;
        case 2:
            Console.WriteLine("Enter Person's Name");
            name = Console.ReadLine();
            Console.WriteLine($"{name}'s phone number: {contactList[name]}");
            break;
        case 3:
            Console.WriteLine("Enter Person's Name");
            name = Console.ReadLine();
            Console.WriteLine("Enter Person's Updated Phone Number");
            number = Console.ReadLine();
            contactList[name] = number;
            Console.WriteLine($"Updated {name} and associated phone number of {number}");
            break;
        case 4:
            Console.WriteLine("Enter Person's Name");
            name = Console.ReadLine();
            contactList.Remove(name);
            Console.WriteLine($"Removed {name} from contacts");
            break;
        case 5:
            foreach (KeyValuePair<string, string> kvp in contactList) Console.WriteLine($"{kvp.Key} : {kvp.Value}");
            break;
        default:
            Console.WriteLine("Enter in a valid number please...");
            break;
    }
} while (option != 6);

