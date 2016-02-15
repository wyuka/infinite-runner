Shader "Custom/CurvedWorld" {
    Properties {
    	_Color ("Color", Color) = (1,1,1,1)
    	_Smoothness("Smoothness", Range(0, 1)) = 0.5
        // Diffuse texture
        _MainTex ("Base (RGB)", 2D) = "white" {}
        // Degree of curvature
        _Curvature ("Curvature", Float) = 0.001
        _FogColor ("Fog Color", Color) = (0.3, 0.4, 0.7, 1.0)
        _FogFarDistance("Fog far distance", Float) = 3000
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        // Surface shader function is called surf, and vertex preprocessor function is called vert
        // addshadow used to add shadow collector and caster passes following vertex modification
        #pragma surface surf Lambert vertex:vert addshadow
        #pragma multi_compile_fog
 
        // Access the shaderlab properties
        uniform sampler2D _MainTex;
        uniform float _Curvature;
        uniform half _Smoothness;
        uniform fixed4 _Color;
        uniform fixed4 _FogColor;
        uniform float _FogFarDistance;
 
        // Basic input structure to the shader function
        // requires only a single set of UV texture mapping coordinates
        struct Input {
            float2 uv_MainTex;
            half fog;
        };


        // This is where the curvature is applied
        void vert( inout appdata_full v, out Input data)
        {
        	UNITY_INITIALIZE_OUTPUT(Input,data);
            // Transform the vertex coordinates from model space into world space
            float4 vv = mul( _Object2World, v.vertex );
 
            // Now adjust the coordinates to be relative to the camera position
            vv.xyz -= _WorldSpaceCameraPos.xyz;
 
            // Reduce the y coordinate (i.e. lower the "height") of each vertex based
            // on the square of the distance from the camera in the z axis, multiplied
            // by the chosen curvature factor
            vv = float4( (vv.z * vv.z) * - 0.003, (vv.z * vv.z) * - 0.0006, 0.0f, 0.0f );
 
            // Now apply the offset back to the vertices in model space
            v.vertex += mul(_World2Object, vv);
            //data.fog = 0.9f;//clamp(0, 0.0f, 0.0f);
        }

        void mycolor (Input IN, SurfaceOutput o, inout fixed4 color) {
	      #ifdef UNITY_PASS_FORWARDADD
	        UNITY_APPLY_FOG_COLOR(IN.fog, color, float4(0,0,0,0));
	      #else
	        UNITY_APPLY_FOG_COLOR(IN.fog, color, _FogColor);
	      #endif
	    }

        // This is just a default surface shader
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            //o.Smoothness = _Smoothness;
        }
        ENDCG
    }
    // FallBack "Diffuse"
}
