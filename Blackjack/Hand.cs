using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    /// <summary>
    /// Represents a hand and manages its cards and points.
    /// </summary>
    internal class Hand
    {
        private List<Card> cardsInHand = new List<Card>();
        public bool isDone = false;

        /// <summary>
        /// Calculates the Blackjack score of a hand.
        /// Number cards count their value, J/Q/K count 10, and Aces count 11
        /// unless that would bust the hand, in which case they count 1.
        /// </summary>
        /// <returns>The best possible score of the hand.</returns>
        public int GetPoints()
        {
            int points = 0;
            int aces = 0;   // number of Aces still counted as 11

            foreach (Card card in cardsInHand)
            {
                if (card.Value >= 2 && card.Value <= 10)
                {
                    points += card.Value;
                }
                else if (card.Value >= 11 && card.Value <= 13)
                {
                    points += 10;
                }
                else if (card.Value == 14)
                {
                    points += 11;
                    aces++;
                }
            }

            // Over 21? Downgrade Aces from 11 to 1 (subtract 10) one at a time
            // until the hand is safe or no Aces are left.
            while (points > 21 && aces > 0)
            {
                points -= 10;
                aces--;
            }

            return points;
        }

        /// <summary>
        /// Adds a card to the hand.
        /// </summary>
        /// <param name="card">The card to add.</param>
        public void AddCard(Card card)
        {
            cardsInHand.Add(card);
        }

        /// <summary>
        /// Removes a card from the hand.
        /// </summary>
        /// <param name="card">The card to remove.</param>
        public void RemoveCard(Card card)
        {
            cardsInHand.Remove(card);
        }

        /// <summary>
        /// Removes all cards from the hand.
        /// </summary>
        public void ClearCards()
        {
            cardsInHand.Clear();
        }

        /// <summary>
        /// Returns all cards in the hand as text.
        /// </summary>
        /// <returns>The cards as a formatted string.</returns>
        public string ToText()
        {
            string cardsAsString = "";

            foreach (Card card in cardsInHand)
            {
                cardsAsString += card.ToText() + ", ";          // Read about String.Join() in the future
            }
            return cardsAsString;
        }

        /// <summary>
        /// Returns the card at the specified index
        /// </summary>
        /// <param name="index">The zero-based index of the card in hand</param>
        /// <returns>The card at the specified index</returns>
        public Card GetCard(int index)
        {
            return cardsInHand[index];
        }
    }
}
