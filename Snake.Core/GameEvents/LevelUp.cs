using Game;

namespace Snake.Core.GameEvents
{
    public class LevelUp : GameEvent, IGameEvent
    {
        public LevelUp(IGameContext gameContext)
            :base(gameContext)
        {
        }
        public override int Priority { get { return 600; } }
    }
}
