Shader "Custom/ShieldShader"
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
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
        };
        
        float _EllipseValueA;
        float _EllipseValueB;
        float _EllipseValueC;
        float _EllipseValueD;
        float _EllipseValueE;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = fixed3(1, 0, 0);
            float2 uv = (IN.uv_MainTex - float2(0.5, 0.5)) * 20;
            uv /= 1.3;
            
            float D = pow(_EllipseValueB * uv.x + _EllipseValueE, 2) - 4 * _EllipseValueC * (_EllipseValueA * uv.x * uv.x + _EllipseValueD * uv.x - 1);
            if (D < 0){
                o.Alpha = 0;
                return;
            }
            
            float yHigh = (-(_EllipseValueB * uv.x + _EllipseValueE) + pow(D, 0.5))/(2 * _EllipseValueC);
            float yLow = (-(_EllipseValueB * uv.x + _EllipseValueE) - pow(D, 0.5))/(2 * _EllipseValueC);
            
            o.Alpha = uv.y < yHigh && uv.y > yLow ? 0.5 : 0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
