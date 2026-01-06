Shader "Custom/SpriteOutline2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineSize ("Outline Size (px)", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a > 0)
                    return col;

                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;

                float alpha =
                    tex2D(_MainTex, i.uv + float2(offset.x, 0)).a +
                    tex2D(_MainTex, i.uv + float2(-offset.x, 0)).a +
                    tex2D(_MainTex, i.uv + float2(0, offset.y)).a +
                    tex2D(_MainTex, i.uv + float2(0, -offset.y)).a;

                if (alpha > 0)
                    return _OutlineColor;

                return col;
            }
            ENDCG
        }
    }
}
