using System;
using System.Threading;

using Game;

namespace Snake.Game
{
    class Program
    {
        static void Main(string[] args)
        {
            SnakeGameFactory snakeGameFactory = new SnakeGameFactory();
            IGameEngine engine = snakeGameFactory.GameEngine;
            
            engine.StartGame();

            while (true)
            {
                while (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    engine.ProceedCommand(key);
                }
                engine.Run();
                Thread.Sleep(engine.GameSpeed + engine.SlowFactor);
            }

        }
    }
}
