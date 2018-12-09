Shader "Custom/Spline Unlit"
{
    Properties
    {
        _Color ("Spline color", Color) = (1, 1, 0, 1)
        _Depth("Spline Z Value", Float) = 0.0
        _NumControlPoints("Number of control points", Float) = 0.0
    }

    SubShader
    {
        LOD 200

        Tags
        {
            "IgnoreProjector" = "True"
            "RenderType" = "Opaque"
        }

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite On
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4      _Color;
            float4      _ControlPoints[1000];
            float       _Width;
            float       _Depth;
            float       _NumControlPoints;
            
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : POSITION;
                float t: COLOR;
            };

            v2f vert (appdata v)
            {
                float alpha = v.vertex.x;
                float centerOffset = v.vertex.y;
                // float UNUSED = v.vertex.z;

                float b = alpha * (_NumControlPoints - 1);
                int index = b;
                float t = b - index;
                float t2 = t * t;

                float2 cp0 = _ControlPoints[index].xy;
                float2 cp1 = _ControlPoints[index +1].xy;
                float2 cp2 = _ControlPoints[index +2].xy;
                float2 cp3 = _ControlPoints[index +3].xy;

                float2 base0 = -cp0 + cp3 + (cp1 - cp2) * 3;
                float2 base1 = 2*cp0 - 5*cp1 + 4*cp2 - cp3;

                float2 pos = 0.5 * (base0 * t2*t + base1 * t2 + (-cp0+cp2)*t + 2 * cp1);
                float2 tang = base0 * t2 * 1.5 + base1 * t + 0.5 * (cp2 - cp0);

                tang = normalize(tang) * _Width;

                pos = pos + lerp(float2(-tang.y, tang.x), float2(tang.y, -tang.x), centerOffset);

                v2f o;
                o.pos = UnityObjectToClipPos(float3(pos.xy, _Depth));
                o.t = t;
                return o;
            }

            float4 frag (v2f i) : COLOR
            {
                if (1 - i.t > .95)
                    return float4(1, 0, 1, 1);
                else
                    return lerp(_Color, float4(1, 1, 1, 1), i.t);
            }
            ENDCG
        }
    }
}
