﻿using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.Numerics;
using Zenseless.HLGL;
using Zenseless.OpenGL;

namespace Example
{
	public class RendererPoints
	{
		public RendererPoints(IContentLoader contentLoader)
		{
			shaderProgram = contentLoader.Load<IShaderProgram>(new string[] { "particle.vert", "pointCircle.frag"});
			geometry = new VAO(PrimitiveType.Points);
		}

		public void DrawPoints(Vector3[] points, float size, Color color)
		{
			shaderProgram.Activate();
			geometry.SetAttribute(shaderProgram.GetResourceLocation(ShaderResourceType.Attribute, "position"), points, VertexAttribPointerType.Float, 3); //copy data to gpu mem
			GL.Uniform1(shaderProgram.GetResourceLocation(ShaderResourceType.Uniform, "pointSize"), resolutionMin * size);
			GL.Uniform4(shaderProgram.GetResourceLocation(ShaderResourceType.Uniform, "color"), color);
			geometry.Draw();
			shaderProgram.Deactivate();
		}

		public void Resize(int width, int height)
		{
			resolutionMin = Math.Min(width, height);
		}

		private IShaderProgram shaderProgram;
		private VAO geometry;
		private int resolutionMin;
	}
}
