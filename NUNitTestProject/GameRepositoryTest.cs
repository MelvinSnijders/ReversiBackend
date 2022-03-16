using NUnit.Framework;
using ReversiRestApi.Model;

namespace NUnitTestProject
{
    [TestFixture]
    public class GameRepositoryTest
    {
        
        // game repo
        private IGameRepository _gameRepo;
        
        [SetUp]
        public void Setup()
        {
            // init game repo
            _gameRepo = new GameRepository();
        }
        
        [Test]
        public void GetGames_ShouldReturnAllGames()
        {
            var games = _gameRepo.GetGames();
            Assert.AreEqual(3, games.Count);
        }
        
        [Test]
        public void GetGame_ShouldReturnGame()
        {
            Game game = new Game();
            _gameRepo.AddGame(game);
            
            var gameFromRepo = _gameRepo.GetGame(game.Token);
            Assert.AreEqual(game, gameFromRepo);
        }
        
        [Test]
        public void AddGame_ShouldAddGame()
        {
            Game game = new Game();
            _gameRepo.AddGame(game);
            
            var games = _gameRepo.GetGames();
            Assert.AreEqual(4, games.Count);
        }
        
        [Test]
        public void GetWaitingGames_ShouldReturnWaitingGames()
        {
            var waitingGames = _gameRepo.GetWaitingGames();
            Assert.AreEqual(2, waitingGames.Count);
        }

    }
}