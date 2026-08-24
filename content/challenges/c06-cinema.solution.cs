string ageText = Console.ReadLine();
int age = int.Parse(ageText);
string day = Console.ReadLine();

if (age < 12)
{
    Console.WriteLine("Child ticket: £5");
}
else if (day == "Tuesday")
{
    Console.WriteLine("Tuesday special: £6");
}
else
{
    Console.WriteLine("Standard ticket: £9");
}
