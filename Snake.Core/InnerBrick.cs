using System;

using Game;

namespace Snake.Core
{
    public class InnerBrick : Cell, ICell
    {
        public InnerBrick()
        {
            Symbol = '\u2593';
            Color = ConsoleColor.Gray;
        }

        public InnerBrick(int left, int top)
            :this()
        {
            Left = left;
            Top = top;
        }
    }
}
