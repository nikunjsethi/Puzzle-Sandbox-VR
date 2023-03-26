Shader "Unlit/CellAutomata"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CellSize ("Cell Size", Range(2, 100)) = 5
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _CellSize;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uv = i.uv * _CellSize;
                float4 color = tex2D(_MainTex, uv);

                float2 topLeft = (uv - float2(1, 1)) / _CellSize;
                float2 top = (uv - float2(0, 1)) / _CellSize;
                float2 topRight = (uv - float2(-1, 1)) / _CellSize;
                float2 left = (uv - float2(1, 0)) / _CellSize;
                float2 right = (uv - float2(-1, 0)) / _CellSize;
                float2 bottomLeft = (uv - float2(1, -1)) / _CellSize;
                float2 bottom = (uv - float2(0, -1)) / _CellSize;
                float2 bottomRight = (uv - float2(-1, -1)) / _CellSize;

                float count = tex2D(_MainTex, topLeft).r +
                              tex2D(_MainTex, top).r +
                              tex2D(_MainTex, topRight).r +
                              tex2D(_MainTex, left).r +
                              tex2D(_MainTex, right).r +
                              tex2D(_MainTex, bottomLeft).r +
                              tex2D(_MainTex, bottom).r +
                              tex2D(_MainTex, bottomRight).r;

                if (color.r > 0.5) {
                    if (count < 2.5 || count > 3.5) {
                        color.rgb = float3(0, 0, 0);
                    }
                } else {
                    if (count > 2.5 && count < 3.5) {
                        color.rgb = float3(1, 1, 1);
                    }
                }

                return color;
            }
            ENDCG
        }
    }
}
