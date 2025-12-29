namespace objs // Allows classes under namespace to be imported to other files
{
    // Animal >> Dog >> Poodle
    // Single Inheritance in C# means you can only have 1 parent
    // However you CAN implement multiple interfaces
    public class Student : Person, ICourseActions
    {
        // instance field
        private double _gpa;
        //Stduent will inherit the non-private fields and methods of Person
        public Student() {}

        public Student(string firstName, string lastName, int age, double gpa) : base(firstName, lastName, age)
        {
            _gpa = gpa;
        }

        public double GetGPA() { return _gpa; }

        public void StartCourse()
        {
            Console.WriteLine("Student is STARTING a course");
        }

        public void StopCourse()
        {
            Console.WriteLine("Student is ENDING a course");
        }

        public override string ToString()
        {
            return base.ToString() + " their gpa is " + _gpa;
        }
    }
}
