namespace LEDMatrixCube
{
    internal class AxisState
    {
        internal const int DEADZONE = 30000;
        public AxisState(byte axis, short value, bool autoRepeat = false, bool inhibitAutoRepeatUntilReleased = false, DateTime? nextAutoRepeatTime = null) { Axis = axis; Value = value; AutoRepeat = autoRepeat; InhibitAutoRepeatUntilReleased = inhibitAutoRepeatUntilReleased; NextAutoRepeatTime = nextAutoRepeatTime; }
        public byte Axis { get; internal set; } = 0;
        public short Value { get; internal set; } =0;
        public bool IsPressed { get { return Math.Abs(Value) > DEADZONE; } }
        public bool AutoRepeat { get; set; } = false;
        public bool InhibitAutoRepeatUntilReleased { get; set; } = false;
        public DateTime? NextAutoRepeatTime { get; internal set; } = null;
    }
}
