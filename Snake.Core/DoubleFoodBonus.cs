using System;

namespace Snake.Core
{
    public class DoubleFoodBonus : Bonus
    {
        public DoubleFoodBonus()
            :base()
        {
            Symbol = 'D';
        }

        public DoubleFoodBonus(int left, int top)
            :this()
        {
            Left = left;
            Top = top;
        }
    }
}
