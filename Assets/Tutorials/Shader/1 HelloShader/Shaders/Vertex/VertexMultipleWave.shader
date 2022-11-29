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

            float remapToColor(float x)
            {
                return (x + 1) * 0.5;
            }

            float falloffMap(float2 uv)
            {
                uv = uv * 2 - 1;
                return 1 - max(abs(uv.x), abs(uv.y));
            }

            float wave(float2 uv)
            {
                const float timeOffset = _Time.y * _Speed;
                const float waveX = cos((uv.x - timeOffset) * TAU * _WaveCount);
                const float waveY = cos((uv.y - timeOffset) * TAU * _WaveCount);
                return waveX * waveY;
            }

            Interpolators vert (VertexData v)
            {
                Interpolators output;

                v.vertex.z = wave(v.uv) * falloffMap(v.uv) * _WaveAmplitude * 0.001;

                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.uv = v.uv;
                return output;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                return remapToColor(wave(i.uv)) * falloffMap(i.uv);
            }
            ENDCG
        }
    }
}
