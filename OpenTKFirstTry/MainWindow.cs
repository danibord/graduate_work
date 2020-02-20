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
    public sealed class MainWindow : GameWindow
    {
        private int prog;
        //private int vertexarray;
        static private double time;
        private List<RenderObject> renderObjects = new List<RenderObject>();
        private List<string[]> Coordinates;
        private List<string[]> Faces;
        private int ColorId;
        private int Scaling;
        private int Vsmode;

        private Matrix4 modelView;
        private Matrix4 projectionMatrix;
        private OpenTK.Graphics.Color4 backcolor = new OpenTK.Graphics.Color4(0.1f, 0.1f, 0.3f, 1.0f);
        private Stopwatch TIMER;


        private float  always = -2.7f;
        private float fov = 60f;
        private float MaxFps = 0;
        private float MinFps = 100000;

        private int NumOfObjects;
        private int TypeOfParallel;
        private int TypeOfCalculation; // 0 - 
        private int TypeOfModel; // 0 - default, 1  - model.obj
        private int[] done;

        public delegate void ClearMem();
        public ClearMem mem;

        public bool isRunning = false;
        public bool exit_flag = false;

        private string filename;
        private int frameindicator = 0;
        private float[] fps = new float[1000];
        bool flag;
        private int[] timelast = new int[1000];



        public MainWindow(int num, int typeofcalc, int typeofparallel, int colorid, int scale, int model, int vs, string filename)
            :base(800, 
                 720,
                 OpenTK.Graphics.GraphicsMode.Default,
                 "Test of calculations",
                 GameWindowFlags.Default,
                 DisplayDevice.Default,
                 4,
                 0,
                 OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible)
        {
            Title += " : OpenGL Version : " + GL.GetString(StringName.Version);
            NumOfObjects = num;
            this.filename = filename;
            isRunning = true;
            TypeOfCalculation = typeofcalc;
            TypeOfModel = model;
            TypeOfParallel = typeofparallel;
            Coordinates = new List<string[]>(); 
            Faces = new List<string[]>();
            ColorId = colorid;
            Vsmode = vs;
            Scaling = scale;
            TIMER = new Stopwatch();
            if(TypeOfModel == 1)
            {
                FactoryOfObjects.LoadCustomModel(filename, Coordinates, Faces);
            }
           
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            CreateProjection();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Vsmode == 0)
                VSync = VSyncMode.Off;
            else
                VSync = VSyncMode.On;

            CreateProjection();
            for (int i = 0; i < NumOfObjects; i++)
            {
                if (TypeOfModel == 0)
                {
                    if(ColorId == 0) 
                            renderObjects.Add(new RenderObject(FactoryOfObjects.CreateCube(0.2f, OpenTK.Graphics.Color4.Red)));
                    if (ColorId == 1)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateCube(0.2f, OpenTK.Graphics.Color4.Blue)));
                    if (ColorId == 2)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateCube(0.2f, OpenTK.Graphics.Color4.White)));
                    if (ColorId == 3)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateCube(0.2f, OpenTK.Graphics.Color4.Black)));
                    if (ColorId == 4)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateCube(0.2f, OpenTK.Graphics.Color4.Azure)));
                    if (ColorId == 5)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateCube(0.2f, OpenTK.Graphics.Color4.Green)));
                    if (ColorId == 6)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateCube(0.2f, OpenTK.Graphics.Color4.Purple)));

                }
                else
                {
                    if (ColorId == 0)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateModel(Coordinates, Faces, OpenTK.Graphics.Color4.Red)));
                    if (ColorId == 1)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateModel(Coordinates, Faces, OpenTK.Graphics.Color4.Blue)));
                    if (ColorId == 2)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateModel(Coordinates, Faces, OpenTK.Graphics.Color4.White)));
                    if (ColorId == 3)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateModel(Coordinates, Faces, OpenTK.Graphics.Color4.Black)));
                    if (ColorId == 4)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateModel(Coordinates, Faces, OpenTK.Graphics.Color4.Azure)));
                    if (ColorId == 5)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateModel(Coordinates, Faces, OpenTK.Graphics.Color4.Green)));
                    if (ColorId == 6)
                        renderObjects.Add(new RenderObject(FactoryOfObjects.CreateModel(Coordinates, Faces, OpenTK.Graphics.Color4.Purple)));
                }
            }
            CursorVisible = true;

            done = new int[NumOfObjects];
            prog = CreateProgram();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            GL.Enable(EnableCap.DepthTest);
            Closed += OnClosed;
        }
    
        //Method for chacking updates of the window
        protected override void OnUpdateFrame(FrameEventArgs e)
        {  
            HandleKeyboard();
        }

        //Method which enables user to change some parametrs of the window
        private void HandleKeyboard()
        {
            var keystate = OpenTK.Input.Keyboard.GetState();
            if(keystate.IsKeyDown(OpenTK.Input.Key.Escape) || exit_flag == true)
            {
                Exit();
            }
            if (keystate.IsKeyDown(OpenTK.Input.Key.M))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
            }
            if (keystate.IsKeyDown(OpenTK.Input.Key.L))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            if (keystate.IsKeyDown(OpenTK.Input.Key.F))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }

            if (keystate.IsKeyDown(OpenTK.Input.Key.W))
            {
                if (fov <= 150)
                {
                    fov = fov + 5f;
                    CreateProjection();
                } 
            }
            if (keystate.IsKeyDown(OpenTK.Input.Key.S))
            {
                fov = 60f;
                CreateProjection();
            }
            if (keystate.IsKeyDown(OpenTK.Input.Key.Z))
            {
                fov = 120f;
                CreateProjection();
            }

            if (keystate.IsKeyDown(OpenTK.Input.Key.B))
            {
                if( fov != 5 )
                    fov = fov - 5f;
                if (fov > 0)
                {
                    CreateProjection();
                }
            }

        }

        //Common method for creation of projection
        private void CreateProjection()
        {
            var aspectRatio = (float)Width / Height;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                fov * ((float)Math.PI / 180f),
                aspectRatio,
                0.001f,
                4000f);
        }

        //Method for closing created window
        private void OnClosed(object sender, EventArgs eventArgs)
        {
            Exit();
            mem?.Invoke();
        }

        //Method for disposing memory 
        public override void Exit()
        {
            System.Diagnostics.Debug.WriteLine("Exit called");
            foreach (var obj in renderObjects)
                obj.Dispose();

            GL.DeleteProgram(prog);
            
            base.Exit();
        }

        
        
        
        //Method for rendering objects 
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            fps[frameindicator] = Convert.ToSingle(1f / e.Time);
            timelast[frameindicator] = (int)TIMER.Elapsed.Ticks;
            frameindicator++;
            if(!flag)
                flag = (frameindicator >= fps.Length) ? true:false;
            frameindicator = frameindicator % fps.Length;
            //float avereg = fps.Sum() / fps.Length;
            float avereg = (flag) ? fps.Sum() / fps.Length : fps.Sum() / frameindicator;
            int averagetime = (flag) ? timelast.Sum() / timelast.Length : timelast.Sum() / frameindicator;
            time += e.Time;
            if ((1f / e.Time) > MaxFps)
                MaxFps = Convert.ToSingle(1f / e.Time);
            if ((1f / e.Time) < MinFps)
                MinFps = Convert.ToSingle(1f / e.Time);
          

            Title = $"(Vsync: {VSync})Fps: {1f/e.Time :0}  FPS average : {avereg :0} MaxFps:{MaxFps:0} MinFps: {MinFps:0} Processors tics {TIMER.Elapsed.Ticks:0} Ticks average {averagetime :0}";
            GL.ClearColor(backcolor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    

            GL.UseProgram(prog);
            GL.UniformMatrix4(20, false, ref projectionMatrix);
            float diff = 0.007f;


            //foreach (var renderObject in renderObjects)
            Stopwatch Timer = new Stopwatch();
            Timer.Restart();
            if (TypeOfCalculation == 2)
            {
                
                if (TypeOfParallel == 0)
                {
                    //Stopwatch Timer = new Stopwatch();
                    //Timer.Restart();
                    Parallel.For(0, renderObjects.Count, index =>
                    {
                        if (done[index] == 0)
                            done[index]++;
                        if (done[index] == 3)
                            done[index] = 0;
                        renderObjects[(int)index].Bind();
                        // c = 0.3
                        float c = 0.007f * index;
                    //for (int i = 0; i < NumOfObjects; i++)
                    //{
                    var k = index + (float)(time * (0.07f + (0.12 * c)));
                        var t2 = Matrix4.CreateTranslation(
                        (float)(Math.Sin(k * 5f) * (c + 0.55f)),
                        (float)(Math.Cos(k * 5f) * (c + 0.55f)),
                        always);
                        Matrix4 r1 = new Matrix4();
                        Matrix4 r2 = new Matrix4();
                        Matrix4 r3 = new Matrix4();


                        r1 = Matrix4.CreateRotationX(k * 12.0f + index);
                        r2 = Matrix4.CreateRotationY(k * 12.0f + index);
                        r3 = Matrix4.CreateRotationZ(k * 7.0f + index);
                        Matrix4 modelView;
                        modelView = r1 * r2 * r3 * t2;
                        if (Scaling > 1)
                        {
                            if (done[index] < 3)
                            {
                                float k1 = modelView.M11 * Scaling;
                                float k2 = modelView.M22 * Scaling;
                                float k3 = modelView.M33 * Scaling;
                                modelView.M11 = k1;
                                modelView.M22 = k2;
                                modelView.M33 = k3;
                            }
                            else
                            {
                                float k1 = modelView.M11 * (1 / Scaling);
                                float k2 = modelView.M22 * (1 / Scaling);
                                float k3 = modelView.M33 * (1 / Scaling);
                                modelView.M11 = k1;
                                modelView.M22 = k2;
                                modelView.M33 = k3;
                            }
                        }
                      

                        GL.UniformMatrix4(21, false, ref modelView);
                        renderObjects[(int)index].Render();
                    //renderObject.Render();
                    //}
                    });
                    //Timer.Stop();
                    //TIMER = Timer;
                }

                if(TypeOfParallel == 1)
                {
                    //Stopwatch Timer = new Stopwatch();
                    //Timer.Restart();
                    Task[] Tasks = new Task[renderObjects.Count];
                    Matrix4[] mv = new Matrix4[renderObjects.Count];
                    for(int i =0; i <renderObjects.Count; i++)
                    {
                        int t = i;
                        Tasks[t] = new Task(() => Randering(renderObjects, t, ref mv[t]));
                        Tasks[t].Start();
                    }
                    Task.WaitAll(Tasks);
                    for(int i  = 0; i < renderObjects.Count; i++)
                    {
                        GL.UniformMatrix4(21, false, ref mv[i]);
                        renderObjects[(int)i].Render();
                    }

                    //for (int i = 0; i < renderObjects.Count; i+=4)
                    //{
                    //    //if (i + 4 <= renderObjects.Count)
                    //    //{
                    //    //   Tasks = new Task[4];
                    //    //    Matrix4[] m = new Matrix4[4];
                    //    //    for (int j = 0; j < 4; j++)
                    //    //    {
                    //    //        int numb = i + j;
                    //    //        int t = j;
                    //    //        Tasks[j] = new Task(() => Randering( renderObjects, numb, ref m[t]));
                    //    //        Tasks[j].Start();
                    //    //    }
                    //    //    Task.WaitAll(Tasks);
                    //    //    for (int k = 0; k < 4; k++)
                    //    //    {
                    //    //        GL.UniformMatrix4(21, false, ref m[k]);
                    //    //        renderObjects[(int)i + k].Render();
                    //    //    }
                    //    //}
                    //    //else
                    //    //{
                    //    //    Tasks = new Task[renderObjects.Count - i];
                    //    //   Matrix4[] m = new Matrix4[renderObjects.Count - i];
                    //    //    for (int j = 0; j < renderObjects.Count - i; j++)
                    //    //    {
                    //    //        int numb = i + j;
                    //    //        int t = j;
                    //    //        Tasks[j] = new Task(() => Randering( renderObjects, numb, ref m[t]));
                    //    //        Tasks[j].Start();
                    //    //    }
                    //    //    Task.WaitAll(Tasks);
                    //    //    for (int k = 0; k < renderObjects.Count - i; k++)
                    //    //    {
                    //    //        GL.UniformMatrix4(21, false, ref m[k]);
                    //    //        renderObjects[(int)i + k].Render();
                    //    //    }
                    //    //}
                    //}
                    //Timer.Stop();
                    //TIMER = Timer;
                }


                if(TypeOfParallel == 3)
                {
                    WaitHandle[] mas = new WaitHandle[4];
                    Matrix4[] mv = new Matrix4[4];
                    for (int i = 0; i < renderObjects.Count; i += 4)
                    {
                        
                        if (i + 4 <= renderObjects.Count)
                        {
                            
                            int flag = 4;
                            for (int j = 0; j < 4; j++)
                            {
                                int numb = i + j;
                                int t = j;
                                mas[t] = new AutoResetEvent(false);
                                ThreadRend rend = new ThreadRend(ref mas[t], ref renderObjects, ref mv[t], i + t);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(RanderingThread), rend);
                            }
                            while(flag > 4)
                            {
                                WaitHandle.WaitAny(mas);
                                flag--;
                            }
                            for (int k = 0; k < 4; k++)
                            {
                                GL.UniformMatrix4(21, false, ref mv[k]);
                                renderObjects[(int)i + k].Render();
                            }
                        }
                        else
                        {
                           
                            mas = new WaitHandle[renderObjects.Count - i];
                            int flag = renderObjects.Count - i;
                            mv = new Matrix4[renderObjects.Count - i];
                            for (int j = 0; j < renderObjects.Count - i; j++)
                            {
                                int numb = i + j;
                                int t = j;
                                mas[t] = new AutoResetEvent(false);
                                ThreadRend rend = new ThreadRend(ref mas[t], ref renderObjects, ref mv[t], i + t);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(RanderingThread),  rend);
                            }
                            while (flag > renderObjects.Count - i)
                            {
                                WaitHandle.WaitAny(mas);
                                flag--;
                            }
                            for (int k = 0; k < renderObjects.Count - i; k++)
                            {
                                GL.UniformMatrix4(21, false, ref mv[k]);
                                renderObjects[(int)i + k].Render();
                            }
                        }
                    }
                }
            }
            else
            {
                //Stopwatch Timer = new Stopwatch();
                //Timer.Restart();
                for (int i = 0; i < renderObjects.Count; i++)
                {
                    //  Randering(ref renderObjects, i);
                    if (done[i] == 0)
                        done[i]++;
                    if (done[i] == 3)
                        done[i] = 0;
                    renderObjects[i].Bind();

                    //HERE GOOOOOOOOOD CODE STARTS
                    //for (int i = 0; i < NumOfObjects; i++)
                    //{
                    var change = 1 * i + (float)(time * (0.07f + (0.12 * diff * i)));
                    var trans = Matrix4.CreateTranslation(
                        (float)(Math.Sin(change * 5f) * (diff * i + 0.55f)),
                        (float)(Math.Cos(change * 5f) * (diff * i + 0.55f)),
                        always);
                    Matrix4 rx = new Matrix4();
                    Matrix4 ry = new Matrix4();
                    Matrix4 rz = new Matrix4();

                    if (TypeOfCalculation == 0)
                    {
                        rx = Matrix4.CreateRotationX(change * 12.0f + i);
                        ry = Matrix4.CreateRotationY(change * 12.0f + i);
                        rz = Matrix4.CreateRotationZ(change * 7.0f + i);
                        Stopwatch timer = new Stopwatch();
                        timer.Restart();
                        modelView = rx * ry * rz * trans;
        
                    }
                    else
                    {
                        rx = Matrix4.CreateRotationX(change * 12.0f + i);
                        ry = Matrix4.CreateRotationY(change * 12.0f + i);
                        rz = Matrix4.CreateRotationZ(change * 7.0f + i);

                        // start
                        

                        Matrix4 temp1 = Calculations.ParallelMultiply(rx, ry, TypeOfParallel);
                        Matrix4 temp2 = Calculations.ParallelMultiply(temp1, rz, TypeOfParallel);
                        modelView = Calculations.ParallelMultiply(temp2, trans, TypeOfParallel);

                       
                        //finish  
                    }
                    // HERE GOOOOOOOD CODE ENDS
                    if (Scaling > 1)
                    {
                        if (done[i] < 3)
                        {
                            float k1 = modelView.M11 * Scaling;
                            float k2 = modelView.M22 * Scaling;
                            float k3 = modelView.M33 * Scaling;
                            modelView.M11 = k1;
                            modelView.M22 = k2;
                            modelView.M33 = k3;
                        }
                        else
                        {
                            float k1 = modelView.M11 * (1 / Scaling);
                            float k2 = modelView.M22 * (1 / Scaling);
                            float k3 = modelView.M33 * (1 / Scaling);
                            modelView.M11 = k1;
                            modelView.M22 = k2;
                            modelView.M33 = k3;
                        }
                    }
                    ////IT WAS WORKING NORMALLY!!!!!!
                    GL.UniformMatrix4(21, false, ref modelView);
                    renderObjects[i].Render();
                    //Timer.Stop();
                    //TIMER = Timer;
                    //}
                    //diff += 0.35f;
                } 
            }
            Timer.Stop();
            TIMER = Timer;
            SwapBuffers();
        }

        //Method for creation of common rotation Matirces
        private void CreateRotation(int type, object angle, ref Matrix4 Rot)
        {
            switch (type)
            {
                case 1:
                    Rot = Matrix4.CreateRotationX((float)angle);
                    break;

                case 2:
                    Rot = Matrix4.CreateRotationY((float)angle);
                    break;

                case 3:
                    Rot = Matrix4.CreateRotationZ((float)angle);
                    break;
                    
            }
        }

        //Method for creation of shaders
        private int CompileShader(ShaderType type,string path)
        {
            var shader = GL.CreateShader(type);
            var src = System.IO.File.ReadAllText(path);
            GL.ShaderSource(shader, src);
            GL.CompileShader(shader);
            var report = GL.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(report))
                System.Diagnostics.Debug.WriteLine($"Gl.CompileShader [{type}] had info log: [{report}]");
            return shader;
        }

        static public void Randering(  List<RenderObject> renderObjects, int index, ref Matrix4 modelView)
        {
            renderObjects[(int)index].Bind();
            float c = 0.007f;
            //for (int i = 0; i < NumOfObjects; i++)
            //{
            var k = index + (float)(time * (0.07f + (0.12 * c * index)));
            var t2 = Matrix4.CreateTranslation(
                (float)(Math.Sin(k * 5f) * (c + 0.55f)),
                (float)(Math.Cos(k * 5f) * (c + 0.55f)),
                -2.7f);
            Matrix4 r1 = new Matrix4();
            Matrix4 r2 = new Matrix4();
            Matrix4 r3 = new Matrix4();


            r1 = Matrix4.CreateRotationX(k * 12.0f + index);
            r2 = Matrix4.CreateRotationY(k * 12.0f + index);
            r3 = Matrix4.CreateRotationZ(k * 7.0f + index);
          //  Matrix4 modelView;
            modelView = r1 * r2 * r3 * t2;
            //GL.UniformMatrix4(21, false, ref modelView);
            //renderObjects[(int)index].Render();
        }


        static public void RanderingThread( object obj)
        {
            if (obj is ThreadRend)
            {
                ThreadRend Temp = (ThreadRend)obj;
                Temp.obj[Temp.index].Bind();
                float c = 0.7f * Temp.index;
                //for (int i = 0; i < NumOfObjects; i++)
                //{
                var k = Temp.index + (float)(time * (0.07f + (0.12 * c)));
                var t2 = Matrix4.CreateTranslation(
                    (float)(Math.Sin(k * 5f) * (c + 0.55f)),
                    (float)(Math.Cos(k * 5f) * (c + 0.55f)),
                    -2.7f);
                Matrix4 r1 = new Matrix4();
                Matrix4 r2 = new Matrix4();
                Matrix4 r3 = new Matrix4();
                r1 = Matrix4.CreateRotationX(k * 12.0f + Temp.index);
                r2 = Matrix4.CreateRotationY(k * 12.0f + Temp.index);
                r3 = Matrix4.CreateRotationZ(k * 7.0f + Temp.index);
                Temp.view = r1 * r2 * r3 * t2;
            }
        }


        //Method for creation of object to which will be attached
        private int CreateProgram()
        {
            try
            {
                var program = GL.CreateProgram();
                var shaders = new List<int>();
                shaders.Add(CompileShader(ShaderType.VertexShader, @"vertexShader2.txt"));
                shaders.Add(CompileShader(ShaderType.FragmentShader, @"fragmentShader2.txt"));

                foreach (var shader in shaders)
                    GL.AttachShader(program, shader);
                GL.LinkProgram(program);
                var info = GL.GetProgramInfoLog(program);
                if (!string.IsNullOrWhiteSpace(info))
                    System.Diagnostics.Debug.WriteLine($"Gl.Linkprogram had info log: [{info}]");

                foreach (var shader in shaders)
                {
                    GL.DetachShader(program, shader);
                    GL.DeleteShader(shader);
                }
                return program;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw;
            }
        }

   
    }
}
