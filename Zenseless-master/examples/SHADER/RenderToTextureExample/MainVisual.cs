﻿using Zenseless.Geometry;
using Zenseless.HLGL;
using Zenseless.OpenGL;
using OpenTK.Graphics.OpenGL4;

namespace Example
{
	public class MainVisual
	{
		public MainVisual(IRenderState renderState, IContentLoader contentLoader)
		{
			camera.FarClip = 50;
			camera.Distance = 1.8f;
			camera.TargetY = -0.3f;
			camera.FovY = 70;
			this.renderState = renderState;
			shaderProgram = contentLoader.Load<IShaderProgram>("lambert.*");
			shaderPostProcess = contentLoader.LoadPixelShader("ChromaticAberration");
			shaderPostProcess = contentLoader.LoadPixelShader("swirl");
			shaderPostProcess = contentLoader.LoadPixelShader("Ripple");
			var mesh = Meshes.CreateCornellBox(); //TODO: ATI seams to do VAO vertex attribute ordering different for each shader would need to create own VAO
			geometry = VAOLoader.FromMesh(mesh, shaderProgram);
		}

		public CameraOrbit OrbitCamera { get { return camera; } }
		private readonly IRenderState renderState;

		public void Draw()
		{
			if (shaderProgram is null) return;
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			renderState.Set(BoolState<IDepthState>.Enabled);
			renderState.Set(BoolState<IBackfaceCullingState>.Enabled);
			shaderProgram.Activate();
			var cam = camera.CalcMatrix().ToOpenTK();
			GL.UniformMatrix4(shaderProgram.GetResourceLocation(ShaderResourceType.Uniform, "camera"), true, ref cam);
			geometry.Draw();
			shaderProgram.Deactivate();
			renderState.Set(BoolState<IBackfaceCullingState>.Disabled);
			renderState.Set(BoolState<IDepthState>.Disabled);
		}

		public void DrawWithPostProcessing(float time)
		{
			renderToTexture.Activate(); //start drawing into texture
			Draw();
			renderToTexture.Deactivate(); //stop drawing into texture
			renderToTexture.Texture.Activate(); //us this new texture
			if (shaderPostProcess is null) return;
			shaderPostProcess.Activate(); //activate post processing shader
			GL.Uniform1(shaderPostProcess.GetResourceLocation(ShaderResourceType.Uniform, "iGlobalTime"), time);
			GL.DrawArrays(PrimitiveType.Quads, 0, 4); //draw quad
			shaderPostProcess.Deactivate();
			renderToTexture.Texture.Deactivate();
		}

		public void Resize(int width, int height)
		{
			renderToTexture = new FBOwithDepth(Texture2dGL.Create(width, height));
			renderToTexture.Texture.WrapFunction = TextureWrapFunction.ClampToEdge;
		}

		private CameraOrbit camera = new CameraOrbit();
		private FBO renderToTexture;
		private IShaderProgram shaderPostProcess;
		private IShaderProgram shaderProgram;
		private VAO geometry;
	}
}
