using ReversiRestApi.Model;
using System.Collections.Generic;
using System.Linq;

namespace ReversiRestApi.DAL
{
    public class GameAccessLayer : IGameRepository
    {

        private readonly ReversiContext _context;

        public GameAccessLayer(ReversiContext context)
        {
            this._context = context;
        }

        public void AddGame(Game game)
        {
            _context.Games.Add(game);
            _context.SaveChanges();
        }

        public Game GetGame(string gameToken)
        {
            return _context.Games.Single(g => g.Token == gameToken);
        }

        public Game GetGameFromPlayerToken(string playerToken)
        {
            return _context.Games.Single(g => g.Player1Token == playerToken || g.Player2Token == playerToken);
        }

        public List<Game> GetGames()
        {
            return _context.Games.ToList();
        }

        public List<Game> GetWaitingGames()
        {
            return _context.Games.Where(g => g.Player2Token == null).ToList();
        }
    }
}
