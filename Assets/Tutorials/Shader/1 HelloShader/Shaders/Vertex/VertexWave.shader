Shader "Custom/Unlit/Vertex/Wave"
{
    Properties
    {
        [Header(Wave)]
        _WaveCount ("Wave Offset", Float) = 5
        _WaveAmplitude ("Wave Amplitude", Float) = 1
        _Speed ("Speed", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define TAU 6.283185307179586

            #include "UnityCG.cginc"

            float _WaveCount;
            float _WaveAmplitude;

            float _Speed;

            struct VertexData
            {
                float4 vertex : POSITION;   // vertex position
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;    // clip space position
                float2 uv : TEXCOORD0;
            };

            float inverseLerp(float a, float b, float v)
            {
                return (v-a)/(b-a);
            }

            float remapToColor(float x)
            {
                return (x + 1) * 0.5;
            }

            Interpolators vert (VertexData v)
            {
                Interpolators output;

                const float gradient = 1 - cos(v.uv.x * TAU);
                const float timeOffset = _Time.y * _Speed;
                const float wave = cos((v.uv.x - timeOffset) * TAU * _WaveCount);
                v.vertex.z = wave * gradient * _WaveAmplitude * 0.001;

                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.uv = v.uv;
                return output;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                const float gradient = 1 - cos(i.uv.x * TAU);
                const float timeOffset = _Time.y * _Speed;
                const float wave = cos((i.uv.x - timeOffset) * TAU * _WaveCount);

                return remapToColor(wave) * gradient;
            }
            ENDCG
        }
    }
}
