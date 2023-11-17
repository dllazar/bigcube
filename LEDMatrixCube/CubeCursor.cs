using RubiksCube;

namespace LEDMatrixCube
{
    internal class CubeCursor
    {
        public uint CursorX { get; private set; } = 0;
        public uint CursorY { get; private set; } = 0;
        public bool CursorIsHorizontal { get; set; } = true;
        public bool CursorIsDot { get; set; } = false;
        private readonly CommandQueue _commandQueue;
        public Cube Cube { get; private set; }

        public CubeCursor(Cube cube, CommandQueue commandQueue)
        {
            Cube = cube;
            _commandQueue = commandQueue;
            CursorX = CursorY = cube.GetSize() / 2;
        }
        public void CursorTowardLeftFace()
        {
            CursorX = CursorX >0 ? CursorX -1: CursorX;
        }
        public void CursorTowardRightFace()
        {
            CursorX = CursorX < Cube.GetSize() -1? CursorX + 1: CursorX;
        }
        public void CursorTowardUpFace()
        {
            CursorY = CursorY > 0 ? CursorY - 1 : CursorY;
        }
        public void CursorTowardDownFace()
        {
            CursorY = CursorY < Cube.GetSize() - 1 ? CursorY + 1 : CursorY;
        }

        public void MoveSelectedRow(bool clockwise=true)
        {
            string move;
            if (CursorY ==0)
            {
                move = "U";
            }
            else if (CursorY < Cube.GetSize() - 1)
            {
                move = $"{CursorY + 1}U";
            }
            else
            {
                move = $"D";
                clockwise = !clockwise;
            }
            if (!clockwise) move += "'";
            _commandQueue.Enqueue(move);
        }
        public void MoveSelectedCol(bool clockwise=true)
        {
            string move;
            if (CursorX ==0)
            {
                move = "L";
            }
            else if (CursorX < Cube.GetSize() - 1)
            {
                move = $"{CursorX + 1}L";
            }
            else
            {
                move = "R";
                clockwise = !clockwise;
            }
            if (!clockwise) move += "'";
            _commandQueue.Enqueue(move);
        }

    }
}
