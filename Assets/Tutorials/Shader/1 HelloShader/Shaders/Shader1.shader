Shader "Unlit/Shader1"
{
    Properties
    {
        _Value ("Value", Float) = 0.5
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

            float _Value;

            struct VertexData
            {
                float4 vertex : POSITION;   // vertex position
                float3 normals : NORMAL;    // 
                float4 tangent : TANGENT;   // 
                float4 color : COLOR;       // 
                float2 uv0 : TEXCOORD0;     // uv0 diffuse/normal map textures
                float2 uv1 : TEXCOORD1;     // uv1 coordinates lightmap coordinates
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;    // clip space position
            };

            Interpolators vert (VertexData v)
            {
                Interpolators output;
                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                return output;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                
                return float4(1,0,0,1);
            }
            ENDCG
        }
    }
}
