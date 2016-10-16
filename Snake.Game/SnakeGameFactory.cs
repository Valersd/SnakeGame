
using Ninject;
using Ninject.Extensions.Factory;

using Game;
using Snake.Core;
using Snake.Core.GameEvents;

namespace Snake.Game
{
    public class SnakeGameFactory
    {
        private IGameEngine _gameEngine;

        public SnakeGameFactory()
        {
            RegisterBindings();
        }

        public IGameEngine GameEngine 
        { 
            get { return _gameEngine; } 
            private set { _gameEngine = value; } 
        }

        private void RegisterBindings()
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind<IGameEventFactory>().ToFactory(() => new UseFirstArgumentAsNameInstanceProvider());
            
            kernel.Bind<IConfigurationDataProvider>().To<ConfigurationDataProvider>();

            kernel.Bind<ICellFactory>().ToFactory(() => new UseFirstArgumentAsNameInstanceProvider());
            kernel.Bind<ICell>().To<Food>().Named("Food");
            kernel.Bind<ICell>().To<SnakePart>().Named("SnakePart");
            kernel.Bind<ICell>().To<Brick>().Named("Brick");
            kernel.Bind<ICell>().To<InnerBrick>().Named("InnerBrick");
            kernel.Bind<ICell>().To<DoubleFoodBonus>().Named("DoubleFoodBonus");
            kernel.Bind<ICell>().To<SlowMoveBonus>().Named("SlowMoveBonus");
            kernel.Bind<ICell>().To<ImmortalMoveBonus>().Named("ImmortalMoveBonus");
            
            kernel.Bind<IGameContext>().To<SnakeGameContext>();
            IGameContext gameContext = kernel.Get<IGameContext>();

            kernel.Bind<IGameEvent>().To<Start>().Named("Start")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<Collision>().Named("Collision")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<EatDoubleFoodBonus>().Named("EatDoubleFoodBonus")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<EatFood>().Named("EatFood")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<FoodMissed>().Named("FoodMissed")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<Move>().Named("Move")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<EatSlowMoveBonus>().Named("EatSlowMoveBonus")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<EatImmortalMoveBonus>().Named("EatImmortalMoveBonus")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<LevelUp>().Named("LevelUp")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<GameOver>().Named("GameOver")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<PlayAgain>().Named("PlayAgain")
               .WithConstructorArgument("gameContext", gameContext);
            kernel.Bind<IGameEvent>().To<Finish>().Named("Finish")
                .WithConstructorArgument("gameContext", gameContext);

            kernel.Bind<Start>().ToSelf().InSingletonScope();
            kernel.Bind<Collision>().ToSelf().InSingletonScope();
            kernel.Bind<EatDoubleFoodBonus>().ToSelf().InSingletonScope();
            kernel.Bind<EatFood>().ToSelf().InSingletonScope();
            kernel.Bind<FoodMissed>().ToSelf().InSingletonScope();
            kernel.Bind<Move>().ToSelf().InSingletonScope();
            kernel.Bind<SlowMoveBonus>().ToSelf().InSingletonScope();
            kernel.Bind<LevelUp>().ToSelf().InSingletonScope();
            kernel.Bind<GameOver>().ToSelf().InSingletonScope();
            kernel.Bind<PlayAgain>().ToSelf().InSingletonScope();
            kernel.Bind<Finish>().ToSelf().InSingletonScope();

            kernel.Bind<IPainter>().To<Painter>()
                .WithConstructorArgument("gameContext", gameContext);
            IPainter painter = kernel.Get<IPainter>();

            kernel.Bind<IGameEngine>().To<SnakeGameEngine>()
                .WithConstructorArgument("gameContext", gameContext)
                .WithConstructorArgument("painter", painter);

            _gameEngine = kernel.Get<IGameEngine>();
        }

    }
}
