using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Microsoft.Xna;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;

namespace OpenTKFirstTry
{
    class ThreadMatrices
    {
        public Matrix4 Left;
        public Matrix4 Right;
        public Matrix4 Result;
        public int i;
        public bool flag;
        public WaitHandle Wait;

        public ThreadMatrices(Matrix4 left, Matrix4 right, int count, WaitHandle wait)
        {
            Left = left;
            Right = right;
            Result = new Matrix4();
            i = count;
            flag = false;
            Wait = wait;

        }
    }
}
