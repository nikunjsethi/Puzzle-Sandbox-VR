Shader "Custom/DistanceShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Color1 ("Color1", Color) = (0.2,0.2,0.2,1)
        _Distance1 ("Distance1", Range(0,1)) = 0.2
        _Distance2 ("Distance2", Range(0,1)) = 0.6
        _LineWidth ("Line Width", Range(0,10)) = 1
        _LastPoint ("Last Point", Vector) = (0,0,0,0)
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
            };
 
            struct v2f {
                float4 vertex : SV_POSITION;
                float dist : TEXCOORD0;
            };
 
            float _Distance1;
            float _Distance2;
            float _LineWidth;
            float4 _Color;
            float4 _Color1;
            float4 _LastPoint;
 
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.dist = length(_LastPoint.xyz - v.vertex.xyz) / _LineWidth;
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = _Color;
                if (i.dist < _Distance1) {
                    col = _Color1;
                } else if (i.dist < _Distance2) {
                    col = lerp(_Color1, _Color, (i.dist - _Distance1) / (_Distance2 - _Distance1));
                }
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
