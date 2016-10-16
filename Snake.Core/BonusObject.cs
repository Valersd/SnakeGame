using System;
using System.Linq;
using System.Diagnostics;

using Game;

namespace Snake.Core
{
    public class BonusObject : GameObject, IGameObject
    {
        protected readonly Random _random;
        protected readonly Stopwatch _stopwatch;

        protected int _bonusPossibilityPercent;
        protected int _remainingTime;

        public BonusObject(ICellFactory cellFactory, IGameEventFactory eventFactory, IConfigurationDataProvider dataProvider)
            : base(cellFactory, eventFactory)
        {
            _random = new Random();
            _stopwatch = new Stopwatch();
            _remainingTime = int.Parse(dataProvider.Get("remainingTime"));
            _bonusPossibility = int.Parse(dataProvider.Get("bonusPossibility"));
        }

        protected override IGameEvent Finish(IGameEvent currentEvent)
        {
            return GameOver(currentEvent);
        }

        protected override IGameEvent GameOver(IGameEvent currentEvent)
        {
            _stopwatch.Reset();
            RemoveBonus(currentEvent);
            return GetNextEvent("PlayAgain");
        }

        protected override IGameEvent LevelUp(IGameEvent currentEvent)
        {
            _stopwatch.Reset();
            RemoveBonus(currentEvent);
            return GetNextEvent("Start");
        }

        protected override IGameEvent EatImmortalMoveBonus(IGameEvent currentEvent)
        {
            _stopwatch.Reset();
            RemoveBonus(currentEvent);
            return GetNextEvent("Move");
        }

        protected override IGameEvent EatSlowMoveBonus(IGameEvent currentEvent)
        {
            _stopwatch.Reset();
            RemoveBonus(currentEvent);
            return GetNextEvent("Move");
        }

        protected override IGameEvent EatDoubleFoodBonus(IGameEvent currentEvent)
        {
            _stopwatch.Reset();
            RemoveBonus(currentEvent);
            return GetNextEvent("Move");
        }

        protected override IGameEvent Move(IGameEvent currentEvent)
        {
            if (_body.Any())
            {
                if (_stopwatch.Elapsed.Seconds >= _remainingTime)
                {
                    _stopwatch.Reset();
                    RemoveBonus(currentEvent);
                }
            }
            else
            {
                if (_random.Next(1000) < _bonusPossibility)
                {
                    CreateBonus(currentEvent);
                    _stopwatch.Start();
                }
            }
            return currentEvent;
        }

        protected override IGameEvent Collision(IGameEvent currentEvent)
        {
            if (_body.Any())
            {
                RemoveBonus(currentEvent);
            }
            return GetNextEvent("Start");
        }

        protected override IGameEvent FoodMissed(IGameEvent currentEvent)
        {
            return GetNextEvent("Move");
        }

        protected override IGameEvent EatFood(IGameEvent currentEvent)
        {
            return GetNextEvent("Move");
        }

        protected override IGameEvent Start(IGameEvent currentEvent)
        {
            CurrentEvent = currentEvent;
            CurrentEvent.UserInput = default(ConsoleKey);
            return GetNextEvent("Move");
        }

        private void CreateBonus(IGameEvent currentEvent)
        {
            ICell cell = default(ICell);
            do
            {
                int left = _random.Next(0, currentEvent.GameContext.PlayWidth - 1);
                int top = _random.Next(0, currentEvent.GameContext.Height - 1);
                int choice = _random.Next(3);
                switch (choice)
                {
                    case 0: cell = _cellFactory.Create("DoubleFoodBonus", left, top); break;
                    case 1: cell = _cellFactory.Create("SlowMoveBonus", left, top); break;
                    case 2: cell = _cellFactory.Create("ImmortalMoveBonus", left, top); break;
                }
            } while (currentEvent.GameContext.Used.Contains(cell));
            _body.Add(cell);
            currentEvent.GameContext.Used.Add(cell);
        }

        private void RemoveBonus(IGameEvent currentEvent)
        {
            _stopwatch.Reset();
            currentEvent.GameContext.RecycleBin.UnionWith(_body);
            currentEvent.GameContext.Used.ExceptWith(_body);
            _body.Clear();
        }

        public int _bonusPossibility { get; set; }
    }
}
