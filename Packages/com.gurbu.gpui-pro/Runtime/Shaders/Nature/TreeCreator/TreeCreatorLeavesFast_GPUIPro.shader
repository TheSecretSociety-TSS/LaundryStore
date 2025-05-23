// GPUInstancer enabled version of Unity built-in shader "Nature/Tree Creator Leaves Fast"

Shader "GPUInstancerPro/Nature/Tree Creator Leaves Fast" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
    _TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
    _ShadowStrength("Shadow Strength", Range(0,1)) = 1.0

    _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}

    // These are here only to provide default values
    [HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
    [HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
    [HideInInspector] _SquashAmount ("Squash", Float) = 1
}

SubShader {
    Tags {
        "IgnoreProjector"="True"
        "RenderType" = "TreeLeaf"
    }
    LOD 200

    Pass {
        Tags { "LightMode" = "ForwardBase" }
        Name "ForwardBase"

    CGPROGRAM
		#include "UnityCG.cginc"
        #include "Packages/com.gurbu.gpui-pro/Runtime/Shaders/Include/GPUInstancerSetup.hlsl"
		#pragma instancing_options procedural:setupGPUI
		#pragma multi_compile_instancing
        #include "UnityBuiltin3xTreeLibrary.cginc"

        #pragma vertex VertexLeaf
        #pragma fragment FragmentLeaf
        #pragma multi_compile_fwdbase nolightmap
        #pragma multi_compile_fog
        #pragma multi_compile _ LOD_FADE_CROSSFADE

        sampler2D _MainTex;
        float4 _MainTex_ST;

        fixed _Cutoff;
        sampler2D _ShadowMapTexture;

        struct v2f_leaf {
            float4 pos : SV_POSITION;
            fixed4 diffuse : COLOR0;
        #if defined(SHADOWS_SCREEN)
            fixed4 mainLight : COLOR1;
        #endif
            float2 uv : TEXCOORD0;
        #if defined(SHADOWS_SCREEN)
            float4 screenPos : TEXCOORD1;
        #endif
            uint gpuiInstanceID : SV_InstanceID;
            UNITY_FOG_COORDS(2)
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f_leaf VertexLeaf (appdata_full v)
        {
            v2f_leaf o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            o.gpuiInstanceID = gpui_InstanceID;
#else
            o.gpuiInstanceID = 0;
#endif
            TreeVertLeaf(v);
            o.pos = UnityObjectToClipPos(v.vertex);

            fixed ao = v.color.a;
            ao += 0.1; ao = saturate(ao * ao * ao); // emphasize AO

            fixed3 color = v.color.rgb * ao;

            float3 worldN = UnityObjectToWorldNormal (v.normal);

            fixed4 mainLight;
            mainLight.rgb = ShadeTranslucentMainLight (v.vertex, worldN) * color;
            mainLight.a = v.color.a;
            o.diffuse.rgb = ShadeTranslucentLights (v.vertex, worldN) * color;
            o.diffuse.a = 1;
        #if defined(SHADOWS_SCREEN)
            o.mainLight = mainLight;
            o.screenPos = ComputeScreenPos (o.pos);
        #else
            o.diffuse += mainLight;
        #endif
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
            UNITY_TRANSFER_FOG(o,o.pos);
            return o;
        }

        fixed4 FragmentLeaf (v2f_leaf IN) : SV_Target
        {
    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            gpui_InstanceID = IN.gpuiInstanceID;
    #endif
            UNITY_APPLY_DITHER_CROSSFADE(IN.pos.xy);
            fixed4 albedo = tex2D(_MainTex, IN.uv);
            fixed alpha = albedo.a;
            clip (alpha - _Cutoff);

        #if defined(SHADOWS_SCREEN)
            half4 light = IN.mainLight;
            half atten = tex2Dproj(_ShadowMapTexture, UNITY_PROJ_COORD(IN.screenPos)).r;
            light.rgb *= lerp(1, atten, _ShadowStrength);
            light.rgb += IN.diffuse.rgb;
        #else
            half4 light = IN.diffuse;
        #endif

            fixed4 col = fixed4 (albedo.rgb * light, 0.0);
            UNITY_APPLY_FOG(IN.fogCoord, col);
            return col;
        }

    ENDCG
    }
}
}
