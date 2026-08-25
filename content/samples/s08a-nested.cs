for (int row = 1; row <= 4; row++)
{
    // Build the whole row in a string first, then print it once the inner loop has finished.
    string line = "";
    for (int col = 1; col <= 6; col++)
    {
        line = line + "*";
    }
    Console.WriteLine(line);
}
