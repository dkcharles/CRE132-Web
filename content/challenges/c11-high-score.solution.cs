List<int> scores = new List<int> { 42, 88, 17, 65, 90, 23 };

foreach (int score in scores)
{
    Console.WriteLine(score);
}

int highest = scores[0];

foreach (int score in scores)
{
    if (score > highest)
    {
        highest = score;
    }
}

Console.WriteLine($"Highest: {highest}");
