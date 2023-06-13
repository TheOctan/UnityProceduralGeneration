Shader "Custom/Unlit/Vertex/RippleWave"
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

            float radialUV(float2 uv)
            {
                const float2 uvsCentered = uv * 2 - 1;
                return min(1, length(uvsCentered));
            }

            float wave(float radialUV)
            {
                const float timeOffset = _Time.y * _Speed;
                return cos((radialUV - timeOffset) * TAU * _WaveCount);
            }

            Interpolators vert (VertexData v)
            {
                Interpolators output;

                const float radialLength = radialUV(v.uv);
                const float gradient = 1 - radialLength;

                v.vertex.z = wave(radialLength) * gradient * _WaveAmplitude * 0.001;

                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.uv = v.uv;
                return output;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                const float radialLength = radialUV(i.uv);
                const float gradient = 1 - radialLength;

                const float grayScale = remapToColor(wave(radialLength)) * gradient;

                return float4(grayScale.xxx, 1);
            }
            ENDCG
        }
    }
}
