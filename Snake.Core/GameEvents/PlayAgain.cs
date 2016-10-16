using Game;

namespace Snake.Core.GameEvents
{
    public class PlayAgain : GameEvent, IGameEvent
    {
        public PlayAgain(IGameContext gameContext)
            :base(gameContext)
        {
        }
        public override int Priority { get { return 900; } }
    }
}
