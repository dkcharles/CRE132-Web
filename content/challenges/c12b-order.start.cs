List<string> names = new List<string> { "Crisps", "Chocolate", "Water", "Juice" };
List<int> prices = new List<int> { 1, 2, 1, 2 };

for (int i = 0; i < names.Count; i++)
{
    Console.WriteLine($"{i + 1}. {names[i]} - £{prices[i]}");
}

// TODO: read one item number from input.
// If it's a valid item (between 1 and names.Count), print:
//   You chose <name> - £<price>
// Otherwise print exactly:
//   Sorry, we don't have that
