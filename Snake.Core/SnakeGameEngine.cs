using System;
using System.Linq;

using Game;

namespace Snake.Core
{
    public class SnakeGameEngine : GameEngine, IGameEngine
    {
        protected IGameObject _additionalFood;
        protected int _gameSpeed;
        protected int _slowFactor;
        public SnakeGameEngine
            (IGameContext gameContext, 
            IPainter painter, 
            IGameEventFactory gameEventFactory, 
            FoodObject additionalFood)
            :base(gameContext, painter, gameEventFactory)
        {
            _gameSpeed = 80;
            _slowFactor = 0;
            CurrentEvent = _eventFactory.Create("Start", _gameContext);
            _additionalFood = additionalFood;
        }

        public override int GameSpeed { get { return _gameSpeed; } protected set { _gameSpeed = value; } }
        public override int SlowFactor { get { return _slowFactor; } protected set { _slowFactor = value; } }

        public override void StartGame()
        {
            while (true)
            {
                ShowGameRules();
                if (!Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    break;
                }
            }
        }


        public override void ProceedCommand(ConsoleKey input)
        {
            if (CurrentEvent.Name == "Move")
            {
                switch (input)
                {
                    case ConsoleKey.DownArrow:
                        CurrentEvent.UserInput = ConsoleKey.DownArrow;
                        break;
                    case ConsoleKey.UpArrow:
                        CurrentEvent.UserInput = ConsoleKey.UpArrow;
                        break;
                    case ConsoleKey.LeftArrow:
                        CurrentEvent.UserInput = ConsoleKey.LeftArrow;
                        break;
                    case ConsoleKey.RightArrow:
                        CurrentEvent.UserInput = ConsoleKey.RightArrow;
                        break;
                    case ConsoleKey.Spacebar:
                        CurrentEvent.UserInput = ConsoleKey.Spacebar;
                        break;
                    case ConsoleKey.P:
                        while (true)
                        {
                            if (!Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.P)
                            {
                                break;
                            }
                        }
                        break;
                }
            }
        }

        public override void Run()
        {
            foreach (var gameObject in _gameContext.GameObjects)
            {
                gameObject.Execute(CurrentEvent);
            }
            CurrentEvent = _gameContext.GameObjects.Max(o => o.CurrentEvent);
            switch (CurrentEvent.Name)
            {
                case "EatImmortalMoveBonus":
                    EatImmortalMoveBonus();
                    break;
                case "EatDoubleFoodBonus":
                    EatDoubleFoodBonus();
                    break;
                case "EatSlowMoveBonus":
                    EatSlowMoveBonus();
                    break;
                case "Start":
                    Start();
                    break;
                case "LevelUp":
                    LevelUpGameOverFinish
                        (" Level completed. Press enter to continue... ", ConsoleColor.Yellow);
                    break;
                case "GameOver":
                    LevelUpGameOverFinish
                        (" G a m e  o v e r  ! ! ! ", ConsoleColor.Red);
                    break;
                case "Finish":
                    LevelUpGameOverFinish
                        (" C o n g r a t u l a t i o n s ! ! ! ", ConsoleColor.Green);
                    break;
                case "PlayAgain":
                    _gameContext.Points = 0;
                    CurrentEvent = _eventFactory.Create("Start", _gameContext);
                    break;
            }
            Painter.Draw();
        }

        protected void EatImmortalMoveBonus()
        {
            RemoveGameObject(_additionalFood);
            SlowFactor = 0;
        }

        protected void EatDoubleFoodBonus()
        {
            if (_gameContext.GameObjects.Count(c => c.GetType().Name == "FoodObject") == 1)
            {
                _additionalFood.Execute(_eventFactory.Create("Start", _gameContext));
                GetGameObject(_additionalFood);
            }
            SlowFactor = 0;
        }

        protected void EatSlowMoveBonus()
        {
            SlowFactor = GameSpeed / 2;
            RemoveGameObject(_additionalFood);
        }

        protected void Start()
        {
            RemoveGameObject(_additionalFood);
            SlowFactor = 0;
        }

        protected void LevelUpGameOverFinish(string text, ConsoleColor color)
        {
            int left = _gameContext.PlayWidth / 2 - text.Length / 2;
            int top = _gameContext.Height / 2;
            Console.SetCursorPosition(left, top);
            Console.ForegroundColor = color;
            Console.Write(text.ToUpper());
            while (true)
            {
                if (!Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(left, top);
                    Console.Write(new string(' ', text.Length));
                    break;
                }
            }
        }

        protected void RemoveGameObject(IGameObject obj)
        {
            _gameContext.Remove(obj);
        }

        protected void GetGameObject(IGameObject obj)
        {
            _gameContext.Add(obj);
        }


        protected void ShowGameRules()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("\t\tGAME RULES:");
            Console.WriteLine();
            Console.WriteLine("\tLEFT ARROW - Move left");
            Console.WriteLine();
            Console.WriteLine("\tRIGHT ARROW - Move right");
            Console.WriteLine();
            Console.WriteLine("\tUP ARROW - Move up");
            Console.WriteLine();
            Console.WriteLine("\tDOWN ARROW - Move down");
            Console.WriteLine();
            Console.WriteLine("\tSPACE - Detonate bomb. The nearest inner walls will be damaged");
            Console.WriteLine();
            Console.WriteLine("\tP - Pause");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("\tLevel - At every new level you will get additional points");
            Console.WriteLine();
            Console.WriteLine("\tPoints - At every 3000 points you will get an additional life");
            Console.WriteLine();
            Console.WriteLine("\tRemaining foods - Number of foods you have to eat before proceeding to the next level");
            Console.WriteLine();
            Console.WriteLine("\tTime to get a food - If it's expired, the food will move to another place");
            Console.WriteLine();
            Console.WriteLine("\tTime to get a timebonus - If it's not expired, you will get additional points");
            Console.WriteLine();
            Console.WriteLine("\tTime to get a bomb - Every three times if it's not expired, you will get a bomb");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("\t\tBONUSES (life span - 10 sec): ");
            Console.WriteLine();
            Console.WriteLine("\tS - Slow move. The speed will decrease by 50%");
            Console.WriteLine();
            Console.WriteLine("\tD - Double food. An additional food will appear");
            Console.WriteLine();
            Console.WriteLine("\tI - Immortality for a while. The inner walls will be no longer obstacles");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("\t\t\tPress ENTER to start ...");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\t\t\t\t\t\tSophisticated - 2016");
        }
    }
}
