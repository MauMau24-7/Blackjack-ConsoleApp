namespace Blackjack
{
    /// <summary>
    /// Represents a hand and manages its cards and points.
    /// </summary>
    internal class Hand
    {
        private List<Card> CardsInHand = new List<Card>();
        public bool IsDone { get; set; }
        public ChipStack BetStack { get; set; } = new ChipStack();

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

            foreach (Card card in CardsInHand)
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

            // Over Settings.BlackjackScore? Downgrade Aces from 11 to 1 (subtract 10) one at a time
            // until the hand is safe or no Aces are left.
            while (points > Settings.BlackjackScore && aces > 0)
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
            CardsInHand.Add(card);
        }

        /// <summary>
        /// Removes a card from the hand.
        /// </summary>
        /// <param name="card">The card to remove.</param>
        public void RemoveCard(Card card)
        {
            CardsInHand.Remove(card);
        }

        /// <summary>
        /// Removes all cards from the hand.
        /// </summary>
        public void ClearCards()
        {
            CardsInHand.Clear();
        }

        /// <summary>
        /// Returns all cards in the hand as text.
        /// </summary>
        /// <returns>The cards as a formatted string.</returns>
        public string ToText()
        {
            return string.Join(", ", CardsInHand.Select(card => card.ToText()));
        }

        /// <summary>
        /// Returns the card at the specified index
        /// </summary>
        /// <param name="index">The zero-based index of the card in hand</param>
        /// <returns>The card at the specified index</returns>
        public Card GetCard(int index)
        {
            return CardsInHand[index];
        }

        /// <summary>
        /// Determines the outcome of the player's hand compared to the dealer's hand according to standard blackjack
        /// rules.
        /// </summary>
        /// <param name="dealer">The dealer's hand to compare against.</param>
        /// <param name="isSplitHand">true if the hand is a split hand; otherwise, false.</param>
        /// <returns>A HandResult value indicating win, loss, push, blackjack, or busted.</returns>
        public HandResult GetResultAgainst(Hand dealer, bool isSplitHand)
        {
            int points = GetPoints();
            int dealerPoints = dealer.GetPoints();
            bool isBlackjack = !isSplitHand && points == Settings.BlackjackScore && CardsInHand.Count == 2;
            bool dealerBlackjack = dealerPoints == Settings.BlackjackScore && dealer.CardsInHand.Count == 2;

            if (points > Settings.BlackjackScore)
            {
                return HandResult.Busted;
            }
            if (isBlackjack && dealerBlackjack)
            {
                return HandResult.Push;
            }
            if (isBlackjack && !dealerBlackjack)
            {
                return HandResult.Blackjack;
            }
            if (!isBlackjack && dealerBlackjack)
            {
                return HandResult.Loss;
            }
            if (dealerPoints > Settings.BlackjackScore)
            {
                return HandResult.Win;
            }
            if (points > dealerPoints)
            {
                return HandResult.Win;
            }
            if (points < dealerPoints)
            {
                return HandResult.Loss;
            }
            return HandResult.Push;
        }

        /// <summary>
        /// Count's the cards of the hand.
        /// </summary>
        /// <returns>The number of cards in hand.</returns>
        public int CardCount()
        {
            return CardsInHand.Count;
        }
    }
}
