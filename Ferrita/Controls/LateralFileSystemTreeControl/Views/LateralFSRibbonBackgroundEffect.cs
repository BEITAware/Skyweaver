using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Ferrita.Rendering;

namespace Ferrita.Controls.LateralFileSystemTreeControl.Views
{
    public sealed class LateralFSRibbonBackgroundEffect : ShaderEffect
    {
        private const string ShaderSource = @"
sampler2D inputSampler : register(s0);
float time : register(c0);
float aspectRatio : register(c1);

float hash(float n)
{
    return frac(sin(n) * 43758.5453);
}

float noise(float2 x)
{
    float2 i = floor(x);
    float2 f = frac(x);
    f = f * f * (3.0 - 2.0 * f);

    float n = i.x + i.y * 57.0;
    float a = hash(n + 0.0);
    float b = hash(n + 1.0);
    float c = hash(n + 57.0);
    float d = hash(n + 58.0);

    return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
}

float ribbonLayer(float2 p, float freq, float speed, float phase, float amp, float offset, float width, float coreWidth)
{
    float curve = sin(p.x * freq + time * speed + phase) * amp;
    curve += sin(p.x * (freq * 0.57) - time * (speed * 0.48) + phase * 1.71) * (amp * 0.58);
    curve += offset;

    float dist = abs(p.y - curve);
    float glow = exp(-dist * (width * 1.08));
    float core = exp(-dist * (coreWidth * 1.12));

    float shimmer = 0.2 * sin(p.x * 7.6 - time * (speed * 2.15) + phase * 1.9);
    shimmer *= shimmer;

    return glow * 0.74 + core * (0.2 + shimmer * 0.48);
}

float arcSweep(float2 p, float offset, float thickness, float bend, float speed, float phase)
{
    float arc = offset + sin(p.x * bend + time * speed + phase) * 0.16 + p.x * p.x * 0.055;
    float dist = abs(p.y - arc);
    return exp(-dist * thickness);
}

float dotMesh(float2 p, float2 center, float2 density, float drift, float tilt)
{
    float2 q = p - center;
    q.x += q.y * tilt;

    float2 maskUv = q * float2(0.9, 1.5);
    float envelope = exp(-dot(maskUv, maskUv) * 0.82);

    float2 uv = q * density + float2(time * drift, -time * drift * 0.65);
    uv.y += sin(uv.x * 0.24 + time * 0.2) * 0.45;

    float2 cell = frac(uv) - 0.5;
    float dotMask = 1.0 - smoothstep(0.028, 0.2, dot(cell, cell));
    float scan = 0.45 + 0.55 * sin(uv.x * 0.19 + uv.y * 0.37);
    scan *= scan;

    return dotMask * scan * envelope;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 source = tex2D(inputSampler, uv);
    float2 p = float2((uv.x - 0.5) * 2.0 * aspectRatio, (0.5 - uv.y) * 2.0);

    float vignette = saturate(1.34 - length(p * float2(0.84, 1.12)) * 0.62);
    float2 glowCenter = p - float2(-0.18, 0.1);
    float2 topCenter = p - float2(0.02, 0.94);
    float2 rightCenter = p - float2(1.12, 0.15);

    float centerGlow = exp(-dot(glowCenter, glowCenter) * 0.92);
    float topGlow = exp(-dot(topCenter, topCenter) * 2.4);
    float rightGlow = exp(-dot(rightCenter, rightCenter) * 1.75);

    float gradientMix = saturate(0.34 + (p.y + 1.0) * 0.34 + centerGlow * 0.42);
    
    // Deep blue background gradient
    float3 baseColor = lerp(float3(0.01, 0.02, 0.08), float3(0.04, 0.12, 0.28), gradientMix);
    baseColor *= 0.62 + vignette * 0.34;
    baseColor += float3(0.02, 0.04, 0.1) * topGlow;
    baseColor += float3(0.02, 0.05, 0.12) * rightGlow * 0.45;

    float haze = (noise(p * 2.15 + float2(time * 0.04, -time * 0.03)) - 0.5) * 0.06;
    baseColor += haze;

    // Make ribbons horizontal by mapping diag to p directly
    float2 diag = float2(p.x, p.y);
    float2 diag2 = float2(p.x * 1.1, p.y * 0.9);

    float r1 = ribbonLayer(diag + float2(0.12, -0.56), 1.24, 0.16, 0.35, 0.16, -0.08, 7.2, 24.0);
    float r2 = ribbonLayer(diag + float2(-0.2, -0.43), 1.08, 0.14, -1.28, 0.18, 0.08, 6.8, 23.0);
    float r3 = ribbonLayer(diag + float2(0.0, -0.64), 1.86, 0.24, 1.85, 0.12, -0.14, 14.0, 62.0);
    float r4 = ribbonLayer(diag2 + float2(-0.62, 0.12), 1.34, 0.22, 2.4, 0.1, 0.02, 10.5, 42.0);

    float sweepA = arcSweep(diag, -0.5, 2.8, 0.95, 0.11, 0.28);
    float sweepB = arcSweep(diag, -0.25, 3.5, 1.15, -0.09, 1.37);

    float dotsLeft = dotMesh(p, float2(-1.1, -0.52), float2(11.0, 13.8), 0.08, 0.45);
    float dotsRight = dotMesh(p, float2(1.28, -0.12), float2(12.0, 15.0), -0.07, -0.52);
    float dots = dotsLeft * 0.72 + dotsRight * 0.55;

    float glint = exp(-abs(diag.y + 0.59 + sin(diag.x * 1.58 - time * 0.2) * 0.095) * 16.0);

    // Dark blue ribbons
    float3 ribbonColor = float3(0.1, 0.25, 0.6) * (r1 * 0.32 + r2 * 0.28);
    ribbonColor += float3(0.15, 0.3, 0.7) * r3 * 0.24;
    ribbonColor += float3(0.05, 0.2, 0.5) * r4 * 0.14;
    ribbonColor += float3(0.05, 0.15, 0.4) * (sweepA * 0.14 + sweepB * 0.1);
    ribbonColor += float3(0.4, 0.6, 0.9) * glint * 0.08;

    float3 meshColor = float3(0.1, 0.2, 0.5) * dots * 0.2;

    float3 finalColor = baseColor + ribbonColor + meshColor;
    finalColor = saturate(finalColor * 1.2);

    return float4(finalColor * max(source.a, 0.0001), source.a);
}
";

        private static readonly PixelShader SharedPixelShader = CreatePixelShader();

        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
            nameof(Input),
            typeof(LateralFSRibbonBackgroundEffect),
            0);

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
            nameof(Time),
            typeof(double),
            typeof(LateralFSRibbonBackgroundEffect),
            new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty AspectRatioProperty = DependencyProperty.Register(
            nameof(AspectRatio),
            typeof(double),
            typeof(LateralFSRibbonBackgroundEffect),
            new UIPropertyMetadata(1.0, PixelShaderConstantCallback(1)));

        public LateralFSRibbonBackgroundEffect()
        {
            PixelShader = SharedPixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(TimeProperty);
            UpdateShaderValue(AspectRatioProperty);
        }

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        public double Time
        {
            get => (double)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public double AspectRatio
        {
            get => (double)GetValue(AspectRatioProperty);
            set => SetValue(AspectRatioProperty, value);
        }

        private static PixelShader CreatePixelShader()
        {
            var shader = new PixelShader();
            var shaderBytes = DirectXShaderCompiler.CompilePixelShader(ShaderSource);

            using var stream = new MemoryStream(shaderBytes, writable: false);
            shader.SetStreamSource(stream);
            shader.Freeze();
            return shader;
        }
    }
}
