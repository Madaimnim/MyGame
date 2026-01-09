Shader "Custom/Sprite/Effect_Outline_InnerEdge_Flash2D"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineSize ("Outline Size (px)", Float) = 0

        _InnerEdgeColor ("Inner Edge Color", Color) = (0,0,0,1)
        _InnerEdgeSize ("Inner Edge Size (px)", Float) = 0

        _FlashStrength ("Flash Strength", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;

            fixed4 _OutlineColor;
            float _OutlineSize;

            fixed4 _InnerEdgeColor;
            float _InnerEdgeSize;

            float _FlashStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // 原圖透明直接略過
                if (col.a <= 0)
                {
                    // 外框（向外）
                    if (_OutlineSize > 0)
                    {
                        float2 off = _MainTex_TexelSize.xy * _OutlineSize;
                        float a =
                            tex2D(_MainTex, i.uv + float2(off.x, 0)).a +
                            tex2D(_MainTex, i.uv + float2(-off.x, 0)).a +
                            tex2D(_MainTex, i.uv + float2(0, off.y)).a +
                            tex2D(_MainTex, i.uv + float2(0, -off.y)).a;
                        if (a > 0) return _OutlineColor;
                    }
                    return 0;
                }

                // 內邊（向內）
                if (_InnerEdgeSize > 0)                                             //常駐的內編
                //if (_FlashStrength > 0)                                             //閃白才啟用的內編
                {
                    float2 off = _MainTex_TexelSize.xy * _InnerEdgeSize;
                    float a =
                        tex2D(_MainTex, i.uv + float2(off.x, 0)).a *
                        tex2D(_MainTex, i.uv + float2(-off.x, 0)).a *
                        tex2D(_MainTex, i.uv + float2(0, off.y)).a *
                        tex2D(_MainTex, i.uv + float2(0, -off.y)).a;

                    if (a < 1)
                        col.rgb = lerp(col.rgb, _InnerEdgeColor.rgb, _InnerEdgeColor.a);
                }

                // 閃白（最後套）
                col.rgb = lerp(col.rgb, 1.0.xxx, _FlashStrength);

                return col;
            }
            ENDCG
        }
    }
}
