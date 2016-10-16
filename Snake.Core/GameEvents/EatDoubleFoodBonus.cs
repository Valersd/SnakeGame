using Game;

namespace Snake.Core.GameEvents
{
    public class EatDoubleFoodBonus : GameEvent, IGameEvent
    {
        public EatDoubleFoodBonus(IGameContext gameContext)
            : base(gameContext)
        {
        }
        public override int Priority { get { return 300; } }
    }
}
