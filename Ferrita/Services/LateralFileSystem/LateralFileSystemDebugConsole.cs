using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Ferrita.Services.LateralFileSystem
{
    internal static class LateralFileSystemDebugConsole
    {
#if DEBUG
        private static readonly object s_syncRoot = new();
        private static bool s_consoleReady;
        private static int s_sequenceNumber;
#endif

        [Conditional("DEBUG")]
        public static void Write(string area, string message)
        {
#if DEBUG
            lock (s_syncRoot)
            {
                EnsureConsole();

                var sequenceNumber = Interlocked.Increment(ref s_sequenceNumber);
                var line = $"[LateralFS {DateTime.Now:HH:mm:ss.fff} T{Environment.CurrentManagedThreadId:D2} #{sequenceNumber:D6} {area}] {message}";
                Debug.WriteLine(line);

                try
                {
                    Console.WriteLine(line);
                }
                catch
                {
                }
            }
#endif
        }

        [Conditional("DEBUG")]
        public static void WriteException(string area, Exception ex, string? context = null)
        {
#if DEBUG
            ArgumentNullException.ThrowIfNull(ex);

            Write(area, $"{context ?? "Exception"}: {ex.GetType().Name}: {ex.Message}");
            if (!string.IsNullOrWhiteSpace(ex.StackTrace))
            {
                Write(area, ex.StackTrace!);
            }
#endif
        }

#if DEBUG
        private static void EnsureConsole()
        {
            if (s_consoleReady)
            {
                return;
            }

            if (GetConsoleWindow() == IntPtr.Zero)
            {
                _ = AttachConsole(AttachParentProcess) || AllocConsole();
            }

            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            try
            {
                Console.Title = "Ferrita LateralFS Debug";
            }
            catch
            {
            }

            s_consoleReady = true;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int processId);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        private const int AttachParentProcess = -1;
#endif
    }
}
