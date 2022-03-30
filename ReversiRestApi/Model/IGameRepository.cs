using System.Collections.Generic;

namespace ReversiRestApi.Model
{
    public interface IGameRepository
    {
        void AddGame(Game game);
        public List<Game> GetGames();
        Game GetGame(string gameToken);
        Game GetGameFromPlayerToken(string playerToken);
        public List<Game> GetWaitingGames();
        void UpdateGame(Game game);
        Game GetGameById(int id);

    }
}
