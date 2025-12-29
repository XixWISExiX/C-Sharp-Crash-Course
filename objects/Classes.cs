namespace objs // Allows classes under namespace to be imported to other files
{
    // Inhertitance is referred to as an "is a" relationship
    public class Person : Object // Inheritance - Person is a subclass of Object
    {
        // Members
        // Instance Fields (data)
        // Methods

        // It's All ABOUT THE INSTANCE FIELDS!
        // All instance fields have default values when an object is created
        // Strings are empty
        // Integers are 0
        // Booleans are false
        // Floating point numbers are 0.0


        private string _firstName; // Data Hiding - Fields are private and can only be accessed/modified by public methods
        private string _lastName;
        private int _age;

        // Constructor - Method that has the same name as the class, no return type
        // Used to initialize the object with other than the default values
        public Person() {} // default constructor
        public Person(string firstName, string lastName) // 2 param constructor
        {
            _firstName = firstName;
            _lastName = lastName;
        }

        public Person(string firstName, string lastName, int age) // 3 param constructor
        {
            _firstName = firstName;
            _lastName = lastName;
            _age = age;
        }


        // Accessor and Mutator Methods
        public string GetFirstName() { return _firstName; }
        public string GetLastName() { return _lastName; }
        public int GetAge() { return _age; }

        //Mutator Method
        public void SetFirstName(string firstName) { _firstName = firstName; }
        public void SetLastName(string lastName) { _lastName = lastName; }
        public void SetAge(int age) { _age = age; }

        public int CalcDaysOld()
        {
            return _age * 365;
        }

        // ToString method is an accessor for MULTIPLE fields
        // This overrides the previous ToString() method inherited by the Object Class.
        public override string ToString()
        {
            return $"The person's name is {_firstName} {_lastName}, with an age of {_age}";
        }

    }

    public class Cart : Object // Inheritance - Person is a subclass of Object
    {
        private string _cartId;
        private Dictionary<String, double> _items;

        public Cart(string cartId)
        {
            _cartId = cartId;
            _items = new Dictionary<String, double>();
        }

        public void AddItem(string item, double price)
        {
            _items.Add(item, price);
        }

        public void RemoveItem(string item)
        {
            _items.Remove(item);
        }

        public double GetTotal()
        {
            double sum = 0;
            foreach (var kvp in _items) sum+=kvp.Value;
            return sum;
        }

        public override string ToString()
        {
            string output;
            output = $"Cart ID: {_cartId}\n";
            foreach (var kvp in _items)
            {
                output += $"{kvp.Key}: ${kvp.Value}\n";
            }
            output += $"Total: ${GetTotal()}\n";
            return output;
        }
    }
}
