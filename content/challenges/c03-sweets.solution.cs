int sweets = 23;
int friends = 4;

// One division answers two questions: how many each friend gets, and how many will not go round.
int sweetsEach = sweets / friends;
int leftOver = sweets % friends;
Console.WriteLine($"Each friend gets {sweetsEach} sweets");
Console.WriteLine($"There are {leftOver} left over");
