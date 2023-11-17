using LEDMatrixCube;
using LEDMatrixCube.Overlays;
using RPiRgbLEDMatrix;

namespace RubiksCube.UI
{
    internal class Renderer:IDisposable
    {
        private const float SELECTED_CUBIE_BRIGHTNESS_MULTIPLIER = 0.5f;
        private readonly RGBLedMatrix _matrix;
        private readonly RGBLedCanvas _canvas;
        private bool disposed;

        private readonly CubeCursor _cubeCursor;
        private readonly StatsOverlay _statsOverlay;
        public Renderer(CubeCursor cubeCursor)
        {
            _cubeCursor = cubeCursor;
            var options = new RGBLedMatrixOptions() { 
                Brightness = 30, 
                ChainLength = 6, 
                Rows = 64, 
                Cols = 64, 
                GpioSlowdown = 4, 
                PwmBits = 6,    
                PwmLsbNanoseconds = 130,
                ShowRefreshRate = false, 
                HardwareMapping = "adafruit-hat-pwm"
            };
            _matrix = new RGBLedMatrix(options);
            _canvas = _matrix.CreateOffscreenCanvas();
            _statsOverlay = new StatsOverlay();
        }
        public void Render(bool showStatsOverlay = false)
        {
            _canvas.Clear();
            var cubeSize = _cubeCursor.Cube.GetSize();
            foreach (var face in Enum.GetValues<Face>())
            {
                var faceXOffset = GetFaceXOffset(face);
                
                var faceState = _cubeCursor.Cube.GetFace(face);
                var rowStates = faceState.Split("\n");

                for (uint cubieY = 0; cubieY< cubeSize; cubieY++)
                {
                    var rowState = rowStates[cubieY];
                    for (uint cubieX = 0; cubieX < cubeSize; cubieX++)
                    {
                        var thisCubieChar = rowState[(int)cubieX];
                        var color = GetLEDColor(thisCubieChar);
                        if (IsSelected(face, cubieX, cubieY))
                        {
                            color.R = (byte)Math.Min(color.R * SELECTED_CUBIE_BRIGHTNESS_MULTIPLIER, byte.MaxValue);
                            color.G = (byte)Math.Min(color.G * SELECTED_CUBIE_BRIGHTNESS_MULTIPLIER, byte.MaxValue);
                            color.B = (byte)Math.Min(color.B * SELECTED_CUBIE_BRIGHTNESS_MULTIPLIER, byte.MaxValue);
                        }
                        ApplyPixelRotation(cubeSize:cubeSize, face: face, cubieX: cubieX, cubieY: cubieY, pixelX: out uint pixelX, pixelY: out uint pixelY);
                        _canvas.SetPixel((int)(pixelX + faceXOffset), (int)pixelY, color);
                    }
                }
            }
            if (showStatsOverlay)
            {
                _statsOverlay.Render(_canvas, GetFaceXOffset(Face.F));
            }
            _matrix.SwapOnVsync(_canvas);
        }
        private static void ApplyPixelRotation(uint cubeSize, Face face, uint cubieX, uint cubieY, out uint pixelX, out uint pixelY)
        {
            pixelX = cubieX;
            pixelY = cubieY;
            switch (face)
            {
                case Face.U:
                    pixelX = cubeSize - 1 - cubieY;
                    pixelY = cubieX;
                    break;
                case Face.L:
                    pixelX = cubieY;
                    pixelY = cubeSize - 1 - cubieX;
                    break;
                case Face.F:
                    break;
                case Face.R:
                    pixelX = cubeSize - 1 - cubieY;
                    pixelY = cubieX;
                    break;
                case Face.B:
                    break;
                case Face.D:
                    pixelX = cubeSize - 1 - cubieY;
                    pixelY = cubieX;
                    break;
            }
        }
        private bool IsSelected(Face face, uint x, uint y)
        {
            switch (face)
            {
                case Face.U:
                    return
                        (_cubeCursor.CursorIsDot && x == _cubeCursor.CursorX && y == _cubeCursor.CursorY)
                            ||
                        (!_cubeCursor.CursorIsDot && !_cubeCursor.CursorIsHorizontal && x == _cubeCursor.CursorX);
                case Face.L:
                    return !_cubeCursor.CursorIsDot && _cubeCursor.CursorIsHorizontal && y == _cubeCursor.CursorY;
                case Face.F:
                    return
                        (_cubeCursor.CursorIsDot && x == _cubeCursor.CursorX && y == _cubeCursor.CursorY) 
                            ||
                        (!_cubeCursor.CursorIsDot && !_cubeCursor.CursorIsHorizontal && x == _cubeCursor.CursorX) || (!_cubeCursor.CursorIsDot && _cubeCursor.CursorIsHorizontal && y == _cubeCursor.CursorY);
                case Face.R:
                    return !_cubeCursor.CursorIsDot && _cubeCursor.CursorIsHorizontal && y == _cubeCursor.CursorY;
                case Face.B:
                    return !_cubeCursor.CursorIsDot &&
                        (!_cubeCursor.CursorIsHorizontal && (x == _cubeCursor.Cube.GetSize()-1 - _cubeCursor.CursorX) || (_cubeCursor.CursorIsHorizontal && y == _cubeCursor.CursorY));
                case Face.D:
                    return !_cubeCursor.CursorIsDot && !_cubeCursor.CursorIsHorizontal && x == _cubeCursor.CursorX;
            }
            return false;
        }
        private static uint GetFaceXOffset(Face face)
        {
            switch (face)
            {
                case Face.U: return 0;
                case Face.L: return 64;
                case Face.F: return 128;
                case Face.R: return 192;
                case Face.B: return 256;
                case Face.D: return 320;
            }
            return 0;
        }
        private static Color GetLEDColor(char color)
        {
            switch (color)
            {
                case 'R':
                    return Colors.Red;
                case 'G':
                    return Colors.Green;
                case 'B':
                    return Colors.Blue;
                case 'Y':
                    return Colors.Yellow;
                case 'O':
                    return Colors.Orange;
                case 'W':
                    return Colors.White;
            }
            return Colors.Black;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    try { _matrix.Dispose(); } catch { }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
