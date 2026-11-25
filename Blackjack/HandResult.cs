using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    /// <summary>
    /// Specifies the possible outcomes of a hand in a blackjack game.
    /// </summary>
    internal enum HandResult
    {
        Win,
        Loss,
        Push,
        Busted,
        Blackjack
    }
}
