//using Assets.conteiner;
//using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverTCP
{
    public class BaseTableForGame: ITable
    {
        //readonly IContainer _con;

        // постоянные переменные количество 
        readonly int _countPoles;
        readonly int _countColumnes;
        readonly int _countRow;
        readonly int _countColors = 3;
        readonly int _countOneColorPoles;

        #region fields
        private List<ICell> cells;
        #endregion

        #region Properties
        public List<ICell> Cells 
        {
            get { return cells; }
            set { cells = value; }
        }

        public int CountPoles { get {return _countPoles; } }
        public int CountColumnes { get { return _countColumnes; } }
        public int CountRow { get { return _countRow; } }
        public int CountColors { get { return _countColors; } }
        public int CountOneColorPoles { get { return _countOneColorPoles; } }
        #endregion

        public BaseTableForGame(int countColumnes)
        {
            if (countColumnes <= 0)
            {
                throw new ArgumentException();
            }
            _countPoles = countColumnes* countColumnes;
            _countColumnes = countColumnes;
            _countRow = countColumnes;
            _countOneColorPoles = countColumnes;
            _countColors = 3;

            Cells = new List<ICell>();

            //var build = MyContainer.ContainerBaseTable();
            //_con = build.Build();

            //CreateBaseTable();
        }

       

    }
}
