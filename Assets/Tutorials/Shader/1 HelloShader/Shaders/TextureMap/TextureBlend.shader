Shader "Custom/Unlit/Texture/TextureUV"
{
    Properties
    {
        _Blend ("UV Height Offset", Range(0,1)) = 0
        [MainTexture] _MainTex("Texture", 2D) = "white" {}
        _SecondTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "PreviewType"="Plane"
            "RenderType"="Opaque"
            "Queue"="Transparent"
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

            float _Blend;
            sampler2D _MainTex;
            sampler2D _SecondTex;

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
                float4 sample1 = tex2D(_MainTex, i.uv);
                float4 sample2 = tex2D(_SecondTex, i.uv);
                return lerp(sample1, sample2, _Blend);
            }
            ENDCG
        }
    }
}
