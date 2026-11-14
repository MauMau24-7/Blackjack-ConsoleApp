using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
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

        public void Run()
        {
            bool playAgain = true;

            while (playAgain)
            {
                PrepareDeck();
                PlaceBet();
                Deal();
                PlayerTurn();
                DealerTurn();
                Settle();
                playAgain = AskPlayAgain();
                if (playAgain) ResetRound();
            }
        }

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
                Player.Hands[0].AddCard(new Card(0, 2));
                Player.Hands[0].AddCard(new Card(1, 2));
                DealerHand.AddCard(new Card(2, 2));
                DealerHand.AddCard(new Card(3, 2));
            }
        }

        private void PlayerTurn()
        {
            if (DealerHand.GetPoints() == 21) return;
            for (int i = 0; i < Player.Hands.Count; i++)
            {
                PlayHand(Player.Hands[i]);
            }
        }

        private void PlayHand(Hand hand)
        {
            while (hand.GetPoints() < Settings.BlackjackScore && !hand.IsDone)
            {
                Console.Clear();

                Hand dealerCovered = new Hand();
                dealerCovered.AddCard(DealerHand.GetCard(0));

                int dealerPoints = dealerCovered.GetPoints();

                Console.WriteLine("Dealer's hand:");

                // Only the dealer's first card is visible; the second stays hidden ("?").
                Console.WriteLine(DealerHand.GetCard(0).ToText() + ", ?");

                Console.WriteLine($"Dealer Points: {dealerPoints}\n\n");

                if (Player.Hands.Count > 1)
                    Console.WriteLine($"{Player.Name} hand {Player.Hands.IndexOf(hand) + 1}");
                else
                {
                    Console.WriteLine($"{Player.Name}'s hand:");
                }

                Console.WriteLine(hand.ToText());
                Console.WriteLine();
                Console.WriteLine($"{Player.Name} Points: {hand.GetPoints()}\n");

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

                        splitHand.BetStack = Player.Bet(hand.BetStack.GetTotalValue());
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

        private void DealerTurn()
        {
            bool anyAlive = Player.Hands.Any(h => h.GetPoints() <= Settings.BlackjackScore);

            while (anyAlive && DealerHand.GetPoints() < Settings.DealerStandsOn)
            {
                DealerHand.AddCard(CardDeck.Draw(Deck));
            }
        }

        private void Settle()
        {
            Console.Clear();
            foreach (Hand hand in Player.Hands)
            {
                int points = hand.GetPoints();
                HandResult result = hand.GetResultAgainst(DealerHand, Player.Hands.Count > 1);
                Player.CollectPayout(hand.BetStack, result);

                ShowResult(result, DealerHand.GetPoints() > Settings.BlackjackScore);

                Console.WriteLine($"\nDealer: {DealerHand.GetPoints()}");
                Console.WriteLine($"{Player.Name}: {points}\n");

                Console.Write("Dealer's hand: ");
                Console.WriteLine(DealerHand.ToText() + "\n");

                if (Player.Hands.Count > 1)
                    Console.WriteLine($"{Player.Name} hand {Player.Hands.IndexOf(hand) + 1}");
                else
                {
                    Console.Write($"{Player.Name}'s hand:");
                }
                Console.WriteLine($"{hand.ToText()}\n\n");
            }
        }

        private void ResetRound()
        {
            DealerHand.ClearCards();
            Player.Hands.Clear();
            Player.Hands.Add(new Hand());
        }

        private bool AskPlayAgain()
        {
            if (Player.Chips.GetTotalValue() <= 0) 
            { 
                Console.WriteLine("You are out of chips");
                return false;
            }
            Console.WriteLine("Do you want to play again? ([Y]es or [N]o)");
            string? playAgainAnswer = Console.ReadLine()?.ToLower();
            return !(playAgainAnswer == "n" || playAgainAnswer == "no");
        }
        
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

        private bool CanSplit(Hand hand)
        {
            return Player.Hands.Count == 1 && hand.CardCount() == 2 &&
                hand.GetCard(0).Value == hand.GetCard(1).Value &&
                Player.Chips.HasEnoughChips(hand.BetStack.GetTotalValue());
        }
    }

}
