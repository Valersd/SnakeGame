
using Game;

namespace Snake.Core.GameEvents
{
    public class EatImmortalMoveBonus : GameEvent, IGameEvent
    {
        public EatImmortalMoveBonus(IGameContext gameContext)
            : base(gameContext)
        {
        }
        public override int Priority { get { return 300; } }
    }
}
