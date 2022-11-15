Shader "Custom/Unlit/Distortion/WaveAnimated"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)
        _Speed ("Speed", Float) = 0.1
        _WaveRepeat ("Repeate", Float) = 5
        _WaveCount ("Wave Offset", Float) = 3
        _WaveAmplitude ("Wave Amplitude", Float) = 0.1
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

            #include "UnityCG.cginc"

            float4 _ColorA;
            float4 _ColorB;
            float _Speed;
            float _WaveRepeat;
            float _WaveCount;
            float _WaveAmplitude;

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

            float inverseLerp(float a, float b, float v)
            {
                return (v-a)/(b-a);
            }

            Interpolators vert (VertexData v)
            {
                Interpolators output;
                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.uv = v.uv;
                output.normal = v.normal;

                return output;
            }

            float remapToColor(float x)
            {
                return (x + 1) * 0.5;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                const float yOffset = cos(i.uv.x * TAU * _WaveCount) * _WaveAmplitude * 0.01;
                const float timeOffset = _Time.y * _Speed;
                const float fade = i.uv.y;

                float t = cos((i.uv.y + yOffset - timeOffset) * TAU * _WaveRepeat);
                t = remapToColor(t);
                t *= fade;

                const float cullHorizontalFaces = (abs(i.normal.y) < 0.999);
                const float waves = t * cullHorizontalFaces;

                float4 gradient = lerp(_ColorA, _ColorB, i.uv.y);

                return gradient * waves;
            }
            ENDCG
        }
    }
}
