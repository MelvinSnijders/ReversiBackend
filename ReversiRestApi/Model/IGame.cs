using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiRestApi.Model
{
    public enum Color { None, White, Black };

    public interface IGame
    {
        int ID { get; set; }
        string Description { get; set; }

        // The unique token of the game.
        string Token { get; set; }
        string Player1Token { get; set; }
        string Player2Token { get; set; }

        Color[,] Board { get; set; }
        Color PlayerTurn { get; set; }
        void Pass();
        bool Finished();

        // The color that is the most common on the board.
        Color DominantColor();

        // Check if a move is possible on a certain position.
        bool MovePossible(int row, int column);
        void Move(int row, int column);

        // Give up
        void GiveUp();
    }
}
