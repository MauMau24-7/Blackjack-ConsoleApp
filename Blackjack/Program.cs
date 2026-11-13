using Blackjack;

// ============================================================================
// TOP-LEVEL STATEMENTS
// Order required by C#: using directives -> statements -> type declarations.
// Only one file per project may contain top-level statements.
// ============================================================================

// --- Game state -------------------------------------------------------------
List<Card> deck = new List<Card>();
Hand mainPlayerHand = new Hand();
Hand splitPlayerHand = new Hand();
Hand dealerHand = new Hand();

int mainPlayerPoints = 0;
int splitPlayerPoints = 0;
int dealerPoints = 0;
bool playAgain = true;      // controls the outer game loop
bool didSplit = false;


/*
||===========================================================||
||      ________            ______                           ||
||     |___  ___||  _____   ||    \\    _____    ,-,,        ||
||         | ||    ||  ||   ||     ||  ||  ||    '-''        ||
||         | ||    ||  ||   ||     ||  ||  ||    ,-,,        ||
||         |__|    |____|   |_____//   |____|    '-''        ||
||===========================================================||


    - Add chips for betting before hands
    |
    |-> Add function RemoveChips(dict)
    - Clean up the program.cs by using class files etc.
*/


// Needed so the suit symbols (♣ ♠ ♥ ♦) display correctly in the console.
Console.OutputEncoding = System.Text.Encoding.UTF8;

// --- TESTING ----------------------------------------------------------------
Player p = new Player("Anna", 1000);
ChipStack bet = p.Bet(250);

Console.WriteLine($"Bet:  {bet.GetTotalValue()}");
Console.WriteLine($"Left: {p.ChipStack.GetTotalValue()}");

bet = p.Bet(5000);

Console.WriteLine($"Bet:  {bet.GetTotalValue()}");
Console.WriteLine($"Left: {p.ChipStack.GetTotalValue()}");

Console.ReadKey();

// --- Startup option ---------------------------------------------------------
if (!Settings.DebugMode)
{
    string? answer;

    Console.Write("Do you want to enable Fast Mode? ([Y]es or [N]o): ");
    answer = Console.ReadLine()?.ToLower();
    if (answer == "y" || answer == "yes")
        Settings.FastMode = true;

    Console.Write("How many Decks do you want to use?: ");
    answer = Console.ReadLine()?.ToLower();
    if (byte.TryParse(answer, out byte amount))
    {
        if (amount == 0)
            amount = 1;
        Settings.AmountOfDecks = amount;
    }
}

// --- Main game loop (one iteration = one round) -----------------------------
while (playAgain)
{
    // Rebuild and reshuffle the deck when fewer than 18 cards are left.
    // This acts like the "cut card" in a casino shoe.
    if (deck.Count < 18)
    {
        Console.Clear();
        deck = CardDeck.CreateDeck();

        // Purely cosmetic shuffle animation.
        Console.Write("Shuffling the deck");
        Settings.Pause(500);
        Console.Write(".");
        Settings.Pause(500);
        Console.Write(".");
        Settings.Pause(500);
        Console.Write(".");
        Settings.Pause(1000);

        CardDeck.Shuffle(deck);
    }

    if (!Settings.ForceSplit)
    {
        // Initial deal: two cards each, alternating player/dealer like at a real table.
        for (int i = 0; i < 2; i++)
        {
            mainPlayerHand.AddCard(CardDeck.Draw(deck));
            dealerHand.AddCard(CardDeck.Draw(deck));
        }
    }
    else
    {
        mainPlayerHand.AddCard(new Card(0, 2));
        mainPlayerHand.AddCard(new Card(1, 2));
        dealerHand.AddCard(new Card(2, 2));
        dealerHand.AddCard(new Card(3, 2));
    }

    // --- Blackjack check ----------------------------------------------------
    // A Blackjack can only happen with the first two cards, so it is checked
    // right after the deal and before the player makes any decision.
    // Both hands have exactly two cards here, so "== 21" is enough.
    mainPlayerPoints = mainPlayerHand.GetPoints();
    dealerPoints = dealerHand.GetPoints();

    bool playerBlackjack = mainPlayerPoints == 21;
    bool dealerBlackjack = dealerPoints == 21;
    bool canSplit = mainPlayerHand.GetCard(0).Value == mainPlayerHand.GetCard(1).Value;
    splitPlayerHand.isDone = true;

    // If the dealer has Blackjack, the round is over immediately.
    // Setting standing = true skips the player's turn loop below.
    // (If only the player has Blackjack, the loop is skipped anyway,
    // because mainPlayerPoints is already 21.)
    if (dealerBlackjack)
        mainPlayerHand.isDone = true;

    // --- Player's turn ------------------------------------------------------
    // Runs until the player stands, reaches 21 or busts.
    while ((mainPlayerPoints < 21 && !mainPlayerHand.isDone) || (splitPlayerPoints < 21 && !splitPlayerHand.isDone))
    {
        Console.Clear();

        // Main hand first, split hand only when the main hand is done
        bool mainActive = mainPlayerPoints < 21 && !mainPlayerHand.isDone;
        Hand currentHand = mainActive ? mainPlayerHand : splitPlayerHand;
        int currentPoints = currentHand.GetPoints();
        int handIndex = mainActive ? 1 : 2;

        Hand dealerCovered = new Hand();
        dealerCovered.AddCard(dealerHand.GetCard(0));

        // Recalculate points from the current hands every iteration.
        mainPlayerPoints = mainPlayerHand.GetPoints();
        dealerPoints = dealerCovered.GetPoints();

        Console.WriteLine("Dealer's hand:");

        // Only the dealer's first card is visible; the second stays hidden ("?").
        Console.WriteLine(dealerHand.GetCard(0).ToText() + ", ?");

        Console.WriteLine($"Dealer Points: {dealerPoints}\n\n");

        if (!didSplit)
            Console.WriteLine("Player's hand:");
        else
        {
            Console.WriteLine($"Player hand {handIndex}");
        }

        Console.WriteLine(currentHand.ToText());
        Console.WriteLine();
        Console.WriteLine($"Player Points: {currentPoints}\n");

        // Only ask for input while the player can still act.
        if (currentPoints < 21)
        {
            Settings.Pause(1500);
            Console.Write("[H]it");
            if (canSplit && mainActive)
                Console.Write(", [Sp]lit");
            Console.WriteLine(" or [S]tand?");

            switch (Console.ReadLine()?.ToLower())
            {
                case "hit" or "h":
                    currentHand.AddCard(CardDeck.Draw(deck));

                    break;

                case "stand" or "s":
                    currentHand.isDone = true;
                    break;

                case "split" or "sp" when canSplit && mainActive:
                    splitPlayerHand.AddCard(mainPlayerHand.GetCard(0));
                    mainPlayerHand.RemoveCard(splitPlayerHand.GetCard(0));
                    didSplit = true;
                    splitPlayerHand.isDone = false;
                    canSplit = false;

                    mainPlayerHand.AddCard(CardDeck.Draw(deck));
                    splitPlayerHand.AddCard(CardDeck.Draw(deck));
                    break;

                case "deck" when Settings.DebugMode:                                                // tool to see the rest of the deck
                    for (int i = 0; i < deck.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {deck[i].ToText()}");
                    }
                    Console.ReadKey();
                    break;


                case "hit35" when Settings.DebugMode:                                               // tool to control if the shuffling works
                    // Checking deck.Count first prevents drawing from an empty deck.
                    for (int i = 0; i < 35 && deck.Count > 0; i++)
                    {
                        currentHand.AddCard(CardDeck.Draw(deck));
                    }
                    break;

                        default:
                    // Invalid input: show a short message, then the loop redraws the menu.
                    Console.Clear();
                    Console.WriteLine("That was not an option...");
                    Settings.Pause(1000);
                    Console.WriteLine("Please try again...");
                    Settings.Pause(1000);
                    break;
            }
        }

        // Points for the loop condition, recalculated after the action
        mainPlayerPoints = mainPlayerHand.GetPoints();
        splitPlayerPoints = splitPlayerHand.GetPoints();
    }

    Console.Clear();

    // --- Dealer's turn ------------------------------------------------------
    // Standard rule: the dealer draws until reaching 17 or more.
    // The dealer skips drawing if the player already busted (nothing left to win) or has a blackjack (21 with 2 cards).
    dealerPoints = dealerHand.GetPoints();

    while (dealerPoints < 17 && mainPlayerPoints < 22 && !playerBlackjack)
    {
        dealerHand.AddCard(CardDeck.Draw(deck));
        dealerPoints = dealerHand.GetPoints();
    }

    List<Hand> hands = new List<Hand>();
    hands.Add(mainPlayerHand);
    if (didSplit)
        hands.Add(splitPlayerHand);

    foreach (Hand hand in hands)
    {   int points = hand.GetPoints();
        // --- Evaluate the result ------------------------------------------------
        // The order of these checks matters: the first matching branch wins.
        // playerBlackjack and dealerBlackjack were already determined after the deal.
        if (points > 21 && dealerPoints <= 21)
        {
            ColorWordInText("Player BUSTED", ConsoleColor.Red);
        }
        else if (playerBlackjack && !dealerBlackjack && !didSplit)
        {
            // A natural 21 with exactly two cards.
            ColorWordInText("Player BLACKJACK", ConsoleColor.Green);
        }
        else if (dealerBlackjack && !playerBlackjack)
        {
            ColorWordInText("Dealer BLACKJACK", ConsoleColor.Red);
        }
        else if (dealerPoints > 21 && points < 22)
        {
            ColorWordInText("Player wins\nDealer BUSTED", ConsoleColor.Green);
        }
        else if (dealerPoints > points && dealerPoints < 22)
        {
            ColorWordInText("Dealer wins", ConsoleColor.Red);
        }
        else if (points > dealerPoints && points < 22)
        {
            ColorWordInText("Player wins", ConsoleColor.Green);
        }
        else if (dealerPoints == points)
        {
            ColorWordInText("Push", ConsoleColor.Yellow);
        }

        // --- Show the final hands (dealer's hidden card is now revealed) --------
        Console.WriteLine($"\nDealer: {dealerPoints}");
        Console.WriteLine($"Player: {points}\n");

        Console.Write("Dealer's hand: ");
        Console.WriteLine(dealerHand.ToText() + "\n");

        Console.Write("Player's hand: ");
        Console.WriteLine(hand.ToText());

        if (hands.Count > 1)
        {
            Console.Write("\nPress Enter to continue");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            Console.Clear();
        }
    }

    Console.WriteLine("\n\nDo you want to play again? ([Y]es or [N]o)");

    string? playAgainAnswer = Console.ReadLine()?.ToLower();
    if (playAgainAnswer == "n" || playAgainAnswer == "no")
    {
        playAgain = false;
    }
    else
    {
        // Reset everything that belongs to a single round.
        // The deck is intentionally NOT reset; it carries over between rounds.
        playAgain = true;
        dealerHand.ClearCards();
        mainPlayerHand.ClearCards();
        splitPlayerHand.ClearCards();
        mainPlayerPoints = 0;
        splitPlayerPoints = 0;
        dealerPoints = 0;
        mainPlayerHand.isDone = false;
        splitPlayerHand.isDone = true;
        didSplit = false;
    }
}

Console.WriteLine("Press a key to exit...");
Console.ReadKey();


// ============================================================================
// TYPE DECLARATIONS
// Must come after all top-level statements.
// ============================================================================

static void ColorWordInText(string coloredText, ConsoleColor color, string textBefore = "", string textAfter = "")
{
    ConsoleColor originalColor = Console.ForegroundColor;

    Console.Write(textBefore);
    Console.ForegroundColor = color;
    Console.Write(coloredText);
    Console.ForegroundColor = originalColor;
    Console.WriteLine(textAfter);
}
