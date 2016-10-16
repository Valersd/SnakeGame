
using Game;

namespace Snake.Core.GameEvents
{
    public class EatFood : GameEvent, IGameEvent
    {
        public EatFood(IGameContext gameContext)
            : base(gameContext)
        {
        }
        public override int Priority { get { return 300; } }
    }
}
