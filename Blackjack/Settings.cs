using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    /// <summary>
    /// Global game settings and timing helpers.
    /// </summary>
    internal class Settings
    {
        /// <summary>
        /// When true, all pauses are halved.
        /// </summary>
        public static bool FastMode = false;
        /// <summary>
        /// The number of standard 52-card decks used in the game.
        /// </summary>
        public static byte AmountOfDecks = 1;
        /// <summary>
        /// When true, skips waiting times and asking settings at the beginning.
        /// Also allows the use of the debug tools.
        /// </summary>
        public static readonly bool DebugMode = false;
        /// <summary>
        /// The amount of chips a player starts with by default.
        /// </summary>
        public const int DefaultStartAmount = 10000;

        private static bool forceSplit = false;

        /// <summary>
        /// Forces a split hand for testing. Only has an effect when <see cref="DebugMode"/> is true.
        /// </summary>
        public static bool ForceSplit
        {
            get => forceSplit && DebugMode;
            set => forceSplit = value;
        }

        /// <summary>
        /// Pauses the program, respecting <see cref="FastMode"/>.
        /// </summary>
        /// <param name="ms">The normal pause duration in milliseconds.</param>
        public static void Pause(int ms)
        {
            if (!DebugMode)
            {
                if (!FastMode)
                    Thread.Sleep(ms);
                else
                    Thread.Sleep(ms / 2);
            }
        }
    }
}
