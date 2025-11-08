Shader "Custom/PassthroughDisplayShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white"
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _BaseMap_TexelSize;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

  
            #define SIN(x) (.5*sin(x)+.5)
            inline half luminance(half3 c) { return dot(c, half3(0.299h, 0.587h, 0.114h)); }

            // Sobel edge detection using the _BaseMap sampler directly
            inline half edgeDetect(float2 uv)
            {
                half2 px = 1./half2(1280, 960); // one pixel in UV units

                half tl = luminance(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv + px * float2(-1,-1)).rgb);
                half  t = luminance(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv + px * float2( 0,-1)).rgb);
                half tr = luminance(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv + px * float2( 1,-1)).rgb);

                half  l = luminance(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv + px * float2(-1, 0)).rgb);
                half  r = luminance(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv + px * float2( 1, 0)).rgb);

                half bl = luminance(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv + px * float2(-1, 1)).rgb);
                half  b = luminance(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv + px * float2( 0, 1)).rgb);
                half br = luminance(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv + px * float2( 1, 1)).rgb);

                half gx = -tl - 2.0h*l - bl + tr + 2.0h*r + br;
                half gy = -tl - 2.0h*t - tr + bl + 2.0h*b + br;

                return length(half2(gx, gy));  // edge magnitude
            }

            inline half3 rgb2hsv(half3 c)
            {
                const half4 K = half4(0.0h, -1.0h/3.0h, 2.0h/3.0h, -1.0h);
                half4 p = (c.g < c.b) ? half4(c.bg, K.wz) : half4(c.gb, K.xy);
                half4 q = (c.r < p.x) ? half4(p.xyw, c.r) : half4(c.r, p.yzx);

                half d = q.x - min(q.w, q.y);
                half e = 1.0e-5h;                // slightly larger epsilon for half precision
                half h = abs(q.z + (q.w - q.y) / (6.0h * d + e));
                half s = d / (q.x + e);
                half v = q.x;
                return half3(h, s, v);
            }

            inline half3 hsv2rgb(half3 c)
            {
                half3 rgb = clamp(abs(frac(c.x + half3(0.0h, 1.0h/3.0h, 2.0h/3.0h)) * 6.0h - 3.0h) - 1.0h, 0.0h, 1.0h);
                return c.z * lerp(half3(1.0h, 1.0h, 1.0h), rgb, c.y);
            }

            half3 pal(half x) {return .5+.5*cos(x*2.*PI-half3(0, 23, 21));}

            half4 frag(Varyings IN) : SV_Target
            {
                half2 uv = IN.uv;
                half2 uvc = uv-.5;
                float tt = _Time.y;

                uvc *= lerp(.85, 1.05, SIN(length(uvc)*4. - tt*2.));
                uv = uvc + .5;
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) * _BaseColor;

                half3 hsv = rgb2hsv(col.rgb);
                
                half edge = edgeDetect(uv);

                half val = luminance(col.rgb) + 1.5*edgeDetect(uv);
                //col.rgb = half3(0, val, .5*pow(val, 2.));

                col.rgb = hsv2rgb(half3((abs(uvc.x)+abs(uvc.y))+hsv.x*2. - .5*tt, smoothstep(0.3, 1., hsv.y), 1.))*1.5*edge;
              //  col.rgb = lerp(.5*hsv2rgb(half3(hsv.x*8. - .1*tt, hsv.y, hsv.z)), col.rgb, edge);

                col.a = lerp(.9, 1., edge); 

                // col.rgb = tanh(col.rgb);

                // col.a = SIN((length(uv-.5)*10. - tt*5.));
                return col;
            }

            ENDHLSL
        }
    }
}
