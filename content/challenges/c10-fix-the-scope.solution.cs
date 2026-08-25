int count = 5;

// total is declared before the loop, so all five trips add to the same running total.
int total = 0;

for (int i = 1; i <= count; i++)
{
    total = total + i;

    if (i == count)
    {
        Console.WriteLine($"Total: {total}");
    }
}
