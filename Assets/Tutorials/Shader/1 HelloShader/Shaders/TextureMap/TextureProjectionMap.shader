Shader "Custom/Unlit/Texture/TextureProjectionMap"
{
    Properties
    {
        [MainTexture] _MainTex("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct VertexData
            {
                float4 vertex : POSITION;   // vertex position
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;    // clip space position
                float2 uv : TEXCOORD0;
                float3 worldPosition : TEXCOORD1;
            };

            Interpolators vert (VertexData v)
            {
                Interpolators output;
                output.worldPosition = mul(UNITY_MATRIX_M, v.vertex); // object to world
                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return output;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                const float2 topDownProjection = i.worldPosition.xz;
                return tex2D(_MainTex, topDownProjection);
            }
            ENDCG
        }
    }
}
