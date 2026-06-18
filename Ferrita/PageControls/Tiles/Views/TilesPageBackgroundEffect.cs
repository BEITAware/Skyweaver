using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Ferrita.Rendering;

namespace Ferrita.PageControls.Tiles.Views
{
    public sealed class TilesPageBackgroundEffect : ShaderEffect
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

float aeroAurora(float2 p)
{
    float value = 0.0;
    for (int i = 0; i < 7; i++)
    {
        float fi = float(i);
        float xCenter = (-0.65 + fi * 0.22) * aspectRatio;
        
        // 运动极度缓慢
        xCenter += sin(time * 0.012 + fi * 1.4) * 0.08 * aspectRatio;
        
        // 极小波动振幅使极光几乎完全垂直
        float wave = sin(p.y * 1.1 + time * 0.03 + fi) * 0.004;
        
        float dist = abs(p.x - (xCenter + wave));
        // 大幅减小 width 系数以获得非常柔化的发散边缘效果
        float width = 1.6 + fi * 0.25;
        value += exp(-dist * width);
    }
    return value;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 source = tex2D(inputSampler, uv);
    float2 p = float2((uv.x - 0.5) * 2.0 * aspectRatio, (0.5 - uv.y) * 2.0);

    // 1. 深蓝色渐变背景 (保持低明度，提升蓝色饱和度)
    float3 colorTop = float3(0.02, 0.05, 0.11); // 顶部深邃的高饱和蓝色
    float3 colorBottom = float3(0.05, 0.11, 0.22); // 底部较明亮的高饱和蓝色
    float gradientMix = (p.y + 1.0) * 0.5;
    float3 baseColor = lerp(colorBottom, colorTop, saturate(gradientMix));
    
    // 2. 密集纵向的 Aero 极光变换 (适当降低光强，避免冲淡深色背景)
    float totalAurora = aeroAurora(p);
    float3 auroraColor = float3(0.95, 0.98, 1.0) * totalAurora * 0.015;
    baseColor += auroraColor;

    // 3. 不发光的柔和的数条旋转丝带 (使用极坐标并在屏幕右下角外侧旋转，呈现优美的弧线运动，消除球团聚集)
    float2 ribbonCenter = float2(aspectRatio * 1.5, -1.5);
    float2 qr = p - ribbonCenter;
    float R = length(qr);
    float theta = atan2(qr.y, qr.x);
    
    // 缓慢旋转的相位
    float thetaRot = theta - time * 0.05;

    float dist1 = abs(R - (1.2 + sin(thetaRot * 3.0) * 0.12));
    float dist2 = abs(R - (1.5 + sin(thetaRot * 4.0 + 1.2) * 0.09));
    float dist3 = abs(R - (0.9 + sin(thetaRot * 2.5 - 0.8) * 0.15));
    float dist4 = abs(R - (1.8 + sin(thetaRot * 3.5 + 2.0) * 0.08));

    float ribbonVal1 = smoothstep(0.035, 0.0, dist1);
    float ribbonVal2 = smoothstep(0.030, 0.0, dist2);
    float ribbonVal3 = smoothstep(0.040, 0.0, dist3);
    float ribbonVal4 = smoothstep(0.032, 0.0, dist4);

    // 调整颜色为中等亮度的雅灰色
    float3 rColor1 = float3(0.28, 0.28, 0.28);
    float3 rColor2 = float3(0.24, 0.24, 0.24);
    float3 rColor3 = float3(0.32, 0.32, 0.32);
    float3 rColor4 = float3(0.30, 0.30, 0.30);

    // 蒙版限制在下三分之一 (p.y < -0.33) 和右五分之二 (uv.x > 0.6)
    float yMask = 1.0 - smoothstep(-0.45, -0.25, p.y);
    float xMask = smoothstep(0.48, 0.62, uv.x);
    float ribbonMask = yMask * xMask;

    baseColor = lerp(baseColor, rColor1, ribbonVal1 * 0.45 * ribbonMask);
    baseColor = lerp(baseColor, rColor2, ribbonVal2 * 0.40 * ribbonMask);
    baseColor = lerp(baseColor, rColor3, ribbonVal3 * 0.50 * ribbonMask);
    baseColor = lerp(baseColor, rColor4, ribbonVal4 * 0.42 * ribbonMask);

    // 4. 一些发光的四处游走的 Shimmering Lights (降低发光强度以保留深色背景美感)
    float2 lPos1 = float2(sin(time * 0.040) * aspectRatio * 0.75, cos(time * 0.028) * 0.65);
    float2 lPos2 = float2(cos(time * 0.048 + 1.2) * aspectRatio * 0.70, sin(time * 0.035 + 0.6) * 0.60);
    float2 lPos3 = float2(sin(time * 0.032 - 1.8) * aspectRatio * 0.60, cos(time * 0.52 + 2.2) * 0.55);
    float2 lPos4 = float2(cos(time * 0.055 + 2.8) * aspectRatio * 0.78, sin(time * 0.030 - 0.8) * 0.72);
    float2 lPos5 = float2(sin(time * 0.045 + 0.6) * aspectRatio * 0.50, cos(time * 0.42 - 1.2) * 0.50);
    float2 lPos6 = float2(cos(time * 0.038 - 0.5) * aspectRatio * 0.65, sin(time * 0.047 + 1.0) * 0.65);
    float2 lPos7 = float2(sin(time * 0.051 + 2.0) * aspectRatio * 0.72, cos(time * 0.033 - 1.5) * 0.58);
    float2 lPos8 = float2(cos(time * 0.025 + 0.8) * aspectRatio * 0.82, sin(time * 0.058 + 2.4) * 0.74);

    float shim1 = 0.5 + 0.5 * sin(time * 1.5);
    float shim2 = 0.5 + 0.5 * sin(time * 1.8 + 1.0);
    float shim3 = 0.5 + 0.5 * sin(time * 1.2 + 2.0);
    float shim4 = 0.5 + 0.5 * sin(time * 2.0 + 0.5);
    float shim5 = 0.5 + 0.5 * sin(time * 1.4 + 1.5);
    float shim6 = 0.5 + 0.5 * sin(time * 1.6 + 0.8);
    float shim7 = 0.5 + 0.5 * sin(time * 1.9 + 2.2);
    float shim8 = 0.5 + 0.5 * sin(time * 1.3 + 1.7);

    // 提高 exp 内的系数值 (从 14-22 提高至 42-52) 来缩小大小
    float g1 = exp(-length(p - lPos1) * 45.0) * shim1;
    float g2 = exp(-length(p - lPos2) * 48.0) * shim2;
    float g3 = exp(-length(p - lPos3) * 42.0) * shim3;
    float g4 = exp(-length(p - lPos4) * 46.0) * shim4;
    float g5 = exp(-length(p - lPos5) * 52.0) * shim5;
    float g6 = exp(-length(p - lPos6) * 44.0) * shim6;
    float g7 = exp(-length(p - lPos7) * 50.0) * shim7;
    float g8 = exp(-length(p - lPos8) * 47.0) * shim8;

    float3 lightColor = float3(0.70, 0.88, 1.0) * (g1 + g2 + g3 + g4 + g5 + g6 + g7 + g8) * 0.20;
    baseColor += lightColor;

    // 5. 移除深蓝色叠加层，直接呈现下方各层的蓝色相加效果

    return float4(baseColor * max(source.a, 0.0001), source.a);
}
";

        private static readonly PixelShader SharedPixelShader = CreatePixelShader();

        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
            nameof(Input),
            typeof(TilesPageBackgroundEffect),
            0);

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
            nameof(Time),
            typeof(double),
            typeof(TilesPageBackgroundEffect),
            new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty AspectRatioProperty = DependencyProperty.Register(
            nameof(AspectRatio),
            typeof(double),
            typeof(TilesPageBackgroundEffect),
            new UIPropertyMetadata(1.0, PixelShaderConstantCallback(1)));

        public TilesPageBackgroundEffect()
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
