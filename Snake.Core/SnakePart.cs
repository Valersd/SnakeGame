using System;

using Game;

namespace Snake.Core
{
    public class SnakePart : Cell, ICell
    {
        public SnakePart()
        {
            Symbol = '@';
            Color = ConsoleColor.Green;
        }

        public SnakePart(int left, int top)
            :this()
        {
            Left = left;
            Top = top;
        }
    }
}
