using System;
using System.Collections.Generic;

using Game;

using Snake.Core.GameEvents;

namespace Snake.Core
{
    public class WallObject : GameObject, IGameObject
    {
        protected readonly Random _random;
        protected readonly IInfo _level;
        public WallObject(ICellFactory cellFactory, IGameEventFactory eventFactory, IConfigurationDataProvider dataProvider)
            : base(cellFactory, eventFactory)
        {
            _random = new Random();
            Level = 1;
            MaxLevel = int.Parse(dataProvider.Get("maxLevel"));
            WallSpreadDencity = int.Parse(dataProvider.Get("wallSpreadDencity"));
            MinWallLength = int.Parse(dataProvider.Get("minWallLength"));
            MaxWallLength = int.Parse(dataProvider.Get("maxWallLength"));
            _level = new Info("Level", 200, ConsoleColor.DarkGray, Level, true);
            _info.Add(_level);
        }

        public int Level { get; set; }
        public int MaxLevel { get; protected set; }
        public int WallSpreadDencity { get; protected set; }
        public int MinWallLength { get; protected set; }
        public int MaxWallLength { get; protected set; }

        protected override IGameEvent Finish(IGameEvent currentEvent)
        {
            return GameOver(currentEvent);
        }

        protected override IGameEvent GameOver(IGameEvent currentEvent)
        {
            Level = 1;
            _level.Parameter = 1;
            currentEvent.GameContext.Points = 0;
            currentEvent.GameContext.RecycleBin.UnionWith(_body);
            currentEvent.GameContext.Used.ExceptWith(_body);
            _body.Clear();
            BePainted = true;
            return GetNextEvent("PlayAgain");
        }

        protected override IGameEvent LevelUp(IGameEvent currentEvent)
        {
            if (Level >= MaxLevel)
            {
                return GetNextEvent("Finish");
            }
            Level++;
            _level.Parameter++;
            currentEvent.GameContext.RecycleBin.UnionWith(_body);
            currentEvent.GameContext.Used.ExceptWith(_body);
            _body.Clear();
            BePainted = true;
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
            return GetNextEvent("Move");
        }

        protected override IGameEvent Collision(IGameEvent currentEvent)
        {
            BePainted = true;
            return GetNextEvent("Start");
        }

        protected override IGameEvent EatFood(IGameEvent currentEvent)
        {
            BePainted = false;
            return GetNextEvent("Move");
        }

        protected override IGameEvent Move(IGameEvent currentEvent)
        {
            BePainted = false;
            return currentEvent;
        }

        protected override IGameEvent Start(IGameEvent currentEvent)
        {
            CurrentEvent = currentEvent;
            CurrentEvent.UserInput = default(ConsoleKey);
            if (_body.Count == 0)
            {
                HashSet<ICell> wall = new HashSet<ICell>();
                CreateWall("Brick", currentEvent, Orientation.Horizontal, 0, 0, currentEvent.GameContext.PlayWidth, ref wall);
                CreateWall("Brick", currentEvent, Orientation.Vertical, 0, 0, currentEvent.GameContext.Height, ref wall);
                CreateWall("Brick", currentEvent, Orientation.Horizontal, 0, currentEvent.GameContext.Height - 1, currentEvent.GameContext.PlayWidth, ref wall);
                CreateWall("Brick", currentEvent, Orientation.Vertical, currentEvent.GameContext.PlayWidth - 1, 0, currentEvent.GameContext.Height, ref wall);

                int innerWallCount = WallSpreadDencity * (Level - 1);
                int count = 0;
                while (count < innerWallCount)
                {
                    Orientation orientation = _random.Next(0, 2) == 0 ? Orientation.Horizontal : Orientation.Vertical;
                    int startLeft = _random.Next(2, currentEvent.GameContext.PlayWidth - 1);
                    int startTop = _random.Next(0, currentEvent.GameContext.Height - 1);
                    int size = _random.Next(MinWallLength, MaxWallLength + 1);
                    if (CreateWall("InnerBrick", currentEvent, orientation, startLeft, startTop, size, ref wall))
                    {
                        count++;
                    }
                }
                _body.UnionWith(wall);
                currentEvent.GameContext.Used.UnionWith(wall);
            }
            return GetNextEvent("Move");
        }

        private bool CreateWall
            (string material, IGameEvent gameEvent, Orientation orientation, int startLeft, int startTop, int size, ref HashSet<ICell> wall)
        {
            ICell endCell = default(ICell);
            switch (orientation)
            {
                case Orientation.Horizontal:
                    endCell = _cellFactory.Create(material, startLeft + size - 1, startTop);
                    if (!gameEvent.GameContext.ValidatePosition(endCell))
                    {
                        return false;
                    }
                    for (int i = startLeft; i < startLeft + size - 1; i++)
                    {
                        wall.Add(_cellFactory.Create(material, i, startTop));
                    }
                    wall.Add(endCell);
                    return true;
                case Orientation.Vertical:
                    endCell = _cellFactory.Create(material, startLeft, startTop + size - 1);
                    if (!gameEvent.GameContext.ValidatePosition(endCell))
                    {
                        return false;
                    }
                    for (int i = startTop; i < startTop + size - 1; i++)
                    {
                        wall.Add(_cellFactory.Create(material, startLeft, i));
                    }
                    wall.Add(endCell);
                    return true;
            }
            return false;
        }
    }
}
