int n = 7;
Console.WriteLine("n % 2 = " + (n % 2));

// 60 is needed twice, so it gets a name: / gives the whole minutes, % the seconds left over.
int secondsPerMinute = 60;
int totalSeconds = 125;
int minutes = totalSeconds / secondsPerMinute;
int seconds = totalSeconds % secondsPerMinute;
Console.WriteLine(minutes + " minutes " + seconds + " seconds");
