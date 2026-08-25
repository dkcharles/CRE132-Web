List<string> names = new List<string> { "Crisps", "Chocolate", "Water", "Juice" };
List<int> prices = new List<int> { 1, 2, 1, 2 };

for (int i = 0; i < names.Count; i++)
{
    // i + 1 turns the index, which counts from 0, into the menu number, which counts from 1.
    Console.WriteLine(MenuLine(i + 1, names[i], prices[i]));
}

string MenuLine(int number, string name, int price)
{
    return $"{number}. {name} - £{price}";
}
