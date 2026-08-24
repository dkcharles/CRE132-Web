List<string> names = new List<string> { "Crisps", "Chocolate", "Water", "Juice" };
List<int> prices = new List<int> { 1, 2, 1, 2 };

for (int i = 0; i < names.Count; i++)
{
    Console.WriteLine($"{i + 1}. {names[i]} - £{prices[i]}");
}

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

// TODO: turn this into a till. Instead of reading just one item number, keep
// reading item numbers in a loop until the shopper enters 0.
// Add a total (an accumulator), starting at 0 and declared before the loop;
// every time a valid item is chosen, add its price to the total.
// When 0 is entered, stop reading and print exactly:
//   That comes to £<total>
