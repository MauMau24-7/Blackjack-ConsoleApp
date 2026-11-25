namespace Blackjack
{
    /// <summary>
    /// Provides static methods for shuffling, drawing, and creating standard playing card decks.
    /// </summary>
    internal class CardDeck
    {
        /// <summary>
        /// Single shared random number generator for the whole class.
        /// Creating a new <see cref="Random"/> per call could produce identical
        /// sequences on older .NET versions, so one instance is reused.
        /// </summary>
        private static readonly Random _random = new Random();

        /// <summary>
        /// Shuffles the deck in place using the Fisher-Yates algorithm
        /// (Durstenfeld variant). Every ordering is equally likely.
        /// </summary>
        /// <param name="deck">The list of cards to shuffle.</param>
        public static void Shuffle(List<Card> deck)
        {
            // Walk backwards; everything after index i is already "finished".
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = _random.Next(0, i + 1);   // random index from 0 to i (inclusive)
                Card tempCard = deck[i];
                deck[i] = deck[j];
                deck[j] = tempCard;
            }
        }

        /// <summary>
        /// Removes the top card (the last element) from the deck and returns it.
        /// </summary>
        /// <param name="deck">The deck to draw from. It must not be empty.</param>
        /// <returns>The drawn card.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the deck is empty.
        /// </exception>
        public static Card Draw(List<Card> deck)
        {
            // Removing from the end of a List is O(1); removing from the front would be O(n).
            Card card = deck[deck.Count - 1];
            deck.RemoveAt(deck.Count - 1);
            return card;
        }

        /// <summary>
        /// Creates a list of standard playing cards for the number of decks specified in the settings.
        /// </summary>
        /// <returns>A list containing all cards from the generated deck or decks.</returns>
        public static List<Card> CreateDeck()
        {
            List<Card> deck = new List<Card>();

            for (int k = 0; k < Settings.AmountOfDecks; k++)
            {
                foreach (Suit suit in Enum.GetValues<Suit>())
                {
                    for (int value = 2; value <= 14; value++)
                    {
                        deck.Add(new Card(suit, value));
                    }
                }
            }
            return deck;
        }
    }
}
