// sum is declared before the loop so it keeps its running total from one trip to the next.
int sum = 0;

for (int i = 1; i <= 100; i++)
{
    sum = sum + i;
}

Console.WriteLine($"Sum: {sum}");
