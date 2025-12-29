namespace polymorphism
{
    public class Patient : IRecoverable
    {
        public string Recover()
        {
            return "Patient is recovering in the doctors office";
        }
    }
}
