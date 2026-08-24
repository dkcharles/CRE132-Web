List<string> names = new List<string> { "Crisps", "Chocolate", "Water", "Juice" };
List<int> prices = new List<int> { 1, 2, 1, 2 };

for (int i = 0; i < names.Count; i++)
{
    Console.WriteLine(MenuLine(i + 1, names[i], prices[i]));
}

string MenuLine(int number, string name, int price)
{
    return $"{number}. {name} - £{price}";
}
