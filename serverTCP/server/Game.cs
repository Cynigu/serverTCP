using System;
using System.Collections.Generic;
using System.Text;

namespace serverTCP
{
    public class Game
    {
        private ITable table;
        static public int WinCount { get; set; }
        static public int CountGame{ get; set; }
        public Game()
        {
            CountGame++;
            table = LogicGame.CreateBaseTable();
        }

        public void SetTableStep(int numCell, Direction dr)
        {
            if (numCell >= table.Cells.Count || numCell < 0)
            {
                throw new ArgumentException();
            }

            if (table.Cells[numCell].Status == StatusImg.BLANK || table.Cells[numCell].Status == StatusImg.BLOCK)
            {
                throw new ArgumentException();
            }

            table = LogicGame.StepImp(dr, table, numCell);
        }

        public void StartNewGame()
        {
            CountGame++;
            table = LogicGame.CreateBaseTable();
        }

        public string GetTableStr()
        {
            string str = "1   2   3\n";
            for (int i=0, c =0; i<table.CountRow; i++)
            {
                for (int j=0; j < table.CountColumnes; j++, c++)
                {
                    int st = (int)table.Cells[c].Status;
                    str +=  st + " ";
                }
                str += "\n";
            }
            return str;
        }

        public bool TheWinGame()
        {
            bool win = LogicGame.AreYouWinner(table);
            if (win) WinCount++;
            return win;
        }
    }
}