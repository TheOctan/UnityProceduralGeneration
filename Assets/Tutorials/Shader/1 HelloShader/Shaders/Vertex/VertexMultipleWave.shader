Shader "Custom/Unlit/Vertex/MultipleWave"
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

            float falloffMap(float2 uv)
            {
                uv = uv * 2 - 1;
                return 1 - max(abs(uv.x), abs(uv.y));
            }

            Interpolators vert (VertexData v)
            {
                Interpolators output;

                const float gradient = falloffMap(v.uv);
                const float timeOffset = _Time.y * _Speed;
                const float waveX = cos((v.uv.x - timeOffset) * TAU * _WaveCount);
                const float waveY = cos((v.uv.y - timeOffset) * TAU * _WaveCount);
                v.vertex.z = waveX * waveY * gradient * _WaveAmplitude * 0.001;

                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.uv = v.uv;
                return output;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                const float gradient = falloffMap(i.uv);
                const float timeOffset = _Time.y * _Speed;
                const float waveX = remapToColor(cos((i.uv.x - timeOffset) * TAU * _WaveCount));
                const float waveY = remapToColor(cos((i.uv.y - timeOffset) * TAU * _WaveCount));

                return waveX * waveY * gradient;
            }
            ENDCG
        }
    }
}
