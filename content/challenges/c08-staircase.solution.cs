string heightText = Console.ReadLine();
int height = int.Parse(heightText);

for (int row = 1; row <= height; row++)
{
    string line = "";
    for (int col = 1; col <= row; col++)
    {
        line = line + "#";
    }
    Console.WriteLine(line);
}
