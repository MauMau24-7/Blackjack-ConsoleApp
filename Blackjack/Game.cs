namespace Blackjack
{
    /// <summary>
    /// Manages the flow of a Blackjack game, including player and dealer actions, betting, dealing, and round
    /// progression.
    /// </summary>
    internal class Game
    {
        private Player Player;
        private List<Card> Deck;
        private Hand DealerHand;

        public Game()
        {
            Player = new Player(Settings.PlayerName);
            Deck = new List<Card>();
            DealerHand = new Hand();
        }

        /// <summary>
        /// Executes the main game loop, handling deck preparation, betting, dealing, player and dealer turns,
        /// settlement, and round resets.
        /// </summary>
        public void Run()
        {
            bool playAgain = true;

            while (playAgain)
            {
                PrepareDeck();
                if (Settings.BetsOn) PlaceBet();
                Deal();
                PlayerTurn();
                DealerTurn();
                Settle();
                playAgain = AskPlayAgain();
                if (playAgain) ResetRound();
            }
        }

        /// <summary>
        /// Prepares the deck by creating a new one and shuffling it if the count is below the reshuffle threshold.
        /// </summary>
        private void PrepareDeck()
        {
            if (Deck.Count < Settings.ReshuffleThreshold)
            {
                Console.Clear();
                Deck = CardDeck.CreateDeck();

                // Purely cosmetic shuffle animation.
                Console.Write("Shuffling the deck");
                Settings.Pause(500);
                Console.Write(".");
                Settings.Pause(500);
                Console.Write(".");
                Settings.Pause(500);
                Console.Write(".");
                Settings.Pause(1000);
                CardDeck.Shuffle(Deck);
            }
        }

        /// <summary>
        /// Prompts the player to enter a valid bet amount and updates the player's bet stack based on the input.
        /// </summary>
        private void PlaceBet()
        {
            bool valid = false;
            int bet = 0;

            while (!valid)
            {
                Console.Clear();
                Console.WriteLine($"Your current chips are: {Player.Chips.GetTotalValue()}");
                Console.Write("Your bet:\t");
                if (int.TryParse(Console.ReadLine(), out bet) && bet > 0 && Player.Chips.HasEnoughChips(bet))
                {
                    valid = true;
                }
                else
                {
                    Console.WriteLine("\n\nThat wasn't a valid bet. Please try again");
                    Settings.Pause(2000);
                }
            }

            Player.Hands[0].BetStack = Player.Bet(bet);
        }

        /// <summary>
        /// Deals two cards each to the player and dealer, either randomly from the deck or using predefined cards based
        /// on settings.
        /// </summary>
        private void Deal()
        {
            if (!Settings.ForceSplit)
            {
                // Initial deal: two cards each, alternating player/dealer like at a real table.
                for (int i = 0; i < 2; i++)
                {
                    Player.Hands[0].AddCard(CardDeck.Draw(Deck));
                    DealerHand.AddCard(CardDeck.Draw(Deck));
                }
            }
            else
            {
                Player.Hands[0].AddCard(new Card(Suit.Clubs, 2));
                Player.Hands[0].AddCard(new Card(Suit.Spades, 2));
                DealerHand.AddCard(new Card(Suit.Hearts, 2));
                DealerHand.AddCard(new Card(Suit.Diamonds, 2));
            }
        }

        /// <summary>
        /// Executes the player's turn by playing each hand unless the dealer has a blackjack.
        /// </summary>
        private void PlayerTurn()
        {
            if (DealerHand.GetPoints() == Settings.BlackjackScore) return;
            for (int i = 0; i < Player.Hands.Count; i++)
            {
                PlayHand(Player.Hands[i]);
            }
        }

        /// <summary>
        /// Lets a player choose actions for a hand until it reaches the blackjack score or stands.
        /// </summary>
        /// <remarks>Supports hitting, standing, and splitting eligible hands. Debug modes also allow
        /// inspecting the deck and drawing 35 cards.</remarks>
        /// <param name="hand">The hand to play.</param>
        private void PlayHand(Hand hand)
        {
            while (hand.GetPoints() < Settings.BlackjackScore && !hand.IsDone)
            {
                Console.Clear();
                ShowTable(hand, false);

                Settings.Pause(1500);
                Console.Write("[H]it");
                if (CanSplit(hand)) Console.Write(", [Sp]lit");
                Console.WriteLine(" or [S]tand?");

                switch (Console.ReadLine()?.ToLower())
                {
                    case "hit" or "h":
                        hand.AddCard(CardDeck.Draw(Deck));
                        break;
                    case "stand" or "s":
                        hand.IsDone = true;
                        break;
                    case "split" or "sp" when CanSplit(hand):
                        Hand splitHand = new Hand();
                        Card moved = hand.GetCard(1);

                        splitHand.AddCard(moved);
                        hand.RemoveCard(moved);

                        if (Settings.BetsOn) splitHand.BetStack = Player.Bet(hand.BetStack.GetTotalValue());
                        Player.Hands.Add(splitHand);

                        hand.AddCard(CardDeck.Draw(Deck));
                        splitHand.AddCard(CardDeck.Draw(Deck));
                        break;
                    case "deck" when Settings.DebugMode:                                                // tool to see the rest of the deck
                        for (int i = 0; i < Deck.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {Deck[i].ToText()}");
                        }
                        Console.ReadKey();
                        break;
                    case "hit35" when Settings.DebugMode:                                               // tool to control if the shuffling works
                                                                                                        // Checking deck.Count first prevents drawing from an empty deck.
                        for (int i = 0; i < 35 && Deck.Count > 0; i++)
                        {
                            hand.AddCard(CardDeck.Draw(Deck));
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
            Console.Clear();
        }

        /// <summary>
        /// Executes the dealer's turn by drawing cards until the dealer's hand reaches the stand threshold or no
        /// players remain within the allowed score.
        /// </summary>
        private void DealerTurn()
        {
            bool anyAlive = Player.Hands.Any(h => h.GetPoints() <= Settings.BlackjackScore);

            while (anyAlive && DealerHand.GetPoints() < Settings.DealerStandsOn)
            {
                DealerHand.AddCard(CardDeck.Draw(Deck));
            }
        }

        /// <summary>
        /// Settles each player hand by determining its result, collecting payout, and displaying the result.
        /// </summary>
        private void Settle()
        {
            Console.Clear();
            foreach (Hand hand in Player.Hands)
            {
                HandResult result = hand.GetResultAgainst(DealerHand, Player.Hands.Count > 1);
                if (Settings.BetsOn) Player.CollectPayout(hand.BetStack, result);

                ShowResult(result, DealerHand.GetPoints() > Settings.BlackjackScore);
                Console.WriteLine();
                ShowTable(hand, true);
            }
        }

        /// <summary>
        /// Resets the round by clearing the dealer's cards and initializing a new hand for the player.
        /// </summary>
        private void ResetRound()
        {
            DealerHand.ClearCards();
            Player.Hands.Clear();
            Player.Hands.Add(new Hand());
        }

        /// <summary>
        /// Prompts the player to play again if they have chips remaining.
        /// </summary>
        /// <returns>true if the player chooses to play again; otherwise, false.</returns>
        private bool AskPlayAgain()
        {
            if (Settings.BetsOn && Player.Chips.GetTotalValue() <= 0)
            {
                Console.WriteLine("You are out of chips");
                Settings.Pause(5000);
                return false;
            }
            Console.WriteLine("Do you want to play again? ([Y]es or [N]o)");
            string? playAgainAnswer = Console.ReadLine()?.ToLower();
            return !(playAgainAnswer == "n" || playAgainAnswer == "no");
        }

        /// <summary>
        /// Displays the outcome of a hand, indicating whether the player has won, lost, pushed, or achieved blackjack.
        /// </summary>
        /// <param name="result">The result of the hand, represented by the HandResult enumeration.</param>
        /// <param name="dealerBusted">true if the dealer has exceeded the maximum hand value; otherwise, false.</param>
        private void ShowResult(HandResult result, bool dealerBusted)
        {
            switch (result)
            {
                case HandResult.Blackjack:
                    ConsoleHelper.ColorWordInText($"{Player.Name} BLACKJACK", ConsoleColor.Green);
                    break;
                case HandResult.Win:
                    ConsoleHelper.ColorWordInText($"{Player.Name} wins", ConsoleColor.DarkGreen);
                    if (dealerBusted) ConsoleHelper.ColorWordInText("Dealer BUSTED", ConsoleColor.DarkRed);
                    break;
                case HandResult.Push:
                    ConsoleHelper.ColorWordInText("Push", ConsoleColor.Yellow);
                    break;
                case HandResult.Loss:
                    ConsoleHelper.ColorWordInText("Dealer wins", ConsoleColor.Red);
                    break;
                case HandResult.Busted:
                    ConsoleHelper.ColorWordInText($"{Player.Name} BUSTED", ConsoleColor.DarkRed);
                    break;
            }
        }

        /// <summary>
        /// Determines whether the specified hand is eligible to be split based on game rules and available chips.
        /// </summary>
        /// <param name="hand">The hand to evaluate for splitting eligibility.</param>
        /// <returns>true if the hand can be split; otherwise, false.</returns>
        private bool CanSplit(Hand hand)
        {
            return Player.Hands.Count == 1 && hand.CardCount() == 2 &&
                hand.GetCard(0).Value == hand.GetCard(1).Value &&
                (!Settings.BetsOn || Player.Chips.HasEnoughChips(hand.BetStack.GetTotalValue()));
        }

        /// <summary>
        /// Displays the dealer and player hands with their point totals.
        /// </summary>
        /// <param name="hand">The player hand to display.</param>
        /// <param name="revealDealer">Whether to display the dealer's complete hand and point total.</param>
        private void ShowTable(Hand hand, bool revealDealer)
        {
            Console.WriteLine("Dealer's hand:");
            if (revealDealer)
            {
                Console.WriteLine(DealerHand.ToText());
                Console.WriteLine($"Dealer Points: {DealerHand.GetPoints()}\n\n");
            }
            else
            {
                Console.WriteLine(DealerHand.GetCard(0).ToText() + ", ?");
                Hand covered = new Hand();
                covered.AddCard(DealerHand.GetCard(0));
                Console.WriteLine($"Dealer Points: {covered.GetPoints()}\n\n");
            }

            if (Player.Hands.Count > 1)
                Console.WriteLine($"{Player.Name} hand {Player.Hands.IndexOf(hand) + 1}");
            else
                Console.WriteLine($"{Player.Name}'s hand:");

            Console.WriteLine(hand.ToText());
            Console.WriteLine();
            Console.WriteLine($"{Player.Name} Points: {hand.GetPoints()}\n");
        }
    }

}
