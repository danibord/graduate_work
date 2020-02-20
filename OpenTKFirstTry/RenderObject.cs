using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Windows.Forms;
using OpenTK.Input;
using System.Threading.Tasks;

namespace OpenTKFirstTry
{
    public class RenderObject : IDisposable
    {
        private bool initialized;
        private readonly int vertexArray;
        private readonly int buffer;
        private readonly int verticeCount;
        public RenderObject(Vertex[] vertices)
        {
            verticeCount = vertices.Length;
            vertexArray = GL.GenVertexArray();
            buffer = GL.GenBuffer();

            GL.BindVertexArray(vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexArray);

            GL.NamedBufferStorage(
                buffer,
                Vertex.Size * vertices.Length,        // the size needed by this buffer
                vertices,                           // data to initialize with
                BufferStorageFlags.MapWriteBit);    // at this point we will only write to the buffer


            GL.VertexArrayAttribBinding(vertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(vertexArray, 0);
            GL.VertexArrayAttribFormat(
                vertexArray,
                0,                      // attribute index, from the shader location = 0
                4,                      // size of attribute, vec4
                VertexAttribType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                0);                     // relative offset, first item


            GL.VertexArrayAttribBinding(vertexArray, 1, 0);
            GL.EnableVertexArrayAttrib(vertexArray, 1);
            GL.VertexArrayAttribFormat(
                vertexArray,
                1,                      // attribute index, from the shader location = 1
                4,                      
                VertexAttribType.Float, 
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                16);                     // relative offset after a vec4

           
            GL.VertexArrayVertexBuffer(vertexArray, 0, buffer, IntPtr.Zero, Vertex.Size);
            initialized = true;
        }

            
        public void Render()
        {
            try
            {
                GL.DrawArrays(PrimitiveType.Triangles, 0, verticeCount);
            }
            catch
            {
                MessageBox.Show("Dont Try to launch two Render Windows at the same time");
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Bind()
        {
            GL.BindVertexArray(vertexArray);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (initialized)
                {
                    GL.DeleteVertexArray(vertexArray);
                    GL.DeleteBuffer(buffer);
                    initialized = false;
                }
            }
        }
    }
}