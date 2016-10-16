using System;

namespace Snake.Core
{
    public class SlowMoveBonus : Bonus
    {
        public SlowMoveBonus()
            :base()
        {
            Symbol = 'S';
        }

        public SlowMoveBonus(int left, int top)
            :this()
        {
            Left = left;
            Top = top;
        }
    }
}
