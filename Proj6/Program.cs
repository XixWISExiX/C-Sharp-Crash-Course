int[] myInts = new int[10]; //Dimensioned
myInts[0] = 1;

String[] myStrings = new String[10];

double[] myDoubles = {1.2, 2.2, 0.4, 0.1, 0.87};

for (int i = 0; i < myDoubles.Length; i++) Console.WriteLine(myDoubles[i]);

Array.Sort(myDoubles);
Console.WriteLine();

for (int i = 0; i < myDoubles.Length; i++) Console.WriteLine(myDoubles[i]);

Dog[] myDogs = new Dog[5];
myDogs[0] = new Dog("Snoop");
myDogs[1] = new Dog("Pancake");
myDogs[2] = new Dog("Gerold");
myDogs[3] = new Dog("Dognessa");
myDogs[4] = new Dog("Ratdog");

for (int i = 0; i < myDogs.Length; i++) Console.WriteLine(myDogs[i].name);


int idx = Array.IndexOf(myDoubles, 2.2);
Console.WriteLine($"Index of 2.2: {idx}");
Console.WriteLine();


int[] testScores = {100, 90, 30, 88, 75, 93};
int bestScore = 0;
int worstScore = 100;
int sumScore = 0;
foreach (int grade in testScores)
{
    if (bestScore < grade) bestScore = grade;
    if (worstScore > grade) worstScore = grade;
    //bestScore = int.Max(bestScore, grade);
    //worstScore = int.Min(worstScore, grade);
    sumScore+=grade;
}
double averageScore = sumScore/testScores.Length;
Console.WriteLine($"Best Score: {bestScore}");
Console.WriteLine($"Worst Score: {worstScore}");
Console.WriteLine($"Average Score: {averageScore}");
Console.WriteLine($"Sum Score: {sumScore}");


class Dog{
    public readonly string name;
    public Dog(string name) {this.name = name;}
}
