using Game;

namespace Snake.Core.GameEvents
{
    public class EatSlowMoveBonus : GameEvent, IGameEvent
    {
        public EatSlowMoveBonus(IGameContext gameContext)
            : base(gameContext)
        {
        }
        public override int Priority { get { return 300; } }
    }
}
