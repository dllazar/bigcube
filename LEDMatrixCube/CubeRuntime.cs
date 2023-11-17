using RubiksCube.UI;
using RubiksCube;
using LEDMatrixCube.Commands;

namespace LEDMatrixCube
{
    internal class CubeRuntime : IDisposable
    {
        private bool _isRunning = true;
        private Cube _cube = new Cube(size: 64);
        private CommandQueue _commandQueue = new CommandQueue();
        private CubeCursor _cubeCursor;
        private GamepadInput _gamePadInput;
        private Renderer _renderer;
        private bool _statsVisible = false;
        private bool _needsSave = false;
        private bool _needsRender = false;
        private uint _lastCursorX;
        private uint _lastCursorY;
        private bool _lastCursorIsHorizontal;
        private bool _lastCursorIsDot;

        private bool _disposed;

        private void Initialize()
        {
            _cube = new Cube(size: 64);
            _commandQueue = new CommandQueue();
            _cubeCursor = new CubeCursor(_cube, _commandQueue);
            _gamePadInput = new GamepadInput(_cubeCursor, _commandQueue);
            _statsVisible = false;
            _renderer = new Renderer(_cubeCursor);
        }

        public void Start()
        {
            Initialize();
            LoadCubeState();
            Render();

            RememberCursorStateForLater();
            while (_isRunning)
            {
                RefreshGamepadInputs();
                ProcessCommandsInQueue();
                CheckIfRenderNeededDueToCursorPositionChange();
                RenderIfNeeded();
                SaveCubeStateIfUnsavedMovesHaveBeenMade();
                RememberCursorStateForLater();
                YieldCPUTimeToOthers();
            }
        }

        private static void YieldCPUTimeToOthers()
        {
            Thread.Sleep(20);
        }

        private void RefreshGamepadInputs()
        {
            _gamePadInput.RefreshInputs();
        }

        private void SaveCubeStateIfUnsavedMovesHaveBeenMade()
        {
            if (_needsSave)
            {
                try
                {
                    CubePersistence.SaveCubeState(_cube);
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
                _needsSave = false;
            }
        }

        private void RenderIfNeeded()
        {
            if (_needsRender)
            {
                _renderer.Render(_statsVisible);
                _needsRender = false;
            }
        }

        private void CheckIfRenderNeededDueToCursorPositionChange()
        {
            if (_cubeCursor.CursorX != _lastCursorX || _cubeCursor.CursorY != _lastCursorY || _cubeCursor.CursorIsHorizontal != _lastCursorIsHorizontal || _cubeCursor.CursorIsDot != _lastCursorIsDot)
            {
                _needsRender = true;
            }
        }

        private void RememberCursorStateForLater()
        {
            _lastCursorX = _cubeCursor.CursorX;
            _lastCursorY = _cubeCursor.CursorY;
            _lastCursorIsHorizontal = _cubeCursor.CursorIsHorizontal;
            _lastCursorIsDot = _cubeCursor.CursorIsDot;
        }

        private void ProcessCommandsInQueue()
        {
            while (_commandQueue.TryDequeue(out var command))
            {
                if (command.ToLowerInvariant().StartsWith("render"))
                {
                    Render();
                }
                else if (command.ToLowerInvariant().StartsWith("scramble"))
                {
                    Scramble();
                }
                else if (command.ToLowerInvariant().StartsWith("reset"))
                {
                    ResetCube();
                }
                else if (command.ToLowerInvariant().StartsWith("undo"))
                {
                    UndoMove();
                }
                else if (command.ToLowerInvariant().StartsWith("toggle-stats"))
                {
                    ToggleStatsVisibility();
                }
                else if (command.ToLowerInvariant().StartsWith("piece-swap-commutator"))
                {
                    SwapFrontAndTopPiecesUnderCursor();
                }
                else if (command.ToLowerInvariant().StartsWith("matching-centers-auto-commutator"))
                {
                    BulkAutocommuteCentersMatchingCursorColorFromFrontFaceToTopFace();
                }
                else
                {
                    ExecuteCubeMove(command);
                }
                _needsRender = true;
            }
        }

        private void ExecuteCubeMove(string command)
        {
            _cube.Move(command);
            _needsSave = true;
        }
        private void BulkAutocommuteCentersMatchingCursorColorFromFrontFaceToTopFace()
        {
            MatchingCentersAutoCommutator.GenerateMoveCommands(_cube, _cubeCursor, _commandQueue, limitNumCommutatorExecutions: 1);
        }
        private void SwapFrontAndTopPiecesUnderCursor()
        {
            CenterPieceSwapCommutator.GenerateMoveCommands(_cube, _cubeCursor.CursorX, _cubeCursor.CursorY, _commandQueue);
        }

        private void ToggleStatsVisibility()
        {
            _statsVisible = !_statsVisible;
        }

        private void UndoMove()
        {
            _cube.UndoMove();
            _needsSave = true;
        }

        private void ResetCube()
        {
            _cube.Reset();
            _needsSave = true;
        }

        private void Scramble()
        {
            var nextScrambleMove = _cube.GenerateScramble(1);
            _cube.Move(nextScrambleMove);
            _needsSave = true;
        }

        private void Render()
        {
            _renderer.Render();
        }

        private void LoadCubeState()
        {
            try
            {
                CubePersistence.LoadIfSaveFileExists(_cube);
            }
            catch { }
        }
        public void Stop()
        {
            _isRunning = false;
        }
        public bool IsRunning { get { return _isRunning; } }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try { _renderer.Dispose(); } catch { }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
