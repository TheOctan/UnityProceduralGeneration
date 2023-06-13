Shader "Custom/Unlit/UV/LocalPosition"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VertexData
            {
                float4 vertex : POSITION;   // vertex position
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;    // clip space position
                float4 localPosition : TEXCOORD0;
            };

            Interpolators vert (VertexData v)
            {
                Interpolators output;
                output.localPosition = v.vertex;
                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                return output;
            }

            float3 remapToColor(float3 normal)
            {
                return (normal + 1) * 0.5;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                return float4(i.localPosition.xyz + 0.5, 1);
            }
            ENDCG
        }
    }
}
