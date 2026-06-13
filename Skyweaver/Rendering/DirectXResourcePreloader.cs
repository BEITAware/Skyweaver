using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Skyweaver.Rendering
{
    internal static class DirectXResourcePreloader
    {
        private static bool _areShaderConstructorsPreloaded;
        private static bool _areRenderResourcesPreloaded;

        public static void PreloadAll()
        {
            var effectTypes = GetShaderEffectTypes();
            PreloadShaderConstructors(effectTypes);
            PreloadRenderResources(effectTypes);
        }

        private static void PreloadShaderConstructors(Type[] effectTypes)
        {
            if (_areShaderConstructorsPreloaded)
            {
                return;
            }

            _areShaderConstructorsPreloaded = true;

            foreach (var effectType in effectTypes)
            {
                try
                {
                    RuntimeHelpers.RunClassConstructor(effectType.TypeHandle);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"DirectX shader preload failed for {effectType.FullName}: {ex}");
                }
            }
        }

        private static void PreloadRenderResources(Type[] effectTypes)
        {
            if (_areRenderResourcesPreloaded)
            {
                return;
            }

            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() => PreloadRenderResources(effectTypes));
                return;
            }

            _areRenderResourcesPreloaded = true;

            foreach (var effectType in effectTypes)
            {
                try
                {
                    if (Activator.CreateInstance(effectType) is ShaderEffect effect)
                    {
                        RenderEffectOnce(effect);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"DirectX render preload failed for {effectType.FullName}: {ex}");
                }
            }
        }

        private static void RenderEffectOnce(ShaderEffect effect)
        {
            var rectangle = new Rectangle
            {
                Width = 32,
                Height = 32,
                Fill = Brushes.White,
                Effect = effect
            };

            var size = new Size(32, 32);
            rectangle.Measure(size);
            rectangle.Arrange(new Rect(size));
            rectangle.UpdateLayout();

            var bitmap = new RenderTargetBitmap(
                32,
                32,
                96,
                96,
                PixelFormats.Pbgra32);
            bitmap.Render(rectangle);
            bitmap.Freeze();
        }

        private static Type[] GetShaderEffectTypes()
        {
            try
            {
                return typeof(DirectXResourcePreloader)
                    .Assembly
                    .GetTypes()
                    .Where(type => !type.IsAbstract && typeof(ShaderEffect).IsAssignableFrom(type))
                    .OrderBy(type => type.FullName ?? type.Name, StringComparer.Ordinal)
                    .ToArray();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types
                    .Where(type => type != null && !type.IsAbstract && typeof(ShaderEffect).IsAssignableFrom(type))
                    .Cast<Type>()
                    .OrderBy(type => type.FullName ?? type.Name, StringComparer.Ordinal)
                    .ToArray();
            }
        }
    }
}
