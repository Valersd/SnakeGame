using System;

namespace Snake.Core
{
    public class ImmortalMoveBonus : Bonus
    {
        public ImmortalMoveBonus()
            :base()
        {
            Symbol = 'I';
        }

        public ImmortalMoveBonus(int left, int top)
            : this()
        {
            Left = left;
            Top = top;
        }
    }

}
