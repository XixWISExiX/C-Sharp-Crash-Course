namespace objs // Allows classes under namespace to be imported to other files
{
    public interface ICourseActions
    {
        // Defines the methods that a class must implement
        // Interface methods are public and abstract by default

        public void StartCourse();
        public void StopCourse();
    }
}
