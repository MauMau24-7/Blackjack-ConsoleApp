namespace Blackjack
{
    /// <summary>
    /// Provides methods for calculating payouts based on hand results and bets in a blackjack game.
    /// </summary>
    internal static class Payout
    {
        /// <summary>
        /// Calculates the payout amount based on the hand result and bet.
        /// </summary>
        /// <param name="result">The outcome of the hand.</param>
        /// <param name="bet">The amount wagered by the player.</param>
        /// <returns>The payout corresponding to the hand result.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the result is not a valid HandResult value.</exception>
        public static int Calculate(HandResult result, int bet)
        {
            return result switch
            {
                HandResult.Blackjack => bet + (bet * Settings.BlackjackPayout),
                HandResult.Win => bet * Settings.NormalPayout,
                HandResult.Push => bet,
                HandResult.Loss or HandResult.Busted => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(result))
            };
        }
    }
}
