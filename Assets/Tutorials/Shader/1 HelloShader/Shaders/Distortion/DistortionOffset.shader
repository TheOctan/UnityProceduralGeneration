Shader "Custom/Unlit/Distortion/Offset"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)
        _Repeat ("Repeate", Float) = 5
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

            float4 _ColorA;
            float4 _ColorB;
            float _Repeat;

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

            Interpolators vert (VertexData v)
            {
                Interpolators output;
                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.uv = v.uv;
                return output;
            }

            float remapToColor(float x)
            {
                return (x + 1) * 0.5;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                const float xOffset = i.uv.y;
                const float t = cos((i.uv.x + xOffset) * TAU * _Repeat);
                return lerp(_ColorA, _ColorB, remapToColor(t));
            }
            ENDCG
        }
    }
}