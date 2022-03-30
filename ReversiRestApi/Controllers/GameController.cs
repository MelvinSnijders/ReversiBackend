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
        public ActionResult<IEnumerable<AvailableGame>> WaitingDescriptions()
        {
            return _iRepository.GetWaitingGames().Select(x => new AvailableGame() { ID = x.ID, Description = x.Description }).ToList();
        }

        // POST api/game
        [HttpPost]
        public void Create([FromBody] NewGame newGame)
        {
            Game game = new()
            {
                Player1Token = newGame.PlayerToken,
                Description = newGame.Description,
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
            Game game = _iRepository.GetGameFromPlayerToken(playerToken);
            if (game == null)
            {
                return NotFound();
            }
            return game;
        }

        [HttpGet("turn/{gameToken}")]
        public ActionResult<Color> PlayerTurn(string gameToken)
        {
            Game game = _iRepository.GetGame(gameToken);
            return game.PlayerTurn;
        }

        [HttpPost("join")]
        public ActionResult<Game> Join([FromBody] JoinGame join)
        {
            Game game = _iRepository.GetGameById(join.GameId);
            if (game == null) return NotFound("Game is not found");
            if (game.Player2Token != null) return NotFound("Game is already occupied");

            game.Player2Token = join.PlayerToken;
            game.PlayerTurn = Color.Black;
            _iRepository.UpdateGame(game);
            return game;
        }

        [HttpPut("move")]
        public ActionResult<MoveResult> Move([FromBody] Move move)
        {
            Game game = _iRepository.GetGame(move.GameToken);
            if (!move.Verify(game)) return MoveResult.Failed;

            // Check if the turn of the player
            if(
                (move.PlayerToken == game.Player1Token && game.PlayerTurn == Color.Black)||
                (move.PlayerToken == game.Player2Token && game.PlayerTurn == Color.White))
            {
                return MoveResult.NotYourTurn;
            }

            if (!game.MovePossible(move.MoveX, move.MoveY)) return MoveResult.InvalidMove;

                if (move.Pass)
                {
                    game.Pass();
                }
                else
                {
                    game.Move(move.MoveX, move.MoveY);
                    if (!game.AnyMovePossible(game.PlayerTurn))
                    {
                        game.Pass();
                    }
                }

            _iRepository.UpdateGame(game);

            return MoveResult.Success;

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

    public class JoinGame
    {
        public int GameId { get; set; }
        public string PlayerToken { get; set; }
    }

    public enum MoveResult
    {
        Success,
        Failed,
        InvalidMove,
        NotYourTurn
    }

    public class AvailableGame
    {

        public int ID { get; set; }
        public string Description { get; set; }
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
            if (game == null || (!game.Player1Token.Equals(PlayerToken) && !game.Player2Token.Equals(PlayerToken)))
            {
                return false;
            }
            return true;
        }

    }

    public class Move : GameAuth
    {
        public int MoveX { get; set; }
        public int MoveY { get; set; }
        public bool Pass { get; set; }
    }

}
