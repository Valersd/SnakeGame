using Game;

namespace Snake.Core.GameEvents
{
    public class Finish : GameEvent, IGameEvent
    {
        public Finish(IGameContext gameContext)
            :base(gameContext)
        {
        }
        public override int Priority { get { return 700; } }
    }
}
