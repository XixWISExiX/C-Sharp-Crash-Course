using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Contacting;


// appsettings.json is a part of the app (deploy)
// contacts.json is a partof the user's data (create/use)

// Dynamic File Configuration via .NET
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

string path = config["Storage:Path"] ?? "defaultContacts.json";

// Dependency Injection added via .NET
var services = new ServiceCollection();

services.AddLogging(b => b.AddConsole());
services.AddSingleton<IStorage<Contact>>(sp => new FileStorage<Contact>(path));
services.AddSingleton<ContactBook>();

var provider = services.BuildServiceProvider();

var book = provider.GetRequiredService<ContactBook>();
var storage = provider.GetRequiredService<IStorage<Contact>>();

// Dynamic Logging via .NET
var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("App");
logger.LogInformation("Dependency Injection wired up");

// Load
List<Contact> loaded = await storage.LoadAsync();
foreach (Contact c in loaded) book.Upsert(c);

int option = 0;
do
{
    Console.WriteLine("1) Add/Update Contact");
    Console.WriteLine("2) Get Contact");
    Console.WriteLine("3) Delete Contact");
    Console.WriteLine("4) List All Contact");
    Console.WriteLine("5) Exit");
    int.TryParse(Console.ReadLine(), out option);
    switch (option)
    {
        case 1:
            string name, phone;
            string? email;
            Contact c;
            try
            {
                Console.WriteLine("Enter in Contact Name");
                name = Console.ReadLine();
                Console.WriteLine("Enter in Contact Phone Number");
                phone = Console.ReadLine();
                Console.WriteLine("Enter in Contact Email (optional)");
                email = Console.ReadLine();
                c = new Contact(name, phone, email);
                ContactValidator.Validate(c);
                if (!book.Add(c)) book.Upsert(c); // Enter in entry into contact book
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            break;
        case 2:
            Console.WriteLine("Enter in Contact Name");
            name = Console.ReadLine();
            bool result = book.TryGet(name, out c);
            if (!result) Console.WriteLine("Name doesn't exist in Contact Book");
            else Console.WriteLine($"Information on {name}: {c}");
            break;
        case 3:
            Console.WriteLine("Enter in Contact Name");
            name = Console.ReadLine();
            result = book.Delete(name);
            if (!result) Console.WriteLine("Name doesn't exist in Contact Book");
            break;
        case 4:
            foreach (Contact contact in book.SortedByName()) Console.WriteLine(contact);
            break;
        case 5:
            break;
        default:
            Console.WriteLine("Please Enter a Valid Entry");
            logger.LogWarning("InValid Entry Entered: {Value}", 123);
            break;
    }
} while (option != 5);

// Menu loop...
// after modifications:
await storage.SaveAsync(book.ListAll());

logger.LogInformation("App ended");
