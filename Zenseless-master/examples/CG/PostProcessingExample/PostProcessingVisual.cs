﻿using OpenTK.Graphics.OpenGL4;
using System;
using Zenseless.Base;
using Zenseless.HLGL;
using Zenseless.OpenGL;

namespace Example
{
	public class PostProcessingVisual
	{
		public PostProcessingVisual(int width, int height, IContentLoader contentLoader)
		{
			renderToTexture = new FBO(Texture2dGL.Create(width, height));
			renderToTexture.Texture.WrapFunction = TextureWrapFunction.MirroredRepeat;
			try
			{
				shaderProgram = contentLoader.LoadPixelShader("Grayscale");
				shaderProgram = contentLoader.LoadPixelShader("Sepia");
				shaderProgram = contentLoader.LoadPixelShader("Vignetting");
				shaderProgram = contentLoader.LoadPixelShader("Ripple");
				shaderProgram = contentLoader.LoadPixelShader("Swirl");
			}
			catch (ShaderException e)
			{
				Console.WriteLine(e.ShaderLog);
				Console.ReadLine();
			}
		}

		public void Render(Action renderAction)
		{
			//render into texture
			renderToTexture.Activate();
			renderAction.Invoke();
			renderToTexture.Deactivate();

			//use this texture to draw
			renderToTexture.Texture.Activate();
			shaderProgram.Activate();
			//SetShaderParameter("effectScale", 0.1f);
			SetShaderParameter("effectScale", 0.5f + 0.5f * (float)Math.Sin(time.AbsoluteTime - 0.5f));
			SetShaderParameter("iGlobalTime", time.AbsoluteTime);
			DrawWindowFillingQuad();
			shaderProgram.Deactivate();
			renderToTexture.Texture.Deactivate();
		}

		private GameTime time = new GameTime();
		private FBO renderToTexture;
		private IShaderProgram shaderProgram;

		private static void DrawWindowFillingQuad()
		{
			GL.DrawArrays(PrimitiveType.Quads, 0, 4);
		}

		private void SetShaderParameter(string name, float value)
		{
			GL.Uniform1(shaderProgram.GetResourceLocation(ShaderResourceType.Uniform, name), value);
		}
	}
}
