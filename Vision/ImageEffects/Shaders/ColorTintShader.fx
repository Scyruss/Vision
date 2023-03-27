sampler2D Input : register(S0);
float r : register(c0);
float g : register(c1);
float b : register(c2);

float4 PSMain(float2 uv : TEXCOORD) : COLOR
{
	float4 texColor = tex2D(Input, uv);
	float4 final = float4(r*texColor.r/255,g*texColor.g/255,b*texColor.b/255,1);
	final.a = texColor.a; //restore alpha value

	return final;
}