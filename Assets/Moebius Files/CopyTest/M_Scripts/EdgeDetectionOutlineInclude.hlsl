#ifndef SOBEL_INCLUDED
#define SOBEL_INCLUDED

// A bunch of convolution filter stuff to make custom nodes in shader graph
// Check out Ned Makes Games videos on convolution filters https://www.youtube.com/watch?v=RMt6DcaMxcE&list=PLAUha41PUKAaYVYT7QwxOtiUllckLZrir&index=4

/*
TEXTURE2D_X(_BlitTexture);
float4 Unity_Universal_SampleBuffer_BlitSource_float(float2 uv)
{
	uint2 pixelCoords = uint2(uv * _ScreenSize.xy);
	return LOAD_TEXTURE2D_X_LOD(_BlitTexture, pixelCoords, 0);
}
*/

static float simpleBlur[9] = {
	1,1,1,
	1,1,1,
	1,1,1
};

static float gaussianBlur[9] = {
	1,2,1,
	2,4,2,
	1,2,1
};

static float sobelYMatrix[9] = {
	1,2,1,
	0,0,0,
	-1,-2,-1
};

static float sobelXMatrix[9] = {
	1,0,-1,
	2,0,-2,
	1,0,-1
};

static float2 sobelSamplePoints[9] = {
	float2(-1,1),float2(0,1),float2(1,1),
	float2(-1,0),float2(0,0),float2(1,1),
	float2(-1,-1),float2(0,-1),float2(1,-1),
};

void TextureSobel_float(float2 UV, float Thickness, UnityTexture2D Tex, UnitySamplerState SS, out float Out) {
	float2 sobel = 0;

	[unroll] for (int i = 0; i < 9; i++)
	{
		float depth = SAMPLE_TEXTURE2D(Tex, SS, UV + sobelSamplePoints[i] * Thickness).r;
		sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
	}

	Out = length(sobel);
}

void DepthSobel_float(float2 UV, float Thickness, out float Out) {
	float2 sobel = 0;

	[unroll] for (int i = 0; i < 9; i++)
	{
		float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + sobelSamplePoints[i] * Thickness);
		sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
	}

	Out = length(sobel);
}

void NormalSobel_float(float2 UV, float Thickness, out float Out) {
	float2 sobel = 0;

	[unroll] for (int i = 0; i < 9; i++)
	{
		float normal = mul(SHADERGRAPH_SAMPLE_SCENE_NORMAL(UV + sobelSamplePoints[i] * Thickness), (float3x3) UNITY_MATRIX_I_V);
		sobel += normal * float2(sobelXMatrix[i], sobelYMatrix[i]);
	}

	Out = length(sobel);
}

void NormalTextureSample_float(float2 UV, out float3 Out) {
	Out = mul(SHADERGRAPH_SAMPLE_SCENE_NORMAL(UV), (float3x3) UNITY_MATRIX_I_V);
	//Out = SHADERGRAPH_SAMPLE_SCENE_NORMAL(UV);
}

void ColorSobel_float(float2 UV, float Thickness, out float Out) {
	float2 sobelR = 0;
	float2 sobelG = 0;
	float2 sobelB = 0;

	[unroll] for (int i = 0; i < 9; i++)
	{
		float3 rgb = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV + sobelSamplePoints[i] * Thickness);
		float2 kernel = float2(sobelXMatrix[i], sobelYMatrix[i]);

		sobelR += rgb.r * kernel;
		sobelG += rgb.g * kernel;
		sobelB += rgb.b * kernel;
	}

	Out = max(length(sobelR), max(length(sobelG), length(sobelB)));
}

/*
void ColorSimpleBlur_float(float2 UV, float Thickness, out float3 Out) {
	float colorR = 0;
	float colorG = 0;
	float colorB = 0;

	[unroll] for (int i = 0; i < 9; i++)
	{
		float3 rgb = Unity_Universal_SampleBuffer_BlitSource_float(UV + sobelSamplePoints[i] * Thickness);
		float kernel = simpleBlur[i];

		colorR += rgb.r * kernel;
		colorG += rgb.g * kernel;
		colorB += rgb.b * kernel;
	}


	Out = 0.11111111111 * float3(colorR, colorG, colorB);
}


void ColorGaussianBlur_float(float2 UV, float Thickness, out float3 Out) {
	float colorR = 0;
	float colorG = 0;
	float colorB = 0;

	[unroll] for (int i = 0; i < 9; i++)
	{
		float3 rgb = Unity_Universal_SampleBuffer_BlitSource_float(UV + sobelSamplePoints[i] * Thickness);
		float kernel = gaussianBlur[i];

		colorR += rgb.r * kernel;
		colorG += rgb.g * kernel;
		colorB += rgb.b * kernel;
	}


	Out = 0.0625 * float3(colorR, colorG, colorB);
}

void ColorSobelWithTransparents_float(float2 UV, float Thickness, out float Out) {
	float2 sobelR = 0;
	float2 sobelG = 0;
	float2 sobelB = 0;

	[unroll] for (int i = 0; i < 9; i++)
	{
		float3 rgb = Unity_Universal_SampleBuffer_BlitSource_float(UV + sobelSamplePoints[i] * Thickness);
		float2 kernel = float2(sobelXMatrix[i], sobelYMatrix[i]);
		//float kernel = sobelXMatrix[i];
		//kernel = sobelYMatrix[i];

		sobelR += rgb.r * kernel;
		sobelG += rgb.g * kernel;
		sobelB += rgb.b * kernel;
	}

	Out = max(length(sobelR), max(length(sobelG), length(sobelB)));
}
*/
#endif

