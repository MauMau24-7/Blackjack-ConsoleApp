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
        public Hand Hand;
        public ChipStack ChipStack;

        public Player(string name, int startAmount = Settings.DefaultStartAmount)
        {
            Name = name;
            Hand = new Hand();
            ChipStack = ChipStack.AmountToStack(startAmount);
        }

        public ChipStack Bet(int amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(amount, "Amount can't be negative.");
            if (!ChipStack.HasEnoughChips(amount)) throw new InvalidOperationException("Not enough chips.");

            var (taken, _) = ChipStack.RemoveUpTo(amount);

            return taken;
        }
    }
}
