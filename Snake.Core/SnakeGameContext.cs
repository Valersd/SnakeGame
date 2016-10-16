using System;

using Game;

namespace Snake.Core
{
    public class SnakeGameContext : GameContext, IGameContext
    {
        public SnakeGameContext
            (IConfigurationDataProvider dataProvider, 
            WallObject wall,
            SnakeObject snake, 
            FoodObject food,
            BonusObject bonus)
            :base(dataProvider, wall, snake, food, bonus)
        {
            Console.Title = "SNAKE";
        }
    }
}
