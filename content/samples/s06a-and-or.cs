int age = 20;
bool isStudent = true;

if (age < 18 && isStudent)
{
    Console.WriteLine("Ticket: £5");
}
else if (age >= 65 || isStudent)
{
    Console.WriteLine("Ticket: £7");
}
else
{
    Console.WriteLine("Ticket: £10");
}
