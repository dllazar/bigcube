namespace LEDMatrixCube.Gamepad
{
    internal class ButtonState
    {
        public ButtonState(byte button, bool isPressed, bool autoRepeat = false, bool inhibitAutoRepeatUntilReleased = false, DateTime? nextAutoRepeatTime = null) { Button = button; IsPressed = isPressed; AutoRepeat = autoRepeat; InhibitAutoRepeatUntilReleased = inhibitAutoRepeatUntilReleased; NextAutoRepeatTime = nextAutoRepeatTime; }
        public byte Button { get; internal set; } = 0;
        public bool IsPressed { get; internal set; } = false;
        public bool AutoRepeat { get; set; } = false;
        public bool InhibitAutoRepeatUntilReleased { get; internal set; } = false;
        public DateTime? NextAutoRepeatTime { get; internal set; } = null;
    }
}
