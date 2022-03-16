using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ReversiRestApi.Model;

namespace ReversiRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {

        private readonly IGameRepository _iRepository;

        public GameController(IGameRepository repository)
        {
            _iRepository = repository;
        }

        // GET api/game
        [HttpGet]
        public ActionResult<IEnumerable<string>> WaitingDescriptions()
        {
            return _iRepository.GetWaitingGames().Select(x => x.Description).ToList();
        }

        // POST api/game
        [HttpPost]
        public void Create([FromBody] NewGame newGame)
        {
            Game game = new()
            {
                Player1Token = newGame.PlayerToken,
                Description = newGame.Description
            };
            _iRepository.AddGame(game);
        }

        [HttpGet("{gameToken}")]
        public ActionResult<Game> GameToken(string gameToken)
        {
            return _iRepository.GetGame(gameToken);
        }
        
        [HttpGet("gameplayer/{playerToken}")]
        public ActionResult<Game> PlayerToken(string playerToken)
        {
            return _iRepository.GetGameFromPlayerToken(playerToken);
        }

        [HttpGet("turn/{gameToken}")]
        public ActionResult<Color> PlayerTurn(string gameToken)
        {
            Game game = _iRepository.GetGame(gameToken);
            return game.PlayerTurn;
        }

        [HttpPut("move")]
        public ActionResult<bool> Move([FromBody] Move move)
        {
            Game game = _iRepository.GetGame(move.GameToken);
            if (!move.Verify(game)) return false;
            if(!game.MovePossible(move.MoveX, move.MoveY)) return false;

            if (move.Pass)
            {
                game.Pass();
            }
            else
            {
                game.Move(move.MoveX, move.MoveY);
            }

            return true;

        }

        [HttpPut("giveup")]
        public ActionResult<bool> GiveUp([FromBody] GameAuth gameAuth)
        {
            Game game = _iRepository.GetGame(gameAuth.GameToken);
            if (!gameAuth.Verify(game)) return false;
          
            game.GiveUp();
            return true;
            
        }

    }
    
    public class NewGame
    {
        public string PlayerToken { get; set; }
        public string Description { get; set; }
    }

    public class GameAuth
    {
        public string GameToken { get; set; }
        public string PlayerToken { get; set; }

        public bool Verify(Game game)
        {
            if (game == null || (game.Player1Token != PlayerToken && game.Player2Token != PlayerToken))
            {
                return false;
            }
            return true;
        }

    }

    public class Move : GameAuth
    {
        public string GameToken { get; set; }
        public string PlayerToken { get; set; }
        public int MoveX { get; set; }
        public int MoveY { get; set; }
        public bool Pass { get; set; }
    }
    
}
