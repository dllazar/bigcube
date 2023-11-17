using Gamepad;
using LEDMatrixCube.Gamepad;

namespace LEDMatrixCube
{
    internal class GamepadInput:IDisposable
    {
        private const uint MIN_TIME_TIL_FIRST_REPEAT_MS = 500;
        private const uint MIN_TIME_BETWEEN_REPEATS_MS = 20;
        private GamepadController _gamepadController;
        private readonly CubeCursor _cubeCursor;
        private readonly CommandQueue _commandQueue;
        private bool _disposed;
        private readonly AxisState[] _axisStates=new AxisState[8];
        private readonly ButtonState[] _buttonStates = new ButtonState[15];
        private bool _altFunctionButtonIsPressed = false;
        private GamepadInput() { }
        public GamepadInput(CubeCursor cubeCursor, CommandQueue commandQueue):this()
        {
            _cubeCursor = cubeCursor;
            _commandQueue = commandQueue;
            RegisterForGamepadEvents();
        }

        private void RegisterForGamepadEvents()
        {
            DisposeGamepad();
            _gamepadController = new GamepadController();
            RefreshInputs();

            _gamepadController.ButtonChanged += (object sender, ButtonEventArgs e) =>
            {
                RefreshInputs();
            };

            _gamepadController.AxisChanged += (object sender, AxisEventArgs e) =>
            {
                RefreshInputs();
            };
        }
        private void ProcessAxis(AxisState priorAxisState, AxisState currentAxisState)
        {
            if (currentAxisState == null) return;
            if (currentAxisState.IsPressed) 
            {
                if (priorAxisState !=null && priorAxisState.IsPressed)
                {
                    if (!currentAxisState.AutoRepeat || (currentAxisState.NextAutoRepeatTime.HasValue && DateTime.Now.CompareTo(currentAxisState.NextAutoRepeatTime.Value) < 0))
                    {
                        return;
                    }
                }

                switch (currentAxisState.Axis)
                {
                    case (byte)XBoxWirelessControllerAxis.LeftStickLeftRight: 
                        if (currentAxisState.Value <0)
                        {
                            _commandQueue.Enqueue("B"); ;
                        }
                        else
                        {
                            _commandQueue.Enqueue("B'"); ;
                        }
                        break;
                    case (byte)XBoxWirelessControllerAxis.LeftStickUpDown: 
                        if (currentAxisState.Value < 0)
                        {
                            _commandQueue.Enqueue("L'"); ;
                        }
                        else
                        {
                            _commandQueue.Enqueue("L"); ;
                        }
                        break;
                    case (byte)XBoxWirelessControllerAxis.RightStickLeftRight:
                        if (currentAxisState.Value < 0)
                        {
                            _commandQueue.Enqueue("F'"); ;
                        }
                        else
                        {
                            _commandQueue.Enqueue("F"); ;
                        }
                        break;
                    case (byte)XBoxWirelessControllerAxis.RightStickUpDown:
                        if (currentAxisState.Value < 0)
                        {
                            _commandQueue.Enqueue("R"); ;
                        }
                        else
                        {
                            _commandQueue.Enqueue("R'"); ;
                        }
                        break;
                    case (byte)XBoxWirelessControllerAxis.RightTrigger:
                        _commandQueue.Enqueue("D'");
                        break;
                    case (byte)XBoxWirelessControllerAxis.LeftTrigger:
                        _commandQueue.Enqueue("D");
                        break;
                    case (byte)XBoxWirelessControllerAxis.DPadLeftRight: 
                        if (_altFunctionButtonIsPressed)
                        {
                            if (currentAxisState.Value < 0)
                            {
                                _commandQueue.Enqueue("Y");
                            }
                            else
                            {
                                _commandQueue.Enqueue("Y'");
                            }
                        }
                        else
                        {
                            if (_cubeCursor.CursorIsDot || !_cubeCursor.CursorIsHorizontal)
                            {
                                if (currentAxisState.Value < 0)
                                {
                                    _cubeCursor.CursorTowardLeftFace();
                                }
                                else
                                {
                                    _cubeCursor.CursorTowardRightFace();
                                }
                            }
                            else if (!_cubeCursor.CursorIsDot && _cubeCursor.CursorIsHorizontal)
                            {
                                currentAxisState.InhibitAutoRepeatUntilReleased = true;
                                _cubeCursor.MoveSelectedRow(clockwise: currentAxisState.Value < 0);
                            }
                        }
                        break;
                    case (byte)XBoxWirelessControllerAxis.DPadUpDown: 
                        if (_altFunctionButtonIsPressed)
                        {
                            if (currentAxisState.Value < 0)
                            {
                                _commandQueue.Enqueue("X");
                            }
                            else
                            {
                                _commandQueue.Enqueue("X'");
                            }
                        }
                        else
                        {
                            if (_cubeCursor.CursorIsHorizontal || _cubeCursor.CursorIsDot)
                            {
                                if (currentAxisState.Value < 0)
                                {
                                    _cubeCursor.CursorTowardUpFace();
                                }
                                else
                                {
                                    _cubeCursor.CursorTowardDownFace();
                                }
                            }
                            else
                            {
                                _cubeCursor.MoveSelectedCol(clockwise: !(currentAxisState.Value < 0));
                                currentAxisState.InhibitAutoRepeatUntilReleased = true;
                            }
                        }
                        break;
                }

                if (priorAxisState != null && priorAxisState.NextAutoRepeatTime.HasValue)
                {
                    currentAxisState.NextAutoRepeatTime = DateTime.Now.AddMilliseconds(MIN_TIME_BETWEEN_REPEATS_MS);
                }
                else
                {
                    currentAxisState.NextAutoRepeatTime = DateTime.Now.AddMilliseconds(MIN_TIME_TIL_FIRST_REPEAT_MS);
                }

            }
            else
            {
                currentAxisState.NextAutoRepeatTime = null;
            }
        }

        private void ProcessButton(ButtonState priorButtonState, ButtonState currentButtonState)
        {
            if (currentButtonState == null) return;
            if (currentButtonState.IsPressed)
            {
                if (priorButtonState != null && priorButtonState.IsPressed)
                {
                    if (!currentButtonState.AutoRepeat || (currentButtonState.NextAutoRepeatTime.HasValue && DateTime.Now.CompareTo(currentButtonState.NextAutoRepeatTime.Value) < 0))
                    {
                        return;
                    }
                }


                switch (currentButtonState.Button)
                {
                    case (byte)XBoxWirelessControllerButton.A:
                        _altFunctionButtonIsPressed = true;
                        break;
                    case (byte)XBoxWirelessControllerButton.X:
                        if (_cubeCursor.CursorIsDot)
                        {
                            _cubeCursor.CursorIsDot = false;
                            _cubeCursor.CursorIsHorizontal = true;

                        }
                        else
                        {
                            _cubeCursor.CursorIsDot = true;
                        }
                        break;
                    case (byte)XBoxWirelessControllerButton.Y:
                        if (_cubeCursor.CursorIsDot) 
                        { 
                            _cubeCursor.CursorIsDot = false; 
                            _cubeCursor.CursorIsHorizontal = false; 
                        
                        }
                        else
                        {
                            _cubeCursor.CursorIsDot = true;
                        }
                        break;
                    case (byte)XBoxWirelessControllerButton.B:
                        if (_cubeCursor.CursorIsDot)
                        {
                            Console.WriteLine("B button press detectd");
                            if (_altFunctionButtonIsPressed)
                            {
                                Console.WriteLine($"alt button pressed so running bulk autocoms;autorep: {currentButtonState.AutoRepeat}");
                                _commandQueue.Enqueue("matching-centers-auto-commutator");
                                
                            }
                            else
                            {
                                Console.WriteLine("alt button NOT pressed so running single autocom");
                                _commandQueue.Enqueue("piece-swap-commutator");
                            }
                        }
                        break;
                    case (byte)XBoxWirelessControllerButton.LeftBumper:
                        _commandQueue.Enqueue("U'");
                        break;
                    case (byte)XBoxWirelessControllerButton.RightBumper:
                        _commandQueue.Enqueue("U");
                        break;
                    case (byte)XBoxWirelessControllerButton.XBoxButton:
                        _commandQueue.Enqueue("toggle-stats");
                        break;
                    case (byte)XBoxWirelessControllerButton.Squares:
                        _commandQueue.Enqueue("undo");
                        break;
                    case (byte)XBoxWirelessControllerButton.Lines:
                        if (_altFunctionButtonIsPressed)
                        {
                            _commandQueue.Enqueue("reset");
                        }
                        else
                        {
                            _commandQueue.Enqueue("scramble");
                        }
                        break;
                    case (byte)XBoxWirelessControllerButton.LeftStickButton:
                        _commandQueue.Enqueue("Z'");
                        break;
                    case (byte)XBoxWirelessControllerButton.RightStickButton:
                        _commandQueue.Enqueue("Z");
                        break;
                    default:
                        break;
                }
                if (priorButtonState != null && priorButtonState.NextAutoRepeatTime.HasValue)
                {
                    currentButtonState.NextAutoRepeatTime = DateTime.Now.AddMilliseconds(MIN_TIME_BETWEEN_REPEATS_MS);
                }
                else
                {
                    currentButtonState.NextAutoRepeatTime = DateTime.Now.AddMilliseconds(MIN_TIME_TIL_FIRST_REPEAT_MS);
                }

            }
            else
            {
                currentButtonState.NextAutoRepeatTime = null;
                switch (currentButtonState.Button)
                {
                    case (byte)XBoxWirelessControllerButton.A: 
                        _altFunctionButtonIsPressed = false;
                        break;
                }
            }

        }

        public void RefreshInputs()
        {
            foreach (var button in _gamepadController.Buttons)
            {
                var priorState = _buttonStates[button.Key];
                var pressedNow = button.Value;
                var currentState = new ButtonState(
                    button: button.Key,
                    isPressed: button.Value,
                    autoRepeat: 
                        button.Key == (byte)XBoxWirelessControllerButton.Lines //enable auto repeat on hold down of scramble button
                        || button.Key == (byte)XBoxWirelessControllerButton.B,
                    inhibitAutoRepeatUntilReleased: pressedNow && priorState != null && priorState.InhibitAutoRepeatUntilReleased,
                    nextAutoRepeatTime: priorState?.NextAutoRepeatTime
                    );
                if (!currentState.InhibitAutoRepeatUntilReleased)
                {
                    ProcessButton(priorState, currentState);
                }
                _buttonStates[button.Key] = currentState;
            }

            foreach (var axis in _gamepadController.Axis)
            {
                var priorState = _axisStates[axis.Key];
                var valueNow = axis.Value;

                //prevent accidental double firing of triggers
                if (axis.Key == (byte)XBoxWirelessControllerAxis.LeftTrigger || axis.Key == (byte)XBoxWirelessControllerAxis.RightTrigger)
                {
                    if (valueNow < 0) valueNow = -(AxisState.DEADZONE-1);
                }

                var pressedNow = Math.Abs(valueNow) > AxisState.DEADZONE;
                var currentState = new AxisState(
                    axis: axis.Key,
                    value: valueNow,
                    autoRepeat: axis.Key == (byte)XBoxWirelessControllerAxis.DPadLeftRight || axis.Key == (byte)XBoxWirelessControllerAxis.DPadUpDown, //enable auto-repeat on dpad inputs 
                    inhibitAutoRepeatUntilReleased: pressedNow && priorState !=null && priorState.InhibitAutoRepeatUntilReleased,
                    nextAutoRepeatTime: priorState?.NextAutoRepeatTime
                );
                if (!currentState.InhibitAutoRepeatUntilReleased)
                {
                    ProcessAxis(priorState, currentState);
                }
                _axisStates[axis.Key] = currentState;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DisposeGamepad();
                } 
                _disposed = true;
            }
        }
        private void DisposeGamepad()
        {
            if (_gamepadController != null) try { _gamepadController.Dispose(); } catch { }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
