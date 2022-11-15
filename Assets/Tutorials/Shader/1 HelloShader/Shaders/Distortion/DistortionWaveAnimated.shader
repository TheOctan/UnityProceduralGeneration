Shader "Custom/Unlit/Distortion/WaveAnimated"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)
        _ColorStart ("Color Start", Range(0,1)) = 0
        _ColorEnd ("Color End", Range(0,1)) = 1

        [Space(10)]
        _WaveRepeat ("Repeate", Float) = 5
        _WaveCount ("Wave Offset", Float) = 3
        _WaveAmplitude ("Wave Amplitude", Float) = 0.1
        _Speed ("Speed", Float) = 0.1

        [Space(10)]
        _FadeStart ("Fade Start", Range(0,1)) = 0
        _FadeEnd ("Fade End", Range(0,1)) = 1
    }
    SubShader
    {
        Tags 
        {
            "RenderType"="Transparent"  // tag to inform the render pipeline of what type this is
            "Queue"="Transparent"       // change the render order
        }

        Pass
        {
            Cull Off
            ZWrite Off
            Blend One One // Additive

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define TAU 6.283185307179586
            #define PI 3.14159265359

            #include "UnityCG.cginc"

            float4 _ColorA;
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;

            float _WaveRepeat;
            float _WaveCount;
            float _WaveAmplitude;

            float _FadeStart;
            float _FadeEnd;

            float _Speed;

            struct VertexData
            {
                float4 vertex : POSITION;   // vertex position
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;    // clip space position
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            Interpolators vert (VertexData v)
            {
                Interpolators output;
                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.uv = v.uv;
                output.normal = v.normal;

                return output;
            }

            float inverseLerp(float a, float b, float v)
            {
                return (v-a)/(b-a);
            }

            float remapToColor(float x)
            {
                return (x + 1) * 0.5;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                const float yOffset = cos(i.uv.x * TAU * _WaveCount) * _WaveAmplitude * 0.01 * (1 - i.uv.y);
                const float timeOffset = _Time.y * _Speed;
                const float waves = remapToColor(cos((i.uv.y + yOffset - timeOffset) * TAU * _WaveRepeat));

                const float cullHorizontalFaces = (abs(i.normal.y) < 0.999);

                float fade = inverseLerp(_FadeStart, _FadeEnd, i.uv.y);
                fade = saturate(fade);

                float gradientRange = inverseLerp(_ColorStart, _ColorEnd, i.uv.y);
                gradientRange = saturate(gradientRange);
                const float4 gradient = lerp(_ColorA, _ColorB, gradientRange);

                return waves * fade * cullHorizontalFaces * gradient;
            }
            ENDCG
        }
    }
}
