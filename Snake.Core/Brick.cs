using System;

using Game;

namespace Snake.Core
{
    public class Brick : Cell, ICell
    {
        public Brick()
        {
            Symbol = '\u2593';
            Color = ConsoleColor.Gray;
        }

        public Brick(int left, int top)
            :this()
        {
            Left = left;
            Top = top;
        }
    }
}
