Shader "Mobile/Diffuse (Mesh Animation)" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        
        [Header(Mesh Animation)]
        _AnimTex ("Animation", 2D) = "white" {}
        _AnimMul ("Animation Bounds Size", Vector) = (1, 1, 1, 0)
        _AnimAdd ("Animation Bounds Offset", Vector) = (0, 0, 0, 0)
        [PerRendererData] _AnimTime ("Animation Time", Vector) = (0, 1, 1, 0) /* (x: start, y: length, z: speed, w: startTime) */
        [PerRendererData] _AnimLoop ("Animation Loop", Float) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 150
    
        CGPROGRAM
        #pragma surface surf Lambert noforwardadd addshadow vertex:vert
        #pragma multi_compile_instancing 
        #pragma target 2.5
        #pragma require samplelod
        
        sampler2D _MainTex;
        sampler2D _AnimTex;
        float4 _AnimTex_TexelSize;
        
        float4 _AnimMul;
        float4 _AnimAdd;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _AnimTime)
            UNITY_DEFINE_INSTANCED_PROP(float, _AnimLoop)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
            float2 vertcoord: TEXCOORD1;
            float4 color : COLOR0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        
        struct Input {
            float2 uv_MainTex;
        };
        
        void vert (inout appdata v) {            
            UNITY_SETUP_INSTANCE_ID(v);
            
            float4 t = UNITY_ACCESS_INSTANCED_PROP(Props, _AnimTime);
            float looping = UNITY_ACCESS_INSTANCED_PROP(Props, _AnimLoop);
            
            float progress = (_Time.y - t.w) * t.z;
            float progress01 = lerp(saturate(progress), frac(progress), looping);
            float2 coords = float2(0.5 + v.vertcoord.x, 0.5 + t.x + progress01 * t.y) * _AnimTex_TexelSize.xy;
            float4 position = tex2Dlod(_AnimTex, float4(coords, 0, 0)) * _AnimMul + _AnimAdd;
            
            v.vertex = float4(position.xyz, 1.0);
        }
        
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    
    Fallback "Mobile/Diffuse"
}