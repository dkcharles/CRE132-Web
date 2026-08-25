int height = 5;

for (int row = 1; row <= height; row++)
{
    string line = "";
    // col <= row is what makes each row one character longer than the row above it.
    for (int col = 1; col <= row; col++)
    {
        line = line + "#";
    }
    Console.WriteLine(line);
}
