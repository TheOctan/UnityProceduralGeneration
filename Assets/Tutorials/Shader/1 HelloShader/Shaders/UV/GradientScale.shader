Shader "Custom/Unlit/UV/GradientScale"
{
    Properties
    {
        _Scale ("UV Scale", Float) = 1
        _Offset ("UV Offset", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Scale;
            float _Offset;

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

            Interpolators vert (VertexData v)
            {
                Interpolators output;
                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.uv = (v.uv + _Offset) * _Scale;
                return output;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                return fixed4(i.uv.yyy, 1);
            }
            ENDCG
        }
    }
}
