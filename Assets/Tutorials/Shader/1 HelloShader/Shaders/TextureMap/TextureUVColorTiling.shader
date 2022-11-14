Shader "Custom/Unlit/Texture/UVTiling"
{
    Properties
    {
        _Tiling ("UV Height Offset", Float) = 2
        _UVHeightOffset ("UV Height Offset", Range(0,1)) = 0
        [MainTexture] _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "PreviewType"="Plane"
            "RenderType"="Opaque"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
            // Blend One OneMinusSrcAlpha // Premultiplied transparency
            // Blend One One // Additive
            // Blend OneMinusDstColor One // Soft additive
            // Blend DstColor Zero // Multiplicative
            // Blend DstColor SrcColor // 2x multiplicative

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Tiling;
            float _UVHeightOffset;
            sampler2D _MainTex;

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
                return tex2D(_MainTex, i.uv * _Tiling) * float4(i.uv, _UVHeightOffset, 1);
            }
            ENDCG
        }
    }
}
