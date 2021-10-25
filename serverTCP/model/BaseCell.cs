using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverTCP
{
    public enum StatusImg
    {
        BLANK,
        YELLOW,
        RED,
        BLUE,
        BLOCK
    }

    public class BaseCell: ICell
    {
        
        #region Fields
        private StatusImg status; // статус ячейки

        private int numberPose; // номер поля в осн таблице
        #endregion

        #region Properties 
        public StatusImg Status
        {
            get { return status; }
            set { status = value; }
        }

        public int NumberPose
        {
            get { return numberPose; }
            set { numberPose = value; }
        }
        #endregion

        public BaseCell(int _numberPose = 0, StatusImg _status = StatusImg.BLANK)
        {
            Status = _status;
            NumberPose = _numberPose;
        }

    }
}
