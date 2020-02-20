using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class ThreadRend
    {
        public WaitHandle Wait;
        public List<RenderObject> obj;
        public Matrix4 view;
        public int index;

        public ThreadRend (ref WaitHandle WAIT, ref List<RenderObject> Obj, ref Matrix4 Modelview, int ind)
        {
            Wait = WAIT;
            obj = Obj;
            view = Modelview;
            index = ind;
        }
    }
}
