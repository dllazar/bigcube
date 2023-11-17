using RPiRgbLEDMatrix;
using System.Net;

namespace LEDMatrixCube.Overlays
{
    internal class StatsOverlay
    {
        internal struct Stats
        {
            public string CPUTempCelcius { get; internal set; }
            public bool PiIsThrottled { get; internal set; }
            public bool PiLowVoltageAlarm { get; internal set; }
            public bool WifiConnected { get; internal set; }
            public string HostName { get; internal set; }
            public bool GamepadConnected { get; internal set; }
        }

        private readonly RGBLedFont _font = new($"fonts{Path.DirectorySeparatorChar}4x6.bdf");

        public StatsOverlay(){}

        public void Render(RGBLedCanvas canvas, uint xOffset=0, uint yOffset=0)
        {
            var stats = CollectStats();
            var lineHeight = 8u;
            var lineX = xOffset;
            var lineY = yOffset;
            for (var y = lineY; y < lineY + 64; y++)
            {
                canvas.DrawLine((int)lineX, (int)y, (int)(lineX + 64), (int)y, Colors.Black);
            }
            canvas.DrawText(_font, (int)lineX, (int)lineY, Colors.White, $"WiFi:{(stats.WifiConnected ? "Y": "N")}"); lineY += lineHeight;
            canvas.DrawText(_font, (int)lineX, (int)lineY, Colors.White, $"{stats.HostName}"); lineY += lineHeight;
            canvas.DrawText(_font, (int)lineX, (int)lineY, Colors.White, $"CPU:{stats.CPUTempCelcius} °C"); lineY += lineHeight;
            canvas.DrawText(_font, (int)lineX, (int)lineY, Colors.White, $"Throttled:{(stats.PiIsThrottled ? "Y" : "N")}"); lineY += lineHeight;
            canvas.DrawText(_font, (int)lineX, (int)lineY, Colors.White, $"Lo Voltage:{(stats.PiLowVoltageAlarm ? "Y" : "N")}"); lineY += lineHeight;
            canvas.DrawText(_font, (int)lineX, (int)lineY, Colors.White, $"Gamepad:{(stats.GamepadConnected ? "Y" : "N")}"); lineY += lineHeight;
        }
        private static Stats CollectStats()
        {
            //TODO: these are dummy values for testing the diag stats overlay
            var stats = new Stats();
            stats.HostName = Dns.GetHostName();
            stats.GamepadConnected = true;
            stats.WifiConnected = true;
            stats.CPUTempCelcius = "60";
            stats.PiIsThrottled = false;
            stats.PiLowVoltageAlarm = false;
            return stats;
        }
    }
}