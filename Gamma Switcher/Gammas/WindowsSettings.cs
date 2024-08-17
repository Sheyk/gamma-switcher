using System.Runtime.InteropServices;
namespace Gamma_Switcher.Gammas;

public static class WindowsSettings
{
    public static GammaRamp.RAMP GetGammaRamp()
    {
        var hDc = GammaRamp.GetDC(IntPtr.Zero);
        return GammaRamp.GetGammaRamp(hDc);
    }

    public static void SetGammaRamp(GammaRamp.RAMP ramp)
    {
        var hDc = GammaRamp.GetDC(IntPtr.Zero);
        GammaRamp.SetDeviceGammaRamp(hDc, ref ramp);
    }
}

public class GammaRamp
{
    // Import the necessary functions from gdi32.dll
    [DllImport("gdi32.dll")]
    public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

    [DllImport("gdi32.dll")]
    private static extern bool GetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

    // Import the necessary function from user32.dll to get the device context
    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    public static RAMP GetGammaRamp(IntPtr hDC)
    {
        var ramp = new RAMP
        {
            Red = new ushort[256],
            Green = new ushort[256],
            Blue = new ushort[256]
        };

        if (!GetDeviceGammaRamp(hDC, ref ramp))
            throw new InvalidOperationException("Unable to get gamma ramp.");

        return ramp;
    }

    // Define the RAMP structure
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct RAMP
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private ushort[] _red;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private ushort[] _green;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private ushort[] _blue;

        public ushort[] Red
        {
            get => _red ??= new ushort[256];
            set => _red = value;
        }

        public ushort[] Green
        {
            get => _green ??= new ushort[256];
            set => _green = value;
        }

        public ushort[] Blue
        {
            get => _blue ??= new ushort[256];
            set => _blue = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is RAMP)
            {
                var other = (RAMP)obj;
                return Red.SequenceEqual(other.Red) &&
                       Green.SequenceEqual(other.Green) &&
                       Blue.SequenceEqual(other.Blue);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            foreach (var element in Red) hash = hash * 31 + element.GetHashCode();
            foreach (var element in Green) hash = hash * 31 + element.GetHashCode();
            foreach (var element in Blue) hash = hash * 31 + element.GetHashCode();
            return hash;
        }

        public static bool operator ==(RAMP left, RAMP right) =>
            left.Equals(right);

        public static bool operator !=(RAMP left, RAMP right) =>
            !(left == right);
    }
}
