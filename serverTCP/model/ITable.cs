using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverTCP
{
    public interface ITable
    {
        public List<ICell> Cells { get; set; }

        public int CountPoles { get; }
        public int CountColumnes { get; }

        public int CountRow { get; }
        public int CountColors { get; }
        public int CountOneColorPoles { get; }
    }
}
