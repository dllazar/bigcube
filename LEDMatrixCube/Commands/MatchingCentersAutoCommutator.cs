
using RubiksCube;

namespace LEDMatrixCube.Commands
{
    internal class MatchingCentersAutoCommutator
    {
        public static void GenerateMoveCommands(Cube cube, CubeCursor cursor, CommandQueue commandQueue, int limitNumCommutatorExecutions=1)
        {
            var frontFaceState = cube.GetFace(Face.F).Replace("\n", "").Replace("\r", "");
            var upFaceState = cube.GetFace(Face.U).Replace("\n", "").Replace("\r", "");
            var frontFaceColorUnderCursor = frontFaceState[(int)((cursor.CursorY * cube.GetSize()) + cursor.CursorX)];
            Console.WriteLine($"color under cursor: {frontFaceColorUnderCursor}");
            var commutatorExecutionCount = 0;
            for (uint y = 1; y< cube.GetSize()-2; y++) 
            {
                if (limitNumCommutatorExecutions > 0 && commutatorExecutionCount >= limitNumCommutatorExecutions) break;
                for (uint x = 1; x< cube.GetSize()-2; x++)
                {
                    if (limitNumCommutatorExecutions > 0 && commutatorExecutionCount >= limitNumCommutatorExecutions) break;
                    var upFaceColorHere = upFaceState[(int)((y * cube.GetSize()) + x)];
                    var frontFaceColorHere = frontFaceState[(int)((y * cube.GetSize()) + x)];
                    Console.WriteLine($"evaluating {x},{y}: Front face color:{frontFaceColorHere}, Up face color:{upFaceColorHere}");
                    if (upFaceColorHere != frontFaceColorHere && frontFaceColorHere == frontFaceColorUnderCursor)
                    {
                        Console.WriteLine($"autocom generated for {x}, {y}");
                        CenterPieceSwapCommutator.GenerateMoveCommands(cube, x, y, commandQueue);
                        commandQueue.Enqueue("render");
                        commutatorExecutionCount++;
                    }
                }
            }
        }
    }
}
