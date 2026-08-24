int count = 5;
int total = 0;

for (int i = 1; i <= count; i++)
{
    total = total + i;

    if (i == count)
    {
        Console.WriteLine($"Total: {total}");
    }
}
