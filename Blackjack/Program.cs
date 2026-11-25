using Blackjack;

/*
||===========================================================||
||      ________            ______                           ||
||     |___  ___||  _____   ||    \\    _____    ,-,,        ||
||         | ||    ||  ||   ||     ||  ||  ||    '-''        ||
||         | ||    ||  ||   ||     ||  ||  ||    ,-,,        ||
||         |__|    |____|   |_____//   |____|    '-''        ||
||===========================================================||

    -----------------------------------------------------------------------------------------------
    - Game rules
    |
    |-> Add Double Down (double the bet, draw exactly one card, hand is done)
    |-> Split Aces: each Ace hand only gets one card
    |-> Allow splitting cards with the same point value (e.g. King + Jack)
    |-> Dealer: skip drawing when the player has a Blackjack (display only)
    -----------------------------------------------------------------------------------------------
    - Chips
    |
    |-> Show the chips the player currently has (per type) instead of the total value
    |-> Let the player choose which chips to bet (e.g. "3 Black, 2 Green")
    |-> Add a chip value legend for people who don't know the chip colors
    -----------------------------------------------------------------------------------------------
    - Settings
    |
    -----------------------------------------------------------------------------------------------
    - Display
    |
    -----------------------------------------------------------------------------------------------
    - Code cleanup
    |
    -----------------------------------------------------------------------------------------------
    - Testing
    |
    |-> Move the manual tests (GetResultAgainst, Payout, RemoveUpTo) into a unit test project
    |-> Test edge cases: 0 chips, amount 1, only large chips, split with too few chips
    -----------------------------------------------------------------------------------------------
    - Optional features
    |
    |-> Insurance and Surrender
    |-> Let the player choose the starting chips
    |-> Save and load the chip balance between sessions
    -----------------------------------------------------------------------------------------------
    - Project
    |
    |-> Commit the current state to Git
    |-> Write a short README (rules, how to run, features)
    -----------------------------------------------------------------------------------------------

*/


// Needed so the suit symbols (♣ ♠ ♥ ♦) display correctly in the console.
Console.OutputEncoding = System.Text.Encoding.UTF8;

Settings.AskStartupOptions();
Game game = new Game();
game.Run();

Console.Clear();
Console.WriteLine("Thank you for playing!\n\n");
