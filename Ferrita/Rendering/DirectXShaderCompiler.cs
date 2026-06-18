using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Ferrita.Rendering
{
    internal static class DirectXShaderCompiler
    {
        private const uint EnableStrictness = 0x00000800;

        [DllImport("d3dcompiler_47.dll", EntryPoint = "D3DCompile", CharSet = CharSet.Ansi)]
        private static extern int D3DCompile(
            byte[] srcData,
            IntPtr srcDataSize,
            string? sourceName,
            IntPtr defines,
            IntPtr include,
            string entryPoint,
            string target,
            uint flags1,
            uint flags2,
            out IntPtr code,
            out IntPtr errorMessages);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr GetBufferPointerDelegate(IntPtr instance);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UIntPtr GetBufferSizeDelegate(IntPtr instance);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate uint ReleaseDelegate(IntPtr instance);

        public static byte[] CompilePixelShader(string source, string entryPoint = "main", string profile = "ps_3_0")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(source);
            ArgumentException.ThrowIfNullOrWhiteSpace(entryPoint);
            ArgumentException.ThrowIfNullOrWhiteSpace(profile);

            var compileFlags = UsesD3D9PixelProfile(profile) ? 0u : EnableStrictness;
            var sourceBytes = Encoding.UTF8.GetBytes(source);

            var result = D3DCompile(
                sourceBytes,
                (IntPtr)sourceBytes.Length,
                "ChatRibbonBackgroundEffect.hlsl",
                IntPtr.Zero,
                IntPtr.Zero,
                entryPoint,
                profile,
                compileFlags,
                0,
                out var shaderBlob,
                out var errorBlob);

            try
            {
                if (result < 0)
                {
                    var message = errorBlob != IntPtr.Zero
                        ? ReadBlobAsString(errorBlob)
                        : $"HLSL compilation failed, HRESULT=0x{result:X8}";

                    throw new InvalidOperationException(message);
                }

                return ReadBlob(shaderBlob);
            }
            finally
            {
                Release(errorBlob);
                Release(shaderBlob);
            }
        }

        private static bool UsesD3D9PixelProfile(string profile)
        {
            return profile.StartsWith("ps_2_", StringComparison.OrdinalIgnoreCase)
                || profile.StartsWith("ps_3_", StringComparison.OrdinalIgnoreCase);
        }

        private static byte[] ReadBlob(IntPtr blob)
        {
            if (blob == IntPtr.Zero)
            {
                return Array.Empty<byte>();
            }

            var bufferPointer = GetBufferPointer(blob);
            var bufferSize = checked((int)GetBufferSize(blob));
            var data = new byte[bufferSize];
            Marshal.Copy(bufferPointer, data, 0, bufferSize);
            return data;
        }

        private static string ReadBlobAsString(IntPtr blob)
        {
            var data = ReadBlob(blob);
            return Encoding.UTF8.GetString(data).TrimEnd('\0', '\r', '\n');
        }

        private static IntPtr GetBufferPointer(IntPtr blob)
        {
            var vtable = Marshal.ReadIntPtr(blob);
            var method = Marshal.ReadIntPtr(vtable, IntPtr.Size * 3);
            var callback = Marshal.GetDelegateForFunctionPointer<GetBufferPointerDelegate>(method);
            return callback(blob);
        }

        private static nuint GetBufferSize(IntPtr blob)
        {
            var vtable = Marshal.ReadIntPtr(blob);
            var method = Marshal.ReadIntPtr(vtable, IntPtr.Size * 4);
            var callback = Marshal.GetDelegateForFunctionPointer<GetBufferSizeDelegate>(method);
            return (nuint)callback(blob).ToUInt64();
        }

        private static void Release(IntPtr instance)
        {
            if (instance == IntPtr.Zero)
            {
                return;
            }

            var vtable = Marshal.ReadIntPtr(instance);
            var method = Marshal.ReadIntPtr(vtable, IntPtr.Size * 2);
            var callback = Marshal.GetDelegateForFunctionPointer<ReleaseDelegate>(method);
            _ = callback(instance);
        }
    }
}
