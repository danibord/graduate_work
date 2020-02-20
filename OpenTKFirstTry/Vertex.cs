using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace OpenTKFirstTry
{
    public struct Vertex
    {
        public const int Size = (4 + 4) * 4; // size of struct in bytes

        public Vector4 position;
        public OpenTK.Graphics.Color4 color;

        public Vertex(Vector4 pos, OpenTK.Graphics.Color4 col)
        {
            position = pos;
            color = col;
        }
    }

}
