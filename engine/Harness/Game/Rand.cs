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

    // float, not double: student code declares positions and speeds as float (Unity's type),
    // and a float variable must be able to hold this without a cast.
    public static float Range(float min, float max)
    {
        if (max <= min)
            throw new ArgumentException($"Rand.Range({min}, {max}): the second number must be bigger than the first.");
        return min + (float)GameHost.State.Random.NextDouble() * (max - min);
    }

    // A student who writes Rand.Range(1.5, 2.5) without the f suffix lands here rather than on a
    // compile error, and still gets a float back. One NextDouble() either way, so the seeded
    // sequence is the same whichever overload is called.
    public static float Range(double min, double max) => Range((float)min, (float)max);
}
