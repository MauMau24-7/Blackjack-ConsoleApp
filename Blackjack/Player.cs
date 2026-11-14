using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
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

        public ChipStack Bet(int amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(amount);
            if (!Chips.HasEnoughChips(amount)) throw new InvalidOperationException("Not enough chips.");

            var (taken, _) = Chips.RemoveUpTo(amount);

            return taken;
        }

        public void CollectPayout(ChipStack bet, HandResult result)
        {
            int value = bet.GetTotalValue();
            int payout = Payout.Calculate(result, value);
            Chips.AddStack(ChipStack.AmountToStack(payout));
        }
    }
}
