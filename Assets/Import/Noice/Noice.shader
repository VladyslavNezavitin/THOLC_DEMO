Shader "Unlit/UnlitFloatingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Angle ("Rotation Speed (deg/sec)", int) = 0
        _FSpeed ("Floating Speed", float) = 0
        _FHeight ("Floating Height", float) = 0
        _WaveScaleX ("Wave Scale X", Range(1, 100)) = 1
        _WaveScaleY ("Wave Scale Y", Range(1, 100)) = 1
        _WaveSpeed ("Wave Speed", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half _Angle;
            half _FSpeed;
            half _FHeight;
            half _WaveScaleX;
            half _WaveScaleY;
            half _WaveSpeed;

            float4 RotateAroundYInDegrees (float4 vertex, float degrees)
            {
                float alpha = degrees * UNITY_PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float4(mul(m, vertex.xz), vertex.yw).xzyw;
            }

            v2f vert (appdata v)
            {
                v2f o;
                
                v.vertex = RotateAroundYInDegrees(v.vertex, _Time.y * _Angle);
                v.vertex.y += sin(v.vertex.x * _WaveScaleX + _Time.y * _WaveSpeed) / (100 / _WaveScaleY);
                v.vertex.y += sin(_Time.y * _FSpeed) * _FHeight;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
