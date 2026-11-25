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
        private static bool forceSplit = false;
        private static bool skipWaitTimes = false;
        /// <summary>
        /// The amount of chips a player starts with by default.
        /// </summary>
        public const int DefaultStartAmount = 10000;
        /// <summary>
        /// Represents the threshold value for reshuffling.
        /// </summary>
        public const int ReshuffleThreshold = 18;
        /// <summary>
        /// Represents the minimum hand value at which the dealer must stand.
        /// </summary>
        public const int DealerStandsOn = 17;
        /// <summary>
        /// Represents the score needed for a Blackjack. Also the max score before a bust.
        /// </summary>
        public const int BlackjackScore = 21;
        /// <summary>
        /// Defines the blackjack payout ratio.
        /// </summary>
        public const int BlackjackPayout = 3 / 2;
        /// <summary>
        /// Defines the payout multiplier for normal winnings.
        /// </summary>
        public const int NormalPayout = 2;
        /// <summary>
        /// The name of the player.
        /// </summary>
        public static string PlayerName = "Player";
        /// <summary>
        /// Decides if bets are on or off.
        /// </summary>
        public static bool BetsOn = false;

        /// <summary>
        /// Forces a split hand for testing. Only has an effect when <see cref="DebugMode"/> is true.
        /// </summary>
        public static bool ForceSplit
        {
            get => forceSplit && DebugMode;
            set => forceSplit = value;
        }

        /// <summary>
        /// Skips all <see cref="Pause(int)"/> for testing. Only has an effect when <see cref="DebugMode"/> is true.
        /// </summary>
        public static bool SkipWaitTimes
        {
            get => skipWaitTimes && DebugMode;
            set => skipWaitTimes = value;
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

        /// <summary>
        /// Prompts the user to configure startup options, including enabling Fast Mode, selecting the number of decks,
        /// and entering a player name.
        /// </summary>
        public static void AskStartupOptions()
        {
            if (!Settings.SkipWaitTimes)
            {
                string? answer;

                Console.Write("Do you want to enable Fast Mode? ([Y]es or [N]o): ");
                answer = Console.ReadLine()?.ToLower();
                if (answer == "y" || answer == "yes")
                    Settings.FastMode = true;

                Console.Write("How many Decks do you want to use?: ");
                answer = Console.ReadLine()?.ToLower();
                if (byte.TryParse(answer, out byte amount))
                {
                    if (amount == 0)
                        amount = 1;
                    Settings.AmountOfDecks = amount;
                }

                Console.Write("Do you want to enable betting? ([Y]es or [N]o): ");
                answer = Console.ReadLine()?.ToLower();
                if (answer == "y" || answer == "yes")
                    Settings.BetsOn = true;

                Console.Write("What would you like to be called?: ");
                answer = Console.ReadLine();
                if (answer != "")
                    Settings.PlayerName = answer!;
            }
        }
    }
}
