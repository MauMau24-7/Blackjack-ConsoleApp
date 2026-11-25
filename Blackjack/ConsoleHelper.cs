using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    /// <summary>
    /// Provides helper methods for console text formatting and output.
    /// </summary>
    internal static class ConsoleHelper
    {
        /// <summary>
        /// Colors the specified text in the given color.
        /// </summary>
        /// <param name="coloredText">The text to color.</param>
        /// <param name="color">The color to use.</param>
        /// <param name="textBefore">The text before <paramref name="coloredText"/></param>
        /// <param name="textAfter">The text after <paramref name="coloredText"/></param>
        public static void ColorWordInText(string coloredText, ConsoleColor color, string textBefore = "", string textAfter = "")
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.Write(textBefore);
            Console.ForegroundColor = color;
            Console.Write(coloredText);
            Console.ForegroundColor = originalColor;
            Console.WriteLine(textAfter);
        }
    }
}
