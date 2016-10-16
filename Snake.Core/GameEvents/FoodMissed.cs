
using Game;

namespace Snake.Core.GameEvents
{
    public class FoodMissed : GameEvent, IGameEvent
    {
    public FoodMissed(IGameContext gameContext)
            : base(gameContext)
        {
        }
        public override int Priority { get { return 400; } }
    }
}
