using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverTCP
{
    public interface ICell
    {
        public StatusImg Status { get; set; }
        public int NumberPose { get; set; }
    }
}
