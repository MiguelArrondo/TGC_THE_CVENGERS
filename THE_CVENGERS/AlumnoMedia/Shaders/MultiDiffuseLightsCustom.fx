/*
* Shader utilizado por el ejemplo "Lights/EjemploMultiDiffuseLights.cs"
* Permite aplicar iluminación dinámica con PhongShading a nivel de pixel.
* Soporta hasta 4 luces por objeto en la misma pasada.
* Las luces tienen atenuación por distancia.
* Solo se calcula el componente Diffuse para acelerar los cálculos. Se ignora
* el Specular.
*/

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
   Texture = (texLightMap);
};


//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialDiffuseColor; //Color RGB

//Variables de las 5 luces
float3 lightColor[5]; //Color RGB de las 4 luces
float4 lightPosition[5]; //Posicion de las 4 luces
float lightIntensity[5]; //Intensidad de las 4 luces
float lightAttenuation[5]; //Factor de atenuacion de las 4 luces

//Parametros de Spot
float3 spotLightDir0; //Direccion del cono de luz
float3 spotLightDir1; //Direccion del cono de luz
float3 spotLightDir2; //Direccion del cono de luz
float3 spotLightDir3; //Direccion del cono de luz
float3 spotLightDir4; //Direccion del cono de luz
float spotLightAngleCos[5]; //Angulo de apertura del cono de luz (en radianes)
float spotLightExponent[5]; //Exponente de atenuacion dentro del cono de luz

/**************************************************************************************/
/* MultiDiffuseLightsTechnique */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
   float4 Position : POSITION0;
   float3 Normal : NORMAL0;
   float4 Color : COLOR;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
};

//Vertex Shader
VS_OUTPUT vs_general(VS_INPUT input)
{
	VS_OUTPUT output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space 
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
	

	return output;
}


//Input del Pixel Shader
struct PS_INPUT
{
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
};




//Funcion para calcular color RGB de Diffuse
float3 computeDiffuseComponent(float3 surfacePosition, float3 N, int i)
{
	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten = length(lightPosition[i].xyz - surfacePosition);
	float3 Pn =  normalize(lightPosition[i].xyz - surfacePosition);
	float3 Ln = (lightPosition[i].xyz - surfacePosition) / distAtten;
	float spotAtten;
	distAtten = distAtten * lightAttenuation[i];
	
	if(i == 0){
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	spotAtten = dot(-spotLightDir0, Pn);
	spotAtten = (spotAtten > spotLightAngleCos[i]) 
					? pow(spotAtten, spotLightExponent[i])
					: 0.0;
	}
	if(i == 1){
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	spotAtten = dot(-spotLightDir1, Pn);
	spotAtten = (spotAtten > spotLightAngleCos[i]) 
					? pow(spotAtten, spotLightExponent[i])
					: 0.0;
	}
	if(i == 2){
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	spotAtten = dot(-spotLightDir2, Pn);
	spotAtten = (spotAtten > spotLightAngleCos[i]) 
					? pow(spotAtten, spotLightExponent[i])
					: 0.0;
	}
	if(i == 3){
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	spotAtten = dot(-spotLightDir3, Pn);
	spotAtten = (spotAtten > spotLightAngleCos[i]) 
					? pow(spotAtten, spotLightExponent[i])
					: 0.0;
	}
	if(i == 4){
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	spotAtten = dot(-spotLightDir4, Pn);
	spotAtten = (spotAtten > spotLightAngleCos[i]) 
					? pow(spotAtten, spotLightExponent[i])
					: 0.0;
	}
	float intensity = lightIntensity[i] * spotAtten / distAtten; //Dividimos intensidad sobre distancia

	if(dot(N, Ln) < 0){
	N = -N;
	}
	
	//Calcular Diffuse (N dot L)
	return intensity * lightColor[i].rgb * materialDiffuseColor * max(0.0, dot(N, Ln));
}



//Pixel Shader para Point Light
float4 point_light_ps(PS_INPUT input) : COLOR0
{      
	float3 Nn = normalize(input.WorldNormal);

	//Emissive + Diffuse de 4 luces PointLight
	float3 diffuseLighting = materialEmissiveColor;

	//Diffuse 0
	diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 0);

	//Diffuse 1
	diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 1);
	
	//Diffuse 2
	diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 2);
	
	//Diffuse 3
	diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 3);
	
	//Diffuse 4
	diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 4);
	
	//Obtener texel de la textura
	float4 texelColor = tex2D(diffuseMap, input.Texcoord);
	texelColor.rgb *= diffuseLighting;

	return texelColor;
}

/*
* Technique con iluminacion
*/
technique MultiDiffuseLightsTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 vs_general();
	  PixelShader = compile ps_3_0 point_light_ps();
   }

}


