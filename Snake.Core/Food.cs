using System;
using Game;

namespace Snake.Core
{
    public class Food : Cell, ICell
    {
        public Food()
        {
            Symbol = '\u2665';
            Color = ConsoleColor.Red;
        }

        public Food(int left, int top)
            :this()
        {
            Left = left;
            Top = top;
        }
    }
}
