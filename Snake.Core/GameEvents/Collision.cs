
using Game;

namespace Snake.Core.GameEvents
{
    public class Collision : GameEvent, IGameEvent
    {
        public Collision(IGameContext gameContext)
            :base(gameContext)
        {
        }
        public override int Priority { get { return 500; } }
    }
}
