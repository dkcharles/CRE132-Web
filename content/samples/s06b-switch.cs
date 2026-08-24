Console.WriteLine("Menu: 1) Tea 2) Coffee 3) Juice");
string choiceText = Console.ReadLine();
int choice = int.Parse(choiceText);

switch (choice)
{
    case 1:
        Console.WriteLine("One tea coming up.");
        break;
    case 2:
        Console.WriteLine("One coffee coming up.");
        break;
    case 3:
        Console.WriteLine("One juice coming up.");
        break;
    default:
        Console.WriteLine("Sorry, we don't have that.");
        break;
}
