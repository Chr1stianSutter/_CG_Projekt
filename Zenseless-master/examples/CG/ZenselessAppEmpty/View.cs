﻿using OpenTK.Graphics.OpenGL;

namespace ZenselessAppEmpty
{
	internal class View
	{
		public View()
		{
		}

		internal void Draw()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.Begin(PrimitiveType.Quads);
			GL.Vertex2(0.0f, 0.0f);
			GL.Vertex2(0.5f, 0.0f);
			GL.Vertex2(0.5f, 0.5f);
			GL.Vertex2(0.0f, 0.5f);
			GL.End();
		}
	}
}