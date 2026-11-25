namespace Blackjack
{
    /// <summary>
    /// Represents a player in the game with a name, a collection of hands, and a stack of chips.
    /// </summary>
    internal class Player
    {
        public string Name;
        public List<Hand> Hands;
        public ChipStack Chips;

        public Player(string name, int startAmount = Settings.DefaultStartAmount)
        {
            Name = name;
            Hands = new List<Hand>();
            Hands.Add(new Hand());
            Chips = ChipStack.AmountToStack(startAmount);
        }

        /// <summary>
        /// Removes the specified number of chips from the stack for a bet.
        /// </summary>
        /// <param name="amount">The number of chips to bet.</param>
        /// <returns>A ChipStack containing the chips removed from the stack.</returns>
        /// <exception cref="InvalidOperationException">Thrown when there are not enough chips to cover the bet.</exception>
        public ChipStack Bet(int amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(amount);
            if (!Chips.HasEnoughChips(amount)) throw new InvalidOperationException("Not enough chips.");

            var (taken, _) = Chips.RemoveUpTo(amount);

            return taken;
        }

        /// <summary>
        /// Calculates and adds the payout to the player's chips based on the bet and hand result.
        /// </summary>
        /// <param name="bet">The stack of chips representing the player's bet.</param>
        /// <param name="result">The outcome of the hand used to determine the payout.</param>
        public void CollectPayout(ChipStack bet, HandResult result)
        {
            int value = bet.GetTotalValue();
            int payout = Payout.Calculate(result, value);
            Chips.AddStack(ChipStack.AmountToStack(payout));
        }
    }
}
