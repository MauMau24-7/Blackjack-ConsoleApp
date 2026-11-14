using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    /// <summary>
    /// Represents a stack of chips categorized by type, providing methods to manage, distribute, and manipulate chip
    /// counts and values.
    /// </summary>
    internal class ChipStack
    {
        /// <summary>
        /// Stores the number of chips for each chip type.
        /// </summary>
        Dictionary<ChipType, int> counts = new Dictionary<ChipType, int>();

        public ChipStack()
        {
            foreach (ChipType type in Enum.GetValues<ChipType>())
            {
                counts[type] = 0;
            }
        }
        /// <summary>
        /// Represents a share of a specific chip type with an associated percentage.
        /// </summary>
        private class Share
        {
            public ChipType Type;
            public byte Percent;
        }

        /// <summary>
        /// Represents the default share distribution percentages for each chip type.
        /// </summary>
        static readonly List<Share> Shares = new List<Share>
        {
            new Share { Type = ChipType.Purple,   Percent = 40 },
            new Share { Type = ChipType.Black,    Percent = 35 },
            new Share { Type = ChipType.Green,    Percent = 15 },
            new Share { Type = ChipType.Blue,     Percent = 5 },
            new Share { Type = ChipType.Red,      Percent = 4 },
        };

        /// <summary>
        /// Converts a specified amount into a stack of chips distributed by predefined shares.
        /// </summary>
        /// <param name="amount">The total amount to convert into chips. Must be non-negative.</param>
        /// <returns>A Chips representing the distributed chips.</returns>
        public static ChipStack AmountToStack(int amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(amount);

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

        /// <summary>
        /// Gets the count of chips for the specified chip type.
        /// </summary>
        /// <param name="type">The chip type whose count to retrieve.</param>
        /// <returns>The number of chips of the specified type.</returns>
        public int GetCount(ChipType type)
        {
            return counts[type];
        }

        /// <summary>
        /// Gets the total value of a Chips.
        /// </summary>
        /// <returns>The value of all chips.</returns>
        public int GetTotalValue()
        {
            int total = 0;

            foreach (var entry in counts)
            {
                total += entry.Value * (int)entry.Key;
            }

            return total;
        }

        /// <summary>
        /// Increments the count for the specified chip type by the given amount.
        /// </summary>
        /// <param name="type">The chip type to increment.</param>
        /// <param name="amount">The number of chips to add.</param>
        public void Add(ChipType type, int amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(amount);
            counts[type] += amount;
        }

        /// <summary>
        /// Decrements the count for the specified chip type by the given amount.
        /// </summary>
        /// <param name="type">The chip type to decrement.</param>
        /// <param name="amount">The number of chips to remove.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Remove(ChipType type, int amount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(amount);
            if (amount > counts[type]) throw new InvalidOperationException("Not enough chips of that type.");
            counts[type] -= amount;
        }

        /// <summary>
        /// Determines whether the specified amount of chips is less than or equal to the total chip value.
        /// </summary>
        /// <param name="amount">The number of chips to compare against the total value.</param>
        /// <returns>true if the amount is less than or equal to the total value; otherwise, false.</returns>
        public bool HasEnoughChips(int amount)
        {
            return amount <= GetTotalValue();
        }

        /// <summary>
        /// Removes chips from the stack up to the specified amount, using the largest denominations available.
        /// </summary>
        /// <param name="amount">The maximum amount of chips to remove from the stack.</param>
        /// <returns>A tuple containing the stack of chips taken and the remaining amount that could not be removed.</returns>
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
                        taken.AddStack(betStack);

                        int change = (int)type - remainder;
                        ChipStack changeStack = AmountToStack(change);
                        AddStack(changeStack);

                        remainder = 0;
                        break;
                    }
                }
            }

            return (taken, remainder);
        }

        public void AddStack(ChipStack other)
        {
            foreach (ChipType type in Enum.GetValues<ChipType>())
            {
                Add(type, other.GetCount(type));
            }
        }
    }
}
