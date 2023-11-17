using System.Collections.Concurrent;

namespace LEDMatrixCube
{
    internal class CommandQueue:ConcurrentQueue<string>{}
}
