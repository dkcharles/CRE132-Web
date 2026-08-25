namespace CRE132.Game;

// Seeded by the host: a fresh seed for a browser Run, a fixed one under the checker and tests.
public static class Rand
{
    public static int Range(int min, int maxExclusive)
    {
        if (maxExclusive <= min)
            throw new ArgumentException($"Rand.Range({min}, {maxExclusive}): the second number must be bigger than the first.");
        return GameHost.State.Random.Next(min, maxExclusive);
    }

    public static double Range(double min, double max) => min + GameHost.State.Random.NextDouble() * (max - min);
}
