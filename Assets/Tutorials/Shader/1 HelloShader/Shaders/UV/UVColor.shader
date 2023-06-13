Shader "Custom/Unlit/UV/Color"
{
    Properties
    {
        _UVHeightOffset ("UV Height Offset", Range(0,1)) = 0
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

            float _UVHeightOffset;

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
                output.uv = v.uv;
                return output;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                return fixed4(i.uv, _UVHeightOffset , 1);
            }
            ENDCG
        }
    }
}
