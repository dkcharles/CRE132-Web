// GameHost/GameState are internal so student code can never reach across sessions; the test
// assembly needs them to install state directly instead of driving a whole GameSession.
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests")]
