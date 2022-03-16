using System;
using System.Collections.Generic;
using System.Linq;

namespace ReversiRestApi.Model
{
    public class GameRepository : IGameRepository
    {

        // List with temporary games
        public List<Game> Games { get; set; }

        public GameRepository()
        {
            Game game1 = new Game();
            Game game2 = new Game();
            Game game3 = new Game();

            game1.Player1Token = "abcdef";
            game1.Description = "Potje snel reveri, dus niet lang nadenken";
            game2.Player1Token = "ghijkl";
            game2.Player2Token = "mnopqr";
            game2.Description = "Ik zoek een gevorderde tegenspeler!";
            game3.Player1Token = "stuvwx";
            game3.Description = "Na dit spel wil ik er nog een paar spelen tegen zelfde tegenstander";


            Games = new List<Game> { game1, game2, game3 };
        }

        public void AddGame(Game game)
        {
            Games.Add(game);
        }

        public List<Game> GetGames()
        {
            return Games;
        }

        public Game GetGame(string gameToken)
        {
            return Games.FirstOrDefault(g => g.Token == gameToken);
        }

        public Game GetGameFromPlayerToken(string playerToken)
        {
            return Games.FirstOrDefault(g => g.Player1Token == playerToken || g.Player2Token == playerToken);
        }

        public List<Game> GetWaitingGames()
        {
            return Games.Where(g => g.Player2Token == null).ToList();
        }

    }
}
