using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Microsoft.Xna;
using System.Windows.Forms;

namespace OpenTKFirstTry
{
    class FactoryOfObjects
    {
        public static Vertex[] CreateCube(float side, OpenTK.Graphics.Color4 color)
        {
            side = side / 2f;
            Vertex[] vertices =
            {
                new Vertex(new Vector4(-side, -side, -side, 1.0f), color),
                new Vertex(new Vector4(-side, side, -side, 1.0f), color),
                new Vertex(new Vector4(-side, -side, side, 1.0f), color),
                new Vertex(new Vector4(-side, -side, side, 1.0f), color),
                new Vertex(new Vector4(-side, side, -side, 1.0f), color),
                new Vertex(new Vector4(-side, side, side, 1.0f), color),

                new Vertex(new Vector4(side, -side, -side, 1.0f), color),
                new Vertex(new Vector4(side, side, -side, 1.0f), color),
                new Vertex(new Vector4(side, -side, side, 1.0f), color),
                new Vertex(new Vector4(side, -side, side, 1.0f), color),
                new Vertex(new Vector4(side, side, -side, 1.0f), color),
                new Vertex(new Vector4(side, side, side, 1.0f), color),

                new Vertex(new Vector4(-side, -side, -side, 1.0f), color),
                new Vertex(new Vector4(side, -side, -side, 1.0f), color),
                new Vertex(new Vector4(-side, -side, side, 1.0f), color),
                new Vertex(new Vector4(-side, -side, side, 1.0f), color),
                new Vertex(new Vector4(side, -side, -side, 1.0f), color),
                new Vertex(new Vector4(side, -side, side, 1.0f), color),

                new Vertex(new Vector4(-side, side, -side, 1.0f), color),
                new Vertex(new Vector4(side, side, -side, 1.0f), color),
                new Vertex(new Vector4(-side, side, side, 1.0f), color),
                new Vertex(new Vector4(-side, side, side, 1.0f), color),
                new Vertex(new Vector4(side, side, -side, 1.0f), color),
                new Vertex(new Vector4(side, side, side, 1.0f), color),

                new Vertex(new Vector4(-side, -side, -side, 1.0f), color),
                new Vertex(new Vector4(side, -side, -side, 1.0f), color),
                new Vertex(new Vector4(-side, side, -side, 1.0f), color),
                new Vertex(new Vector4(-side, side, -side, 1.0f), color),
                new Vertex(new Vector4(side, -side, -side, 1.0f), color),
                new Vertex(new Vector4(side, side, -side, 1.0f), color),

                new Vertex(new Vector4(-side, -side, side, 1.0f), color),
                new Vertex(new Vector4(side, -side, side, 1.0f), color),
                new Vertex(new Vector4(-side, side, side, 1.0f), color),
                new Vertex(new Vector4(-side, side, side, 1.0f), color),
                new Vertex(new Vector4(side, -side, side, 1.0f), color),
                new Vertex(new Vector4(side, side, side, 1.0f), color),
        };
            return vertices;
        }

        //Method for downloading an info about vertices and faces of an object
        static public void LoadCustomModel(string filename, List<string[]> Coordinates, List<string[]> Faces)
        {
            StreamReader fileload = new StreamReader(filename);
            string str = fileload.ReadLine();


            while (str != null)
            {
                while (str[0] != 'v')
                {
                    str = fileload.ReadLine();
                }

                while (str[0] == 'v')
                {
                    String[] s = str.Split(' ');
                    string[] data = new string[3];
                    for (int i = 0; i < 3; i++)
                    {
                        data[i] = s[i + 1].Replace('.', ',');
                    }

                    Coordinates.Add(data);
                    str = fileload.ReadLine();
                }

                while (str[0] != 'f')
                {
                    str = fileload.ReadLine();
                }

                while (str != null && str[0] == 'f')
                {
                    String[] s = str.Split(' ');
                    string[] data = new string[3];
                    for (int i = 0; i < 3; i++)
                    {
                        data[i] = s[i + 1];
                    }
                    Faces.Add(data);
                    str = fileload.ReadLine();
                }
            }
            fileload.Close();
        }

        public static Vertex[] CreateModel(List<string[]> Coordinates, List<string[]> Faces, OpenTK.Graphics.Color4 color)
        {
            Vertex[] VerticesOfModel = new Vertex[Faces.Count * 3];
            int k = 0;

            for (int i = 0; i < Faces.Count(); i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int index = Convert.ToInt32(Faces[i][j]) - 1;
                    float x = Convert.ToSingle(Coordinates[index][0]);
                    float y = Convert.ToSingle(Coordinates[index][1]);
                    float z = Convert.ToSingle(Coordinates[index][2]);
                    VerticesOfModel[k] = new Vertex(new Vector4(x, y, z, 1), color);
                    k++;
                }
            }


            return VerticesOfModel;
        }
    }
}
