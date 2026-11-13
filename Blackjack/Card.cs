using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    /// <summary>
    /// Represents a single playing card and contains the static helper methods
    /// for working with decks and hands (shuffling, drawing, scoring).
    /// </summary>
    internal class Card
    {
        /// <summary>
        /// The suit of the card: 0 = clubs, 1 = spades, 2 = hearts, 3 = diamonds.
        /// </summary>
        public int Suit;

        /// <summary>
        /// The face value of the card: 2-10 = number cards, 11 = Jack,
        /// 12 = Queen, 13 = King, 14 = Ace.
        /// </summary>
        public int Value;

        /// <summary>
        /// Creates a new card.
        /// </summary>
        /// <param name="suit">The suit (0 = clubs, 1 = spades, 2 = hearts, 3 = diamonds).</param>
        /// <param name="value">The face value (2-14, where 14 is the Ace).</param>
        public Card(int suit, int value)
        {
            Suit = suit;
            Value = value;
        }

        /// <summary>
        /// Converts the suit number into its display symbol.
        /// </summary>
        /// <returns>The suit symbol, or "Unknown" if the suit is out of range.</returns>
        public string GetSuitAsText()
        {
            switch (Suit)
            {
                case 0: return "♣";
                case 1: return "♠";
                case 2: return "♥";
                case 3: return "♦";
                default: return "Unknown";
            }
        }

        /// <summary>
        /// Converts the card value into its display text (e.g. 11 becomes "J").
        /// </summary>
        /// <returns>The value as text, or "Unknown" if the value is out of range.</returns>
        public string GetValueAsText()
        {
            if (Value >= 2 && Value <= 10)
                return Value.ToString();
            else if (Value == 11)
                return "J";
            else if (Value == 12)
                return "Q";
            else if (Value == 13)
                return "K";
            else if (Value == 14)
                return "A";
            else
                return "Unknown";
        }

        /// <summary>
        /// Returns the card as a printable string, e.g. "♥ A".
        /// </summary>
        /// <returns>The suit symbol followed by the value.</returns>
        public string ToText()
        {
            return $"{GetSuitAsText()} {GetValueAsText()}";
        }
    }
}
