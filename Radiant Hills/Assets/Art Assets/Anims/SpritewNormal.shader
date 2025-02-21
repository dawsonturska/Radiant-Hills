Shader "Custom/SpriteWithNormalMap"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                half3 normal = tex2D(_NormalMap, i.uv).rgb;
                normal = normalize(normal * 2.0 - 1.0); // Convert normal map to normal

                // Simple lighting model (just an example)
                half3 lightDir = normalize(float3(1, 1, 1));
                half diff = max(0, dot(normal, lightDir));
                
                col.rgb *= diff; // Apply lighting based on the normal map
                return col;
            }
            ENDCG
        }
    }
}