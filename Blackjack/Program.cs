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
    - Display
    |
    |-> Extract the table output into ShowTable(...); it is duplicated in PlayHand and Settle
    -----------------------------------------------------------------------------------------------
    - Code cleanup
    |
    |-> Replace magic numbers in Payout (3:2, 1:1) with named constants in Settings
    |-> Make the Card suits an enum instead of the numbers 0-3
    |-> Make Hand.IsDone a property instead of a public field
    |-> Add ArgumentOutOfRangeException.ThrowIfNegative to ChipStack.Add
    |-> Use long for large amounts to avoid int overflow in AmountToStack
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

Console.ReadKey();