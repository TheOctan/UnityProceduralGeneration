Shader "Custom/Unlit/Texture/Color"
{
    Properties
    {
        [MainColor] _Color ("Color A", Color) = (1,1,1,1)
        [MainTexture] _MainTexture("Texture", 2D) = "white" {}
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

            float4 _Color;
            sampler2D _MainTexture;

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
                return tex2D(_MainTexture, i.uv) * _Color;
            }
            ENDCG
        }
    }
}
