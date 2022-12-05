Shader "Custom/Unlit/Normal"
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
                float3 normal : NORMAL;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;    // clip space position
                float3 normal : NORMAL;         // local normal
            };

            Interpolators vert (VertexData v)
            {
                Interpolators output;
                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.normal = UnityObjectToWorldNormal(v.normal);
                return output;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                return fixed4(i.normal, 1);
            }
            ENDCG
        }
    }
}
