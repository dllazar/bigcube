using RubiksCube;

namespace LEDMatrixCube
{
    internal class CenterPieceSwapCommutator
    {
        public static void GenerateMoveCommands(Cube cube, uint x, uint y, CommandQueue commandQueue) 
        {
            var cubeSize = cube.GetSize();
            if (x == 0 || y == 0 || x == cubeSize - 1 || y == cubeSize - 1)
            {
                return; //can't use this commutator on edge pieces
            }
            if (cubeSize %2 > 0 && x == cubeSize/2 + 1 && y == cubeSize / 2 + 1) 
            {
                return; //can't commute center pieces on odd-order cubes
            }

            if (x >= cubeSize / 2)
            {
                commandQueue.Enqueue($"{x + 1}L'");
                commandQueue.Enqueue($"U'");
                commandQueue.Enqueue($"{y + 1}L'");
                commandQueue.Enqueue($"U");
                commandQueue.Enqueue($"{x + 1}L");
                commandQueue.Enqueue($"U'");
                commandQueue.Enqueue($"{y + 1}L");
                commandQueue.Enqueue($"U");
            }
            else
            {
                commandQueue.Enqueue($"{x + 1}L'");
                commandQueue.Enqueue($"U");
                commandQueue.Enqueue($"{y + 1}R");
                commandQueue.Enqueue($"U'");
                commandQueue.Enqueue($"{x + 1}L");
                commandQueue.Enqueue($"U");
                commandQueue.Enqueue($"{y + 1}R'");
                commandQueue.Enqueue($"U'");
            }
        }

    }
}
