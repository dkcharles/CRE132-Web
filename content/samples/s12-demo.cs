List<string> names = new List<string> { "Crisps", "Chocolate", "Water", "Juice" };
List<int> prices = new List<int> { 1, 2, 1, 2 };

for (int i = 0; i < names.Count; i++)
{
    Console.WriteLine(MenuLine(i + 1, names[i], prices[i]));
}

Console.WriteLine("Enter an item number, or 0 to finish:");

// total is declared before the loop, so it survives from one order to the next.
int total = 0;
int choice = int.Parse(Console.ReadLine());

while (choice != 0)
{
    if (choice >= 1 && choice <= names.Count)
    {
        // Only safe now the range check has passed: the menu number counts from 1, the index from 0.
        int index = choice - 1;
        Console.WriteLine($"You chose {names[index]} - £{prices[index]}");
        total = total + prices[index];
    }
    else
    {
        Console.WriteLine("Sorry, we don't have that");
    }

    // Read the next number here, at the end of the loop, so while has something new to check.
    choice = int.Parse(Console.ReadLine());
}

Console.WriteLine($"That comes to £{total}");

string MenuLine(int number, string name, int price)
{
    return $"{number}. {name} - £{price}";
}
