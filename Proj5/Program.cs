CreditCard creditCard;
// 1111 2222 3333 4444
do{
    Console.WriteLine("Please enter your credit card number.");
    creditCard = new CreditCard(Console.ReadLine());
} while(!creditCard.validCreditCard());

creditCard.displayCensoredCreditCard();



class CreditCard
{
    // readonly means you can only assign the value at initialization.
    private readonly string creditCard;

    public CreditCard(string creditCard)
    {
        this.creditCard = creditCard ?? throw new ArgumentNullException(nameof(creditCard));
    }
    
    public bool validCreditCard()
    {
        foreach (char letter in creditCard)
        {
            if (!Char.IsDigit(letter) && Char.IsLetter(letter)) return false;
        }
        return true;
    }

    public void displayCensoredCreditCard()
    {
        char[] displayCreditCard = creditCard.ToCharArray();

        for (int i = creditCard.Length-(1+4); i > -1; i--)
        {
            if (Char.IsDigit(displayCreditCard[i])) displayCreditCard[i] = 'X';
        }
        Console.WriteLine(new String(displayCreditCard));
    }
}

