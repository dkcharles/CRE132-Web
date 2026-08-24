List<string> names = new List<string> { "Crisps", "Chocolate", "Water", "Juice" };
List<int> prices = new List<int> { 1, 2, 1, 2 };

int choice = int.Parse(Console.ReadLine());

if (choice >= 1 && choice <= names.Count)
{
    int index = choice - 1;
    Console.WriteLine($"You chose {names[index]} - £{prices[index]}");
}
else
{
    Console.WriteLine("Sorry, we don't have that");
}
