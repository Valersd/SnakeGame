using System;
using System.Linq;
using System.Diagnostics;

using Game;


namespace Snake.Core
{
    public class FoodObject : GameObject, IGameObject
    {
        protected readonly Random _random;
        protected readonly Stopwatch _stopwatch;
        protected readonly IInfo _remainingTimeToEat;

        protected int _timeFoodBeEaten;
        protected int _timeToBomb;
        protected int _timeGetTimeBonus;

        public FoodObject(ICellFactory cellFactory, IGameEventFactory eventFactory, IConfigurationDataProvider dataProvider)
            : base(cellFactory, eventFactory)
        {
            _random = new Random();
            _stopwatch = new Stopwatch();
            _timeFoodBeEaten = int.Parse(dataProvider.Get("timeFoodBeEaten"));
            _remainingTimeToEat = new Info("Time to get a food", 600, ConsoleColor.DarkGreen, _timeFoodBeEaten, false);
            _info.Add(_remainingTimeToEat);
            _timeToBomb = int.Parse(dataProvider.Get("timeToBomb"));
            _timeGetTimeBonus = int.Parse(dataProvider.Get("timeGetTimeBonus"));
            
        }

        protected override IGameEvent Finish(IGameEvent currentEvent)
        {
            return GameOver(currentEvent);
        }

        protected override IGameEvent GameOver(IGameEvent currentEvent)
        {
            currentEvent.GameContext.RecycleBin.UnionWith(_body);
            currentEvent.GameContext.Used.UnionWith(_body);
            _body.Clear();
            _stopwatch.Start();
            _remainingTimeToEat.Parameter = _timeFoodBeEaten;
            return GetNextEvent("PlayAgain");
        }

        protected override IGameEvent LevelUp(IGameEvent currentEvent)
        {
            var food = _body.FirstOrDefault();
            currentEvent.GameContext.RecycleBin.Add(food);
            currentEvent.GameContext.Used.Remove(food);
            _body.Remove(food);
            _stopwatch.Start();
            _remainingTimeToEat.Parameter = _timeFoodBeEaten;
            return GetNextEvent("Start");
        }

        protected override IGameEvent EatImmortalMoveBonus(IGameEvent currentEvent)
        {
            return GetNextEvent("Move");
        }

        protected override IGameEvent EatSlowMoveBonus(IGameEvent currentEvent)
        {
            return GetNextEvent("Move");
        }

        protected override IGameEvent EatDoubleFoodBonus(IGameEvent currentEvent)
        {
            return GetNextEvent("Move");
        }

        protected override IGameEvent FoodMissed(IGameEvent currentEvent)
        {
            var food = _body.FirstOrDefault();
            currentEvent.GameContext.RecycleBin.Add(food);
            currentEvent.GameContext.Used.Remove(food);
            _body.Remove(food);
            _stopwatch.Start();
            _remainingTimeToEat.Parameter = _timeFoodBeEaten;
            return Start(CurrentEvent);
        }

        protected override IGameEvent Collision(IGameEvent currentEvent)
        {
            var food = _body.FirstOrDefault();
            currentEvent.GameContext.RecycleBin.Add(food);
            currentEvent.GameContext.Used.Remove(food);
            _body.Remove(food);
            _stopwatch.Restart();
            _remainingTimeToEat.Parameter = _timeFoodBeEaten;
            return GetNextEvent("Start");
        }

        protected override IGameEvent EatFood(IGameEvent currentEvent)
        {
            var food = _body.FirstOrDefault();
            currentEvent.GameContext.RecycleBin.UnionWith(_body);
            currentEvent.GameContext.Used.Remove(food);
            _body.Remove(food);

            CreateFood(currentEvent);
            _stopwatch.Restart();
            _remainingTimeToEat.Parameter = _timeFoodBeEaten;
            
            return GetNextEvent("Move");
        }

        protected override IGameEvent Move(IGameEvent currentEvent)
        {
            
            _remainingTimeToEat.Parameter =
                (double)(_timeFoodBeEaten * 1000 - _stopwatch.Elapsed.TotalMilliseconds) / 1000.0;
            if (_stopwatch.Elapsed.TotalSeconds > _timeToBomb 
                && _stopwatch.Elapsed.TotalSeconds <= _timeGetTimeBonus)
            {
                _body.FirstOrDefault().Color = ConsoleColor.DarkRed;
            }
            if (_stopwatch.Elapsed.TotalSeconds > _timeGetTimeBonus)
            {
                _body.FirstOrDefault().Color = ConsoleColor.Cyan;
            }
            if (_stopwatch.Elapsed.TotalMilliseconds >= _timeFoodBeEaten * 1000)
            {
                _remainingTimeToEat.Parameter = _timeFoodBeEaten;
                return GetNextEvent("FoodMissed");
            }
            return currentEvent;
        }

        protected override IGameEvent Start(IGameEvent currentEvent)
        {
            CurrentEvent = currentEvent;
            CurrentEvent.UserInput = default(ConsoleKey);
            CreateFood(currentEvent);
            _stopwatch.Restart();
            _remainingTimeToEat.Parameter = _timeFoodBeEaten;
            return GetNextEvent("Move");
        }

        private void CreateFood(IGameEvent currentEvent)
        {
            _body.Clear();
            ICell cell = default(ICell);
            do
            {
                int left = _random.Next(0, currentEvent.GameContext.PlayWidth - 1);
                int top = _random.Next(0, currentEvent.GameContext.Height - 1);
                cell = _cellFactory.Create("Food", left, top);
            } while (currentEvent.GameContext.Used.Contains(cell));
            _body.Add(cell);
            currentEvent.GameContext.Used.Add(cell);
        }
    }
}
