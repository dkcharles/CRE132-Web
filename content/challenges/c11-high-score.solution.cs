List<int> scores = new List<int> { 42, 88, 17, 65, 90, 23 };

foreach (int score in scores)
{
    Console.WriteLine(score);
}

// Start with the first score, then replace it whenever a bigger one turns up.
int highest = scores[0];

foreach (int score in scores)
{
    if (score > highest)
    {
        highest = score;
    }
}

Console.WriteLine($"Highest: {highest}");
