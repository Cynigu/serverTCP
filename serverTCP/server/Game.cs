using System;
using System.Collections.Generic;
using System.Text;

namespace serverTCP
{
    public class Game
    {
        private ITable table;
        public int WinCount { get; set; }
        public int CountGame{ get; set; }
        public int CountGamers { get; set; }
        public int NumberGamerForThisStep { get; set; }

        private string rool1 = "Правила: \nДано поле 5*5 клеток и 15 фишек трех цветов, по пять каждого цвета.\n" +
                    "Каждая клетка поля может быть либо блокирована, либо занята одной фишкой любого цвета, либо свободна.\n" +
                    "На поле выставлены все фишки, 6 клеток блокированы и 4 клетки свободны.\n" +
                    "Блокированные клетки остаются таковыми всегда.Фишки мы можем передвигать на \n" +
                    "соседнее свободное место по горизонтали или вертикали. Требуется, передвигая фишки,\n" +
                    "выставить их в три вертикальных ряда соответственно цветам, стоящим над полем.\n";
        private string rool2 = "\nЧтобы сделать ход нужно написать номер ячеки (от 0 до 24 (включая))\nи напрвление движения куда двинуть ячейку " +
                    "(влево - л, вправо - п, вверх - в, вниз - н) через пробел. \nПример: \"2 л\" \n";
        public string Rool1 { get => rool1;  }
        public string Rool2 { get => rool2; }

        public Game()
        {
            CountGame++;
            table = LogicGame.CreateBaseTable();
            NumberGamerForThisStep = -1;
            CountGamers = 0;
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
            NextClient();
        }

        private void CheckCorrectNumberStep()
        {
            if (NumberGamerForThisStep >= CountGamers && CountGamers != 0)
            {
                NumberGamerForThisStep = 0;
            }
            else if(CountGamers == 0)
            {
                NumberGamerForThisStep = -1;
            }
        }

        public bool IsMyStep(int myNum)
        {
            if (myNum == NumberGamerForThisStep)
            {
                return true;
            }

            CheckCorrectNumberStep();
            return false;
        }

        private void NextClient()
        {
            NumberGamerForThisStep++;
            CheckCorrectNumberStep();
        }
        protected internal void SetCountClients(int count)
        {
            CountGamers = count;
            if (CountGamers == 1)
            {
                NumberGamerForThisStep = 0;
            }
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