using System;

using Game;

namespace Snake.Core
{
    public abstract class Bonus : Cell, ICell
    {
        public Bonus()
        {
            Color = ConsoleColor.Magenta;
        }
    }
}
