using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    internal class ChipStack
    {
        Dictionary<ChipType, int> counts = new Dictionary<ChipType, int>();

        public ChipStack()
        {
            foreach (ChipType type in Enum.GetValues<ChipType>())
            {
                counts[type] = 0;
            }
        }
        private class Share
        {
            public ChipType Type;
            public byte Percent;
        }

        static readonly List<Share> Shares = new List<Share>
    {
        new Share { Type = ChipType.Purple,   Percent = 40 },
        new Share { Type = ChipType.Black,    Percent = 35 },
        new Share { Type = ChipType.Green,    Percent = 15 },
        new Share { Type = ChipType.Blue,     Percent = 5 },
        new Share { Type = ChipType.Red,      Percent = 4 },
    };

        public static ChipStack AmountToStack(int amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(amount, "Amount can't be negative.");

            ChipStack stack = new ChipStack();
            int overflow = 0;
            int distributedAmount = 0;

            foreach (Share share in Shares)
            {
                int portion = amount * share.Percent / 100;
                int pot = portion + overflow;
                int chipAmount = pot / (int)share.Type;
                overflow = pot % (int)share.Type;
                distributedAmount += chipAmount * (int)share.Type;

                stack.counts[share.Type] = chipAmount;
            }
            stack.counts[ChipType.White] = amount - distributedAmount;

            return stack;
        }

        public int GetCount(ChipType type)
        {
            return counts[type];
        }

        public int GetTotalValue()
        {
            int total = 0;

            foreach (var entry in counts)
            {
                total += entry.Value * (int)entry.Key;
            }

            return total;
        }

        public void Add(ChipType type, int amount)
        {
            counts[type] += amount;
        }

        public void Remove(ChipType type, int amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(amount, "Amount can't be negative.");
            if (amount > counts[type]) throw new InvalidOperationException("Not enough chips of that type.");
            counts[type] -= amount;
        }

        public bool HasEnoughChips(int amount)
        {
            return amount <= GetTotalValue();
        }

        public (ChipStack taken, int remainder) RemoveUpTo(int amount)
        {
            ChipStack taken = new ChipStack();
            int remainder = amount;

            foreach (ChipType type in Enum.GetValues<ChipType>().Reverse())
            {
                int fitInRemainder = remainder / (int)type;
                int count = counts[type];
                int takeAmount = Math.Min(fitInRemainder, count);

                Remove(type, takeAmount);
                taken.Add(type, takeAmount);
                remainder -= takeAmount * (int)type;
            }

            if (remainder > 0)
            {
                foreach (ChipType type in Enum.GetValues<ChipType>())
                {
                    if (counts[type] > 0)
                    {
                        Remove(type, 1);

                        ChipStack betStack = AmountToStack(remainder);
                        foreach (ChipType t in Enum.GetValues<ChipType>())
                        {
                            taken.Add(t, betStack.GetCount(t));
                        }

                        int change = (int)type - remainder;
                        ChipStack changeStack = AmountToStack(change);
                        foreach (ChipType t in Enum.GetValues<ChipType>())
                        {
                            Add(t, changeStack.GetCount(t));
                        }

                        remainder = 0;
                        break;
                    }
                }
            }

            return (taken, remainder);
        }
    }
}
