using System.IO;

namespace BattleShipBrain
{
    public struct BoardSquareState
    {
        public bool IsShip { get ; set; }
        public bool IsBomb { get; set; }

        public void Bombing()
        {
            IsBomb = true;
        }

        public override string ToString()
        {
            switch (IsShip, IsBomb)
            {
                case (false,false):
                    return "|  ~  ";
                case (true,false):
                    return "|  #  ";
                case (false,true):
                    return "|  o  ";
                case (true,true):
                    return "|  X  ";
            }
        }
    }
}