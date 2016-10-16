
using Game;

namespace Snake.Core.GameEvents
{
    public class Move : GameEvent, IGameEvent
    {
        public Move(IGameContext gameContext)
            :base(gameContext)
        {
        }
        public override int Priority { get { return 200; } }
    }
}
