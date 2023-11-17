using LEDMatrixCube;

namespace RubiksCube
{
    public class Program
    {
        public static void Main(String[] args)
        {
            using var runtime = new CubeRuntime();
            Console.CancelKeyPress += (_, e) =>
            {
                runtime.Stop();
                e.Cancel = true; // do not terminate program with Ctrl+C, we need to dispose
            };

            runtime.Start();
            while (runtime.IsRunning)
            {
                Thread.Sleep(20);
            }
        }
    }
}