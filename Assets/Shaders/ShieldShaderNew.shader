Shader "Unlit/ShieldShaderNew"
{
    Properties
    {
        _EllipseValueA ("Ellipse Value A", Float) = 1
        _EllipseValueB ("Ellipse Value B", Float) = 1
        _EllipseValueC ("Ellipse Value C", Float) = 1
        _EllipseValueD ("Ellipse Value D", Float) = 1
        _EllipseValueE ("Ellipse Value E", Float) = 1
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float _EllipseValueA;
            float _EllipseValueB;
            float _EllipseValueC;
            float _EllipseValueD;
            float _EllipseValueE;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = fixed4(2, 0, 0, 1);
                float2 uv = (i.uv - float2(0.5, 0.5)) * 20;
                uv /= 1.3;
                
                float D = pow(_EllipseValueB * uv.x + _EllipseValueE, 2) - 4 * _EllipseValueC * (_EllipseValueA * uv.x * uv.x + _EllipseValueD * uv.x - 1);
                if (D < 0){
                    col.a = 0;
                    return col;
                }
                
                float yHigh = (-(_EllipseValueB * uv.x + _EllipseValueE) + pow(D, 0.5))/(2 * _EllipseValueC);
                float yLow = (-(_EllipseValueB * uv.x + _EllipseValueE) - pow(D, 0.5))/(2 * _EllipseValueC);
                
                col.a = uv.y < yHigh && uv.y > yLow ? 0.5 : 0;
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
