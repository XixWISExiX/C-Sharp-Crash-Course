namespace objs // Allows classes under namespace to be imported to other files
{
    public class Book
    {

        //// C# Property
        //private int _numPages;
        //public int NumPages
        //{
        //    get {return _numPages; }
        //    set { _numPages = value; }
        //}

        // Auto-Implemented Property
        public int NumPages {get; set; } = 200;
        
    }
}
