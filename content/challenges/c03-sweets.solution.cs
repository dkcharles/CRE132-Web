int sweets = 23;
int friends = 4;

// / shares the sweets out evenly; % is what would not divide up.
int sweetsEach = sweets / friends;
int leftOver = sweets % friends;
Console.WriteLine($"Each friend gets {sweetsEach} sweets");
Console.WriteLine($"There are {leftOver} left over");
