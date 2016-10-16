using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Game;


namespace Snake.Core
{
    public class SnakeObject : GameObject, IGameObject
    {
        protected readonly new Queue<ICell> _body;
        protected readonly IInfo _lifes;
        protected readonly IInfo _points;
        protected readonly IInfo _remainingFoods;
        protected readonly IInfo _remainingTimeToTimeBonus;
        protected readonly IInfo _timeToGetBomb;
        protected readonly IInfo _bomb;
        protected readonly Stopwatch _stopwatch;

        protected int _initialLifes;
        protected int _pointsToAdditionalLife;
        protected int _timeToGetTimeBonus;
        protected int _bombCount;
        protected int _timeToBomb;
        protected int _bombRange;

        protected bool _immortal;
        protected int _immortalMovePeriod;
        protected Stopwatch _immortalMoveStopwatch;
        public SnakeObject(ICellFactory cellFactory, IGameEventFactory eventFactory, IConfigurationDataProvider dataProvider)
            : base(cellFactory, eventFactory)
        {
            _body = new Queue<ICell>();
            _points = new Info("Points", 400, ConsoleColor.Magenta, 0, true);
            _info.Add(_points);
            _initialLifes = int.Parse(dataProvider.Get("lifes"));
            Lifes = _initialLifes;
            _pointsToAdditionalLife = int.Parse(dataProvider.Get("pointsToAdditionalLife"));
            InitialLength = int.Parse(dataProvider.Get("initialSnakeLength"));
            MaxLength = int.Parse(dataProvider.Get("maxSnakeLength"));
            _timeToBomb = int.Parse(dataProvider.Get("timeToBomb"));
            _bombRange = int.Parse(dataProvider.Get("bombRange"));
            CurrentDirection = Direction.Down;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _bombCount = 0;

            _immortal = false;
            _immortalMoveStopwatch = new Stopwatch();
            _immortalMovePeriod = int.Parse(dataProvider.Get("immortalMovePeriod"));

            _lifes = new Info("Lifes", 100, ConsoleColor.Green, Lifes, true);
            _info.Add(_lifes);
            _remainingFoods = new Info("Remaining foods", 500, ConsoleColor.Blue, MaxLength - Length, true);
            _info.Add(_remainingFoods);
            _timeToGetTimeBonus = int.Parse(dataProvider.Get("timeGetTimeBonus"));
            _remainingTimeToTimeBonus = new Info("Time to get a timebonus", 700, ConsoleColor.Cyan, _timeToGetTimeBonus, false);
            _info.Add(_remainingTimeToTimeBonus);
            _timeToGetBomb = new Info("Time to get a bomb", 800, ConsoleColor.DarkMagenta, _timeToBomb, false);
            _info.Add(_timeToGetBomb);
            _bomb = new Info("Bombs", 900, ConsoleColor.DarkRed, _bombCount, true);
            _info.Add(_bomb);
        }

        public override IEnumerable<ICell> Body { get { return _body; } }

        public int Lifes { get; set; }
        public int InitialLength { get; private set; }
        public int MaxLength { get; private set; }
        public int Length { get { return _body.Count == 0 ? InitialLength : _body.Count; } }

        public ICell Head { get { return _body.Last(); } }
        public ICell Tail { get { return _body.First(); } }

        public Direction CurrentDirection { get; set; }

        public override void Add(ICell cell)
        {
            _body.Enqueue(cell);
        }

        public override void Remove(ICell cell)
        {
            _body.Dequeue();
        }

        protected override IGameEvent Finish(IGameEvent currentEvent)
        {
            return GameOver(currentEvent);
        }

        protected override IGameEvent GameOver(IGameEvent currentEvent)
        {
            Lifes = _initialLifes;
            _lifes.Parameter = _initialLifes;
            _points.Parameter = 0;
            _bombCount = 0;
            _bomb.Parameter = 0;
            return GetNextEvent("PlayAgain");
        }

        protected override IGameEvent LevelUp(IGameEvent currentEvent)
        {
            _points.Parameter += 100;
            if (currentEvent.GameContext.Points % _pointsToAdditionalLife == 0)
            {
                Lifes++;
                _lifes.Parameter++;
            }
            currentEvent.GameContext.Used.ExceptWith(_body);
            currentEvent.GameContext.RecycleBin.UnionWith(_body);
            _body.Clear();
            return GetNextEvent("Start");
        }

        protected override IGameEvent EatImmortalMoveBonus(IGameEvent currentEvent)
        {
            _immortal = true;
            _immortalMoveStopwatch.Start();
            return GetNextEvent("Move");
        }

        protected override IGameEvent EatSlowMoveBonus(IGameEvent currentEvent)
        {
            _immortal = false;
            _immortalMoveStopwatch.Reset();
            return GetNextEvent("Move");
        }

        protected override IGameEvent EatDoubleFoodBonus(IGameEvent currentEvent)
        {
            _immortal = false;
            _immortalMoveStopwatch.Reset();
            return GetNextEvent("Move");
        }

        protected override IGameEvent FoodMissed(IGameEvent currentEvent)
        {
            return GetNextEvent("Move");
        }

        protected override IGameEvent Collision(IGameEvent currentEvent)
        {
            _immortal = false;
            _immortalMoveStopwatch.Reset();
            
            
            if (Lifes <= 0)
            {
                currentEvent.GameContext.Used.ExceptWith(_body);
                currentEvent.GameContext.RecycleBin.UnionWith(_body);
                _body.Clear();
                return GetNextEvent("GameOver");
            }
            currentEvent.GameContext.Used.ExceptWith(_body.Take(Length - 1));
            currentEvent.GameContext.RecycleBin.UnionWith(_body.Take(Length - 1));
            _body.Clear();
            Lifes--;
            _lifes.Parameter--;
            _stopwatch.Restart();
            return GetNextEvent("Start");
        }

        protected override IGameEvent EatFood(IGameEvent currentEvent)
        {
            _remainingFoods.Parameter = MaxLength - Length;
            if (_stopwatch.Elapsed.TotalSeconds <= _timeToBomb)
            {
                _timeToGetBomb.Parameter = _timeToBomb;
                _timeToGetBomb.Color = ConsoleColor.Green;
                _bombCount++;
                if (_bombCount != 0 && _bombCount % 3 == 0)
                {
                    _bomb.Parameter++;
                }
            }
            if (_stopwatch.Elapsed.TotalSeconds <= _timeToGetTimeBonus)
            {
                _remainingTimeToTimeBonus.Parameter = _timeToGetTimeBonus;
                _remainingTimeToTimeBonus.Color = ConsoleColor.Green;
                currentEvent.GameContext.Points += 100;
                _points.Parameter += 100;
                if (currentEvent.GameContext.Points % _pointsToAdditionalLife == 0)
                {
                    Lifes++;
                    _lifes.Parameter++;
                }
                currentEvent.GameContext.Points += 100;
                _points.Parameter += 100;
                if (currentEvent.GameContext.Points % _pointsToAdditionalLife == 0)
                {
                    Lifes++;
                    _lifes.Parameter++;
                }
                if (_remainingFoods.Parameter <= 0)
                {
                    _stopwatch.Restart();
                    return GetNextEvent("LevelUp");
                }
                _stopwatch.Restart();
                return GetNextEvent("Move");
            }
            currentEvent.GameContext.Points += 100;
            _points.Parameter += 100;
            if (currentEvent.GameContext.Points % _pointsToAdditionalLife == 0)
            {
                Lifes++;
                _lifes.Parameter++;
            }
            if (_remainingFoods.Parameter <= 0)
            {
                _stopwatch.Restart();
                return GetNextEvent("LevelUp");
            }
            _stopwatch.Restart();
            return GetNextEvent("Move");
        }

        protected override IGameEvent Move(IGameEvent currentEvent)
        {
            if (currentEvent.UserInput == ConsoleKey.Spacebar && _bomb.Parameter > 0)
            {
                currentEvent.UserInput = default(ConsoleKey);
                _bomb.Parameter--;
                int leftLimit = Head.Left - _bombRange < 2 ? 2 : Head.Left - _bombRange;
                int rightLimit = Head.Left + _bombRange > currentEvent.GameContext.PlayWidth - 2
                    ? currentEvent.GameContext.PlayWidth - 2 : Head.Left + _bombRange;
                int topLimit = Head.Top - _bombRange < 1 ? 1 : Head.Top - _bombRange;
                int bottomLimit = Head.Top + _bombRange > currentEvent.GameContext.Height - 2
                    ? currentEvent.GameContext.Height - 2 : Head.Top + _bombRange;
                var blasted = currentEvent.GameContext.Used.Where(c => c.GetType().Name == "InnerBrick"
                            && c.Left >= leftLimit && c.Left <= rightLimit
                            && c.Top >= topLimit && c.Top <= bottomLimit).ToList();

                currentEvent.GameContext.RecycleBin.UnionWith(blasted);
                foreach (var c in blasted)
                {
                    currentEvent.GameContext.GameObjects.First(o => !o.BePainted).Remove(c);
                }
                currentEvent.GameContext.Used.RemoveWhere(c => blasted.Contains(c));
                currentEvent.GameContext.Used.ExceptWith(blasted);
            }
            GetCurrentDirection(currentEvent);
            var newHead = GetNewHead();
            if (_immortal)
            {
                newHead.Color = ConsoleColor.Blue;
                if (_immortalMoveStopwatch.Elapsed.Seconds > _immortalMovePeriod)
                {
                    _immortalMoveStopwatch.Reset();
                    _immortal = false;
                    newHead.Color = ConsoleColor.Green;
                }
            }
            switch (CurrentDirection)
            {
                case Direction.Down: newHead.Symbol = '\u25bc'; break;
                case Direction.Up: newHead.Symbol = '\u25b2'; break;
                case Direction.Left: newHead.Symbol = '\u25c4'; break;
                case Direction.Right: newHead.Symbol = '\u25ba'; break;
            }
            if (_stopwatch.Elapsed.TotalSeconds > _timeToGetTimeBonus)
            {
                _remainingTimeToTimeBonus.Parameter = 0;
                _remainingTimeToTimeBonus.Color = ConsoleColor.Cyan;
            }
            else
            {
                _remainingTimeToTimeBonus.Parameter =
                    (double)(_timeToGetTimeBonus * 1000 - _stopwatch.Elapsed.TotalMilliseconds) / 1000.0;
            }
            if (_stopwatch.Elapsed.TotalSeconds > _timeToBomb)
            {
                _timeToGetBomb.Parameter = 0;
                _timeToGetBomb.Color = ConsoleColor.DarkMagenta;
            }
            else
            {
                _timeToGetBomb.Parameter =
                    (double)(_timeToBomb * 1000 - _stopwatch.Elapsed.TotalMilliseconds) / 1000;
            }
            var tail = default(ICell);
            var cell = currentEvent.GameContext.Used.FirstOrDefault(c => c.Equals(newHead));
            if(cell != null)
            {
                if (_immortal)
                {
                    if (cell is InnerBrick || cell is SnakePart)
                    {
                        tail = _body.Dequeue();
                        currentEvent.GameContext.Used.Remove(tail);
                        currentEvent.GameContext.RecycleBin.Add(tail);
                        _body.Enqueue(newHead);
                        currentEvent.GameContext.Used.Add(newHead);
                        currentEvent.GameContext.RecycleBin.Add(cell);
                        currentEvent.GameContext.Used.Remove(cell);
                        currentEvent.GameContext.GameObjects.First(o => !o.BePainted).Remove(cell);
                        return GetNextEvent("Move");
                    }
                }
                else
                {
                    if (cell is InnerBrick || cell is SnakePart)
                    {
                        tail = _body.Dequeue();
                        currentEvent.GameContext.Used.Remove(tail);
                        currentEvent.GameContext.RecycleBin.Add(tail);
                        _body.Enqueue(newHead);
                        currentEvent.GameContext.Used.Add(newHead);
                        return GetNextEvent("Collision");
                    }
                }
                if (cell is Brick)
                {
                    tail = _body.Dequeue();
                    currentEvent.GameContext.Used.Remove(tail);
                    currentEvent.GameContext.RecycleBin.Add(tail);
                    _body.Enqueue(newHead);
                    currentEvent.GameContext.Used.Add(newHead);
                    return GetNextEvent("Collision");
                    
                }
                else if (cell is Food)
                {
                    _body.Enqueue(newHead);
                    currentEvent.GameContext.Used.Add(newHead);
                    return GetNextEvent("EatFood");
                }
                else if (cell is DoubleFoodBonus)
                {
                    tail = _body.Dequeue();
                    currentEvent.GameContext.Used.Remove(tail);
                    currentEvent.GameContext.RecycleBin.Add(tail);
                    _body.Enqueue(newHead);
                    currentEvent.GameContext.Used.Add(newHead);
                    return GetNextEvent("EatDoubleFoodBonus");
                }
                else if (cell is SlowMoveBonus)
                {
                    tail = _body.Dequeue();
                    currentEvent.GameContext.Used.Remove(tail);
                    currentEvent.GameContext.RecycleBin.Add(tail);
                    _body.Enqueue(newHead);
                    currentEvent.GameContext.Used.Add(newHead);
                    return GetNextEvent("EatSlowMoveBonus");
                }
                else if (cell is ImmortalMoveBonus)
                {
                    tail = _body.Dequeue();
                    currentEvent.GameContext.Used.Remove(tail);
                    currentEvent.GameContext.RecycleBin.Add(tail);
                    _body.Enqueue(newHead);
                    currentEvent.GameContext.Used.Add(newHead);
                    return GetNextEvent("EatImmortalMoveBonus");
                }
            }
            
            tail = _body.Dequeue();
            currentEvent.GameContext.Used.Remove(tail);
            currentEvent.GameContext.RecycleBin.Add(tail);
            _body.Enqueue(newHead);
            currentEvent.GameContext.Used.Add(newHead);
            return GetNextEvent("Move");
        }
        protected override IGameEvent Start(IGameEvent currentEvent)
        {
            _immortal = false;
            _immortalMoveStopwatch.Reset();
            CurrentEvent = currentEvent;
            CurrentEvent.UserInput = default(ConsoleKey);
            //currentEvent.GameContext.Used.RemoveWhere(o => Body.Take(Length - 1).Contains(o));
            //currentEvent.GameContext.RecycleBin.UnionWith(_body.Take(Length - 1));
            //_body.Clear();

            _remainingTimeToTimeBonus.Color = ConsoleColor.Cyan;
            _timeToGetBomb.Color = ConsoleColor.DarkMagenta;
            int count = 1;
            while (count < InitialLength)
            {
                ICell cell = _cellFactory.Create("SnakePart", 1, count);
                _body.Enqueue(cell);
                currentEvent.GameContext.Used.Add(cell);
                count++;
            }
            ICell head = _cellFactory.Create("SnakePart", 1, InitialLength);
            CurrentDirection = Direction.Down;
            head.Symbol = '\u25bc';
            _body.Enqueue(head);
            currentEvent.GameContext.Used.Add(head);
            _remainingFoods.Parameter = MaxLength - Length;
            _remainingTimeToTimeBonus.Parameter = _timeToGetTimeBonus;
            _stopwatch.Restart();
            return GetNextEvent("Move");
        }

        private void GetCurrentDirection(IGameEvent currentEvent)
        {
            if (currentEvent.UserInput != default(ConsoleKey))
            {
                var key = currentEvent.UserInput;
                if (key == ConsoleKey.DownArrow)
                {
                    if (CurrentDirection == Direction.Up)
                    {
                        CurrentDirection = Direction.Up;
                    }
                    else
                    {
                        CurrentDirection = Direction.Down;
                    }
                }
                else if (key == ConsoleKey.LeftArrow)
                {
                    if (CurrentDirection == Direction.Right)
                    {
                        CurrentDirection = Direction.Right;
                    }
                    else
                    {
                        CurrentDirection = Direction.Left;
                    }
                }
                else if (key == ConsoleKey.RightArrow)
                {
                    if (CurrentDirection == Direction.Left)
                    {
                        CurrentDirection = Direction.Left;
                    }
                    else
                    {
                        CurrentDirection = Direction.Right;
                    }
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    if (CurrentDirection == Direction.Down)
                    {
                        CurrentDirection = Direction.Down;
                    }
                    else
                    {
                        CurrentDirection = Direction.Up;
                    }
                }
            }
        }

        private ICell GetNewHead()
        {
            int left = 0;
            int top = 0;
            var currentHead = Head;
            Head.Symbol = '@';
            Head.Color = ConsoleColor.Green;
            int currenLeft = currentHead.Left;
            int currentTop = currentHead.Top;
            switch (CurrentDirection)
            {
                case Direction.Down:
                    left = 0; top = 1;
                    break;
                case Direction.Up:
                    left = 0; top = -1;
                    break;
                case Direction.Left:
                    left = -1; top = 0;
                    break;
                case Direction.Right:
                    left = 1; top = 0;
                    break;
            }
            return _cellFactory.Create("SnakePart", currenLeft + left, currentTop + top);
        }
    }
}
