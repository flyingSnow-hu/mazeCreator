Shader "Unlit/GridColor"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (1,1,1,1)
        _BorderWidth ("Border Width", Range(0.01,0.1)) = 0.01
        [MaterialToggle] _Left("Left Wall", Float) = 0
        [MaterialToggle] _Top("Top Wall", Float) = 0
        [MaterialToggle] _Right("Right Wall", Float) = 0
        [MaterialToggle] _Bottom("Bottom Wall", Float) = 0
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

            fixed4 _BorderColor;
            fixed4 _MainColor;
            fixed _BorderWidth;
            fixed _Left, _Top, _Right, _Bottom;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _MainColor;
                if (i.uv.x < _BorderWidth && _Left > 0) return _BorderColor;
                if (i.uv.y > 1 - _BorderWidth && _Top > 0) return _BorderColor;
                if (i.uv.x > 1 - _BorderWidth && _Right > 0) return _BorderColor;
                if (i.uv.y < _BorderWidth && _Bottom > 0) return _BorderColor;   
                return col;
            }
            ENDCG
        }
    }
}
