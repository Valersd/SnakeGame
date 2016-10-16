using Game;

namespace Snake.Core.GameEvents
{
    public class GameOver : GameEvent, IGameEvent
    {
        public GameOver(IGameContext gameContext)
            :base(gameContext)
        {
        }
        public override int Priority { get { return 700; } }
    }
}
