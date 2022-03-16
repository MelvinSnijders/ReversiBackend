using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReversiRestApi.Model
{
    public class Game : IGame
    {
        private const int BoardSize = 8;
        private readonly int[,] Directions = new int[8, 2] {
                                {  0,  1 },         // to the right
                                {  0, -1 },         // to the left
                                {  1,  0 },         // down
                                { -1,  0 },         // up
                                {  1,  1 },         // to the bottom right
                                {  1, -1 },         // to the bottom left
                                { -1,  1 },         // to the upper right
                                { -1, -1 } };       // to the upper left

        public int ID { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public string Player1Token { get; set; }
        public string Player2Token { get; set; }

        private Color[,] _board;
        public Color[,] Board
        {
            get
            {
                return _board;
            }
            set
            {
                _board = value;
            }
        }

        public Color PlayerTurn { get; set; }
        public Game()
        {
            Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            Token = Token.Replace("/", "q");    // slash mijden ivm het opvragen van een spel via een api obv het token
            Token = Token.Replace("+", "r");    // plus mijden ivm het opvragen van een spel via een api obv het token

            Board = new Color[BoardSize, BoardSize];
            Board[3, 3] = Color.White;
            Board[4, 4] = Color.White;
            Board[3, 4] = Color.Black;
            Board[4, 3] = Color.Black;

            PlayerTurn = Color.None;
        }

        public void Pass()
        {
            // controleeer of er geen zet mogelijk is voor de speler die wil passen, alvorens van beurt te wisselen.
            if (AnyMovePossible(PlayerTurn))
                throw new Exception("Passen mag niet, er is nog een zet mogelijk");
            else
                SwapTurn();
        }


        public bool Finished()     // return true als geen van de spelers een zet kan doen
        {
            return !AnyMovePossible(Color.White) && !AnyMovePossible(Color.Black);
        }

        public Color DominantColor()
        {
            int aantalWit = 0;
            int aantalZwart = 0;
            for (int rijZet = 0; rijZet < BoardSize; rijZet++)
            {
                for (int kolomZet = 0; kolomZet < BoardSize; kolomZet++)
                {
                    if (_board[rijZet, kolomZet] == Color.White)
                        aantalWit++;
                    else if (_board[rijZet, kolomZet] == Color.Black)
                        aantalZwart++;
                }
            }
            if (aantalWit > aantalZwart)
                return Color.White;
            if (aantalZwart > aantalWit)
                return Color.Black;
            return Color.None;
        }

        public bool MovePossible(int rijZet, int kolomZet)
        {
            if (!WithinBorders(rijZet, kolomZet))
                throw new Exception($"Zet ({rijZet},{kolomZet}) ligt buiten het bord!");
            return MovePossible(rijZet, kolomZet, PlayerTurn);
        }

        public void Move(int rijZet, int kolomZet)
        {
            if (!MovePossible(rijZet, kolomZet))
            {
                throw new Exception($"Zet ({rijZet},{kolomZet}) is niet mogelijk!");
            }

            for (int i = 0; i < BoardSize; i++)
            {
                ReversePieces(rijZet, kolomZet, PlayerTurn, Directions[i, 0], Directions[i, 1]);
            }

            Board[rijZet, kolomZet] = PlayerTurn;

            SwapTurn();

        }

        private static Color OppositeColor(Color kleur)
        {
            return kleur switch
            {
                Color.White => Color.Black,
                Color.Black => Color.White,
                _ => Color.None
            };
        }

        private bool AnyMovePossible(Color kleur)
        {
            if (kleur == Color.None)
                throw new Exception("Kleur mag niet gelijk aan Geen zijn!");
            // controleeer of er een zet mogelijk is voor kleur
            for (int rijZet = 0; rijZet < BoardSize; rijZet++)
            {
                for (int kolomZet = 0; kolomZet < BoardSize; kolomZet++)
                {
                    if (MovePossible(rijZet, kolomZet, kleur))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool MovePossible(int rijZet, int kolomZet, Color kleur)
        {
            // Check of er een richting is waarin een zet mogelijk is. Als dat zo is, return dan true.
            for (int i = 0; i < 8; i++)
            {
                {
                    if (PiecesReversible(rijZet, kolomZet,
                                                             kleur,
                                                             Directions[i, 0], Directions[i, 1]))
                        return true;
                }
            }
            return false;
        }

        private void SwapTurn()
        {
            if (PlayerTurn == Color.White)
                PlayerTurn = Color.Black;
            else
                PlayerTurn = Color.White;
        }

        private static bool WithinBorders(int rij, int kolom)
        {
            return (rij >= 0 && rij < BoardSize &&
                    kolom >= 0 && kolom < BoardSize);
        }

        private bool WithinBordersAndFree(int rijZet, int kolomZet)
        {
            // Als op het bord gezet wordt, en veld nog vrij, dan return true, anders false
            return (WithinBorders(rijZet, kolomZet) && Board[rijZet, kolomZet] == Color.None);
        }

        private bool PiecesReversible(int rijZet, int kolomZet,
                                                          Color kleurZetter,
                                                          int rijRichting, int kolomRichting)
        {
            int rij, kolom;
            Color kleurTegenstander = OppositeColor(kleurZetter);
            if (!WithinBordersAndFree(rijZet, kolomZet))
                return false;

            // Zet rij en kolom op de index voor het eerst vakje naast de zet.
            rij = rijZet + rijRichting;
            kolom = kolomZet + kolomRichting;

            int aantalNaastGelegenStenenVanTegenstander = 0;
            // Zolang Bord[rij,kolom] niet buiten de bordgrenzen ligt, en je in het volgende vakje 
            // steeds de kleur van de tegenstander treft, ga je nog een vakje verder kijken.
            // Bord[rij, kolom] ligt uiteindelijk buiten de bordgrenzen, of heeft niet meer de
            // de kleur van de tegenstander.
            // N.b.: deel achter && wordt alleen uitgevoerd als conditie daarvoor true is.
            while (WithinBorders(rij, kolom) && Board[rij, kolom] == kleurTegenstander)
            {
                rij += rijRichting;
                kolom += kolomRichting;
                aantalNaastGelegenStenenVanTegenstander++;
            }

            // Nu kijk je hoe je geeindigt bent met bovenstaande loop. Alleen
            // als alle drie onderstaande condities waar zijn, zijn er in de
            // opgegeven richting stenen in te sluiten.
            return (WithinBorders(rij, kolom) &&
                    Board[rij, kolom] == kleurZetter &&
                    aantalNaastGelegenStenenVanTegenstander > 0);
        }

        private bool ReversePieces(int rijZet, int kolomZet,
                                                                                     Color kleurZetter,
                                                                                     int rijRichting, int kolomRichting)
        {
            int rij, kolom;
            Color kleurTegenstander = OppositeColor(kleurZetter);
            bool stenenOmgedraaid = false;

            if (PiecesReversible(rijZet, kolomZet, kleurZetter, rijRichting, kolomRichting))
            {
                rij = rijZet + rijRichting;
                kolom = kolomZet + kolomRichting;

                // N.b.: je weet zeker dat je niet buiten het bord belandt,
                // omdat de stenen van de tegenstander ingesloten zijn door
                // een steen van degene die de zet doet.
                while (Board[rij, kolom] == kleurTegenstander)
                {
                    Board[rij, kolom] = kleurZetter;
                    rij += rijRichting;
                    kolom += kolomRichting;
                }
                stenenOmgedraaid = true;
            }
            return stenenOmgedraaid;
        }

        public void GiveUp()
        {
            


        }
    }
}
