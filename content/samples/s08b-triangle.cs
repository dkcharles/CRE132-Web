int height = 5;

for (int row = 1; row <= height; row++)
{
    string line = "";
    for (int col = 1; col <= row; col++)
    {
        line = line + "#";
    }
    Console.WriteLine(line);
}
