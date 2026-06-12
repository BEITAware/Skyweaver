using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Skyweaver.Rendering;

namespace Skyweaver.Controls.SkyweaverPreferencesControl.Views
{
    public sealed class PreferencesBlueRippleBackgroundEffect : ShaderEffect
    {
        private const string ShaderSource = @"
sampler2D inputSampler : register(s0);
float time : register(c0);
float aspectRatio : register(c1);

// 伪随机哈希函数
float hash(float n)
{
    return frac(sin(n) * 43758.5453);
}

// 二维噪声函数
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

// 动态极光波带生成函数
float softBand(float2 p, float center, float width, float amp, float freq, float speed, float phase)
{
    float curve = center;
    curve += sin(p.x * freq + time * speed + phase) * amp;
    curve += sin(p.x * (freq * 0.47) - time * (speed * 0.62) + phase * 1.7) * (amp * 0.48);

    return exp(-abs(p.y - curve) * width);
}

// 水面涟漪生成函数
float ripple(float2 p, float2 center, float radius, float speed, float phase)
{
    float2 q = p - center;
    float dist = length(q);
    float ring = sin(dist * radius - time * speed + phase);
    float envelope = exp(-dist * dist * 0.72);

    return ring * envelope;
}

// 像素着色器主入口
float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 source = tex2D(inputSampler, uv);
    
    // 将纹理坐标映射到 [-aspectRatio, aspectRatio] 和 [-1.0, 1.0] 空间
    float2 p = float2((uv.x - 0.5) * 2.0 * aspectRatio, (0.5 - uv.y) * 2.0);

    // 计算水面/扰动偏移以增加动态细节
    float waterShift = sin(p.y * 2.35 + time * 0.32) * 0.035;
    waterShift += sin(p.x * 1.65 - time * 0.25) * 0.025;
    float2 q = p + float2(waterShift, -waterShift * 0.55);

    // 1. 极暗蓝黑色基底设计，实现设计图般的高对比度
    float topMix = smoothstep(0.0, 0.7, uv.y);
    float3 topDarkBlue = float3(0.003, 0.01, 0.028);      // 顶部深黑蓝色
    float3 bottomDarkBlue = float3(0.002, 0.014, 0.038);   // 底部深黑蓝色
    float3 baseColor = lerp(topDarkBlue, bottomDarkBlue, topMix);

    // 2. 局部微弱环境光与辉光效果
    float leftTopGlow = exp(-dot(q - float2(-aspectRatio * 0.9, 0.8), q - float2(-aspectRatio * 0.9, 0.8)) * 1.5);
    baseColor += float3(0.0, 0.12, 0.32) * leftTopGlow * 0.45;

    float leftBottomGlow = exp(-dot(q - float2(-aspectRatio * 0.85, -0.65), q - float2(-aspectRatio * 0.85, -0.65)) * 0.8);
    baseColor += float3(0.01, 0.25, 0.65) * leftBottomGlow * 0.6;

    // 3. 动态极光设计 - 聚焦在下半部分（y < 0），并在右侧平滑衰减，保持高对比度
    float normalizedX = q.x / max(aspectRatio, 0.0001);
    float auroraMask = saturate(1.0 - (normalizedX + 0.35) / 1.15); // 右侧淡出遮罩

    // 使用噪声扭曲极光坐标，使其更具流动感
    float2 noiseCoord = q * 1.5 + float2(time * 0.05, -time * 0.04);
    float auroraNoise = noise(noiseCoord) - 0.5;
    float2 qAurora = q + float2(auroraNoise * 0.08, auroraNoise * 0.05);

    // 第一层：中等宽度的背景极光带（中心偏中下 -0.38）
    float auroraLayer1 = softBand(qAurora, -0.38, 4.2, 0.18, 0.85, 0.25, 0.5);
    float3 colorLayer1 = float3(0.0, 0.35, 0.85) * auroraLayer1;

    // 第二层：核心高度集中的亮蓝色极光带（中心偏下 -0.52，波带更窄）
    float auroraLayer2 = softBand(qAurora, -0.52, 7.8, 0.15, 1.25, -0.32, 2.8);
    float3 colorLayer2 = float3(0.02, 0.68, 1.0) * auroraLayer2;

    // 第三层：底部的柔和宽极光晕（中心在最下方 -0.68）
    float auroraLayer3 = softBand(qAurora, -0.68, 2.8, 0.12, 0.65, 0.15, 4.2);
    float3 colorLayer3 = float3(0.0, 0.18, 0.55) * auroraLayer3;

    // 极光耀眼核心高亮：在最亮处叠加亮白色至淡蓝色核心
    float totalAuroraVal = auroraLayer1 * 0.4 + auroraLayer2 * 0.8 + auroraLayer3 * 0.2;
    float coreHighlight = pow(saturate(totalAuroraVal * 1.2), 3.5);
    float3 colorCore = float3(0.55, 0.90, 1.0) * coreHighlight * 0.95;

    // 组合极光图层并应用淡出遮罩
    float3 finalAurora = (colorLayer1 + colorLayer2 + colorLayer3 + colorCore) * auroraMask;

    // 4. 微弱水面涟漪效果
    float rippleA = ripple(q, float2(-0.2, -0.2), 6.5, 0.68, 0.8);
    float rippleB = ripple(q, float2(0.4, -0.5), 5.2, -0.52, 2.0);
    float rippleMask = saturate(0.88 - length(q * float2(0.35, 0.65)) * 0.25);
    float rippleLight = (rippleA * 0.35 + rippleB * 0.25) * rippleMask;

    float3 finalColor = baseColor + finalAurora;
    finalColor += float3(0.02, 0.42, 0.85) * rippleLight * (0.2 + totalAuroraVal * 0.8);

    // 注入微弱细腻噪声颗粒
    float fineNoise = noise(q * 3.5 + float2(time * 0.03, -time * 0.028)) - 0.5;
    finalColor += fineNoise * 0.015;

    // 暗角处理，让边缘更深邃
    float vignette = saturate(1.15 - length(p * float2(0.55, 0.85)) * 0.42);
    finalColor *= 0.42 + vignette * 0.58;

    // 对比度与高光调谐
    finalColor = finalColor / (1.0 + finalColor * 0.12);
    finalColor *= 1.08;
    finalColor = saturate(finalColor);

    return float4(finalColor * max(source.a, 0.0001), source.a);
}";

        private static readonly PixelShader SharedPixelShader = CreatePixelShader();

        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
            nameof(Input),
            typeof(PreferencesBlueRippleBackgroundEffect),
            0);

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
            nameof(Time),
            typeof(double),
            typeof(PreferencesBlueRippleBackgroundEffect),
            new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty AspectRatioProperty = DependencyProperty.Register(
            nameof(AspectRatio),
            typeof(double),
            typeof(PreferencesBlueRippleBackgroundEffect),
            new UIPropertyMetadata(1.0, PixelShaderConstantCallback(1)));

        public PreferencesBlueRippleBackgroundEffect()
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
