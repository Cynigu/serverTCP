//using Assets.conteiner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverTCP
{
    public enum Direction
    {
        LEFT,
        RIGHT,
        TOP, 
        BOTTOM
    }

    public class LogicGame // желтый, красный, синий
    {
        private const int dimenshionTable = 5;

        private static ITable CreatNullTable()
        {
            ITable table = new BaseTableForGame(dimenshionTable); // таблица 5х5

            // поля 5*5 клеток, где каждая клетка в нечетном столбце и четной строке - блокирована, остальные клетки пустые
            for (int i = 0, z = 0; i < table.CountRow; i++) // строки
            {

                for (int j = 0; j < table.CountColumnes; j++, z++) // столбцы
                {
                    ICell cell = new BaseCell();

                    // Debug.Log(" Cells " + z);

                    table.Cells.Add(cell);


                    cell.NumberPose = z;

                    if (j % 2 != 0 && i % 2 == 0)
                    {
                        cell.Status = StatusImg.BLOCK;
                    }
                }
            }

            return table;
        }

        // Проверка конца игры, если true - вы выиграли, если false - вы проиграли
        public static bool AreYouWinner(ITable table)
        {

            for (int i = 0, z = 0; i < table.CountRow; i++) // строки
            {

                for (int j = 0; j < table.CountColumnes; j++, z++) // столбцы
                {
                    ICell cell = table.Cells[z];

                    if (j == 0 && cell.Status != StatusImg.YELLOW)
                    {
                        return false;
                    }
                    else if (j == 2 && cell.Status != StatusImg.RED)
                    {
                        return false;
                    }
                    else if (j == 4 && cell.Status != StatusImg.BLUE)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Реализация шага, возвращает статус выбранной ячейки
        // direction - в какую сторону сдвинуть блок; Table - таблица до шага,  NumSelecteCell - выбранная ячейка
        public static ITable StepImp(in Direction _direction,in ITable _table,in int _NumSelecteCell)
        {
            if (_NumSelecteCell >= 25 || _NumSelecteCell < 0)
                throw new ArgumentException();

            ITable table = _table;

            ICell selectCell = table.Cells[_NumSelecteCell];

            ICell nextCell;

            // Если нажать стрелку влево и шаг не выходит за границы таблицы 5х5
            if ((_direction == Direction.LEFT) && _NumSelecteCell % 5 != 0 && (_NumSelecteCell - 1) >= 0) 
            {
                nextCell = table.Cells[_NumSelecteCell - 1];
                if (nextCell.Status == (int)StatusImg.BLANK) // Если след поле пустое
                {
                    nextCell.Status = selectCell.Status;
                    selectCell.Status = (int)StatusImg.BLANK;
                    return table;
                }
                else throw new Exception("Поле куда вы пытаетесь пойти занято");
            }

            if ((_direction == Direction.RIGHT) && (_NumSelecteCell + 1) % 5 != 0 && (_NumSelecteCell + 1) <= 24) // Если нажат стрелку вправо
            {
                nextCell = table.Cells[_NumSelecteCell + 1];
                if (nextCell.Status == (int)StatusImg.BLANK) // Если след поле пустое
                {
                    nextCell.Status = selectCell.Status;
                    selectCell.Status = (int)StatusImg.BLANK;
                    return table;
                }
                else throw new Exception("Поле куда вы пытаетесь пойти занято");
            }

            if ((_direction == Direction.TOP) && (_NumSelecteCell - 5) >= 0) // Если нажат стрелку вверх
            {
                nextCell = table.Cells[_NumSelecteCell - 5];
                if (nextCell.Status == (int)StatusImg.BLANK) // Если след поле пустое
                {
                    nextCell.Status = selectCell.Status;
                    selectCell.Status = (int)StatusImg.BLANK;
                    return table;
                }
                else throw new Exception("Поле куда вы пытаетесь пойти занято");
            }

            if ((_direction == Direction.BOTTOM) && (_NumSelecteCell + 5) <= 24) // Если нажат стрелку вниз
            {
                nextCell = table.Cells[_NumSelecteCell + 5];
                if (nextCell.Status == (int)StatusImg.BLANK) // Если след поле пустое
                {
                    nextCell.Status = selectCell.Status;
                    selectCell.Status = (int)StatusImg.BLANK;
                    return table;
                }
                else throw new Exception("Поле куда вы пытаетесь пойти занято");
            }

            throw new Exception("Вы не можете выйти за границу таблицы");
        }

        // Создает стартовую таблицу для игры
        public static ITable CreateBaseTable()
        {
            ITable table = CreatNullTable();

            // Заполняем рандомными цветными полями каждый четный столбец, так чтобы каждого цвета было по 5 (всего 15)
            int countYellow = 0;
            int countRed = 0;
            int countBlue = 0;

            int colorColumn = 0;

            Random rnd = new Random();
            while (colorColumn < table.CountColumnes)
            {
                for (int j = colorColumn; j < table.CountPoles; j += 5)
                {
                    ICell cell = table.Cells[j];
                    bool temp = true;

                    while (temp)
                    {
                        int rndc = rnd.Next(1, 4);
                        StatusImg randColor = (StatusImg)rndc;

                        if (randColor == StatusImg.YELLOW && countYellow != table.CountOneColorPoles)
                        {
                            countYellow++;
                            cell.Status = randColor;
                            temp = false;
                        }
                        else if (randColor == StatusImg.RED && countRed != table.CountOneColorPoles)
                        {
                            countRed++;
                            cell.Status = randColor;
                            temp = false;
                        }
                        else if (randColor == StatusImg.BLUE && countBlue != table.CountOneColorPoles)
                        {
                            countBlue++;
                            cell.Status = randColor;
                            temp = false;
                        }
                    }

                }
                colorColumn += 2;
            }
            return table;
        }

        // Создаёт таблицу выигрышную
        public static ITable CreateWinTable()
        {
            ITable table = CreatNullTable();

            for (int i=0; i < table.CountPoles; i++)
            {
                if (i%5 == 0)
                    table.Cells[i].Status = StatusImg.YELLOW;
                else if ((i+3)%5==0)
                    table.Cells[i].Status = StatusImg.RED;
                else if((i+1)%5 == 0)
                    table.Cells[i].Status = StatusImg.BLUE;
            }
            return table;
        }
    }
}
