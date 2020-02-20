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
    class Calculations
    {

        delegate Matrix4 AsyncMult(int n, Matrix4 left, Matrix4 right);
        static private AsyncMult Mult0 = new AsyncMult(AsynMult);
        static private AsyncMult Mult1 = new AsyncMult(AsynMult);
        static private AsyncMult Mult2 = new AsyncMult(AsynMult);
        static private AsyncMult Mult3 = new AsyncMult(AsynMult);

        static WaitHandle[] waitHandles = new WaitHandle[]
        {
            new AutoResetEvent(false),
            new AutoResetEvent(false)
        };

        // Main method which is resposnable for switching and launching different types of calculation
        static public Matrix4 ParallelMultiply(Matrix4 Left, Matrix4 Right, int TypeOfParallel)
        {
            Matrix4 Result = new Matrix4();

            if (TypeOfParallel == 0)
            {
                Parallel.For(0, 4, index =>
                {
                    switch (index)
                    {
                        case 0:
                            Result.M11 = Left.M11 * Right.M11 + Left.M12 * Right.M21 + Left.M13 * Right.M31 + Left.M14 * Right.M41;
                            Result.M12 = Left.M11 * Right.M12 + Left.M12 * Right.M22 + Left.M13 * Right.M32 + Left.M14 * Right.M42;
                            Result.M13 = Left.M11 * Right.M13 + Left.M12 * Right.M23 + Left.M13 * Right.M33 + Left.M14 * Right.M43;
                            Result.M14 = Left.M11 * Right.M14 + Left.M12 * Right.M24 + Left.M13 * Right.M34 + Left.M14 * Right.M44;
                            break;

                        case 1:
                            Result.M21 = Left.M21 * Right.M11 + Left.M22 * Right.M21 + Left.M23 * Right.M31 + Left.M24 * Right.M41;
                            Result.M22 = Left.M21 * Right.M12 + Left.M22 * Right.M22 + Left.M23 * Right.M32 + Left.M24 * Right.M42;
                            Result.M23 = Left.M21 * Right.M13 + Left.M22 * Right.M23 + Left.M23 * Right.M33 + Left.M24 * Right.M43;
                            Result.M24 = Left.M21 * Right.M14 + Left.M22 * Right.M24 + Left.M23 * Right.M34 + Left.M24 * Right.M44;
                            break;

                        case 2:
                            Result.M31 = Left.M31 * Right.M11 + Left.M32 * Right.M21 + Left.M33 * Right.M31 + Left.M34 * Right.M41;
                            Result.M32 = Left.M31 * Right.M12 + Left.M32 * Right.M22 + Left.M33 * Right.M32 + Left.M34 * Right.M42;
                            Result.M33 = Left.M31 * Right.M13 + Left.M32 * Right.M23 + Left.M33 * Right.M33 + Left.M34 * Right.M43;
                            Result.M34 = Left.M31 * Right.M14 + Left.M32 * Right.M24 + Left.M33 * Right.M34 + Left.M34 * Right.M44;
                            break;

                        case 3:
                            Result.M41 = Left.M41 * Right.M11 + Left.M42 * Right.M21 + Left.M43 * Right.M31 + Left.M44 * Right.M41;
                            Result.M42 = Left.M41 * Right.M12 + Left.M42 * Right.M22 + Left.M43 * Right.M32 + Left.M44 * Right.M42;
                            Result.M43 = Left.M41 * Right.M13 + Left.M42 * Right.M23 + Left.M43 * Right.M33 + Left.M44 * Right.M43;
                            Result.M44 = Left.M41 * Right.M14 + Left.M42 * Right.M24 + Left.M43 * Right.M34 + Left.M44 * Right.M44;
                            break;

                    }
                });
                return Result;
            }

            if (TypeOfParallel == 1)
            {
                Task[] Tasks = new Task[4];
                for (int i = 0; i < 4; i++)
                {
                    int numb = i;
                    Tasks[i] = new Task(() => TaskMult(numb, ref Left, ref Right, ref Result));
                }
                foreach (var t in Tasks)
                {
                    t.Start();
                }
                Task.WaitAll(Tasks);
                return Result;
            }

            if (TypeOfParallel == 2)
            {
                return AMult(Left, Right);
            }

            if(TypeOfParallel== 3)
            {
                WaitHandle[] mas = new WaitHandle[4];
           
                Matrix4 result = new Matrix4();
                Thread[] T = new Thread[4];
                mas[0] = new AutoResetEvent(false);
                mas[1] = new AutoResetEvent(false);
                mas[2] = new AutoResetEvent(false);
                mas[3] = new AutoResetEvent(false);
                int index = 4;
                ThreadMatrices Matrices = new ThreadMatrices(Left, Right, 0, mas[0]);
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadMult), Matrices);
         //       T[0] = new Thread(new ParameterizedThreadStart(ThreadMult));
           //     T[0].Start(Matrices);
                ThreadMatrices Matrices1 = new ThreadMatrices(Left, Right, 1, mas[1]);
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadMult), Matrices1);
                //     T[1] = new Thread(new ParameterizedThreadStart(ThreadMult));
                //     T[1].Start(Matrices1);
                ThreadMatrices Matrices2 = new ThreadMatrices(Left, Right, 2, mas[2]);
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadMult), Matrices2);
                //  T[2] = new Thread(new ParameterizedThreadStart(ThreadMult));
                //   T[2].Start(Matrices2);
                ThreadMatrices Matrices3 = new ThreadMatrices(Left, Right, 3, mas[3]);
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadMult), Matrices3);
                // T[3] = new Thread(new ParameterizedThreadStart(ThreadMult));
                //   T[3].Start(Matrices3);
                int[] check = new int[4];

                while (index > 0)
                {
                    WaitHandle.WaitAny(mas);
                    index--;
                }

                //   WaitHandle.WaitAll(new WaitHandle[] {mas[0],mas[1],mas[2],mas[3]});

                //while (check.Sum() != 4)
                ////while (t0.IsAlive || t1.IsAlive || t2.IsAlive || t3.IsAlive == true)
                //{
                //    if (check[0] == 0 && T[0].IsAlive == false)
                //    {
                //        check[0] = 1;
                //        result += Matrices.Result;
                //    }
                //    if (check[1] == 0 && T[1].IsAlive == false)
                //    {
                //        check[1] = 1;
                //        result += Matrices1.Result;
                //    }
                //    if (check[2] == 0 && T[2].IsAlive == false)
                //    {
                //        check[2] = 1;
                //        result += Matrices2.Result;
                //    }
                //    if (check[3] == 0 && T[3].IsAlive == false)
                //    {
                //        check[3] = 1;
                //        result += Matrices3.Result;
                //    }
                //}
                return result = Matrices.Result + Matrices1.Result + Matrices2.Result + Matrices3.Result;

                //for(int i = 0; i < 4; i++)
                //{
                //    new Thread(() =>
                //    {
                //        switch (i)
                //        {
                //            case 0:
                //                Result.M11 = Left.M11 * Right.M11 + Left.M12 * Right.M21 + Left.M13 * Right.M31 + Left.M14 * Right.M41;
                //                Result.M12 = Left.M11 * Right.M12 + Left.M12 * Right.M22 + Left.M13 * Right.M32 + Left.M14 * Right.M42;
                //                Result.M13 = Left.M11 * Right.M13 + Left.M12 * Right.M23 + Left.M13 * Right.M33 + Left.M14 * Right.M43;
                //                Result.M14 = Left.M11 * Right.M14 + Left.M12 * Right.M24 + Left.M13 * Right.M34 + Left.M14 * Right.M44;
                //                break;

                //            case 1:
                //                Result.M21 = Left.M21 * Right.M11 + Left.M22 * Right.M21 + Left.M23 * Right.M31 + Left.M24 * Right.M41;
                //                Result.M22 = Left.M21 * Right.M12 + Left.M22 * Right.M22 + Left.M23 * Right.M32 + Left.M24 * Right.M42;
                //                Result.M23 = Left.M21 * Right.M13 + Left.M22 * Right.M23 + Left.M23 * Right.M33 + Left.M24 * Right.M43;
                //                Result.M24 = Left.M21 * Right.M14 + Left.M22 * Right.M24 + Left.M23 * Right.M34 + Left.M24 * Right.M44;
                //                break;

                //            case 2:
                //                Result.M31 = Left.M31 * Right.M11 + Left.M32 * Right.M21 + Left.M33 * Right.M31 + Left.M34 * Right.M41;
                //                Result.M32 = Left.M31 * Right.M12 + Left.M32 * Right.M22 + Left.M33 * Right.M32 + Left.M34 * Right.M42;
                //                Result.M33 = Left.M31 * Right.M13 + Left.M32 * Right.M23 + Left.M33 * Right.M33 + Left.M34 * Right.M43;
                //                Result.M34 = Left.M31 * Right.M14 + Left.M32 * Right.M24 + Left.M33 * Right.M34 + Left.M34 * Right.M44;
                //                break;

                //            case 3:
                //                Result.M41 = Left.M41 * Right.M11 + Left.M42 * Right.M21 + Left.M43 * Right.M31 + Left.M44 * Right.M41;
                //                Result.M42 = Left.M41 * Right.M12 + Left.M42 * Right.M22 + Left.M43 * Right.M32 + Left.M44 * Right.M42;
                //                Result.M43 = Left.M41 * Right.M13 + Left.M42 * Right.M23 + Left.M43 * Right.M33 + Left.M44 * Right.M43;
                //                Result.M44 = Left.M41 * Right.M14 + Left.M42 * Right.M24 + Left.M43 * Right.M34 + Left.M44 * Right.M44;
                //                break;
                //        }
                //    });

                //}
                //WaitHandle.WaitAll();
                // return Result;
            }

            return Left * Right;
        }

        static public void ThreadMult( object obj)
        {
            if(obj is ThreadMatrices)
            {
                ThreadMatrices Temp = (ThreadMatrices)obj;
                if(Temp.i == 0)
                {
                    Matrix4 Left = Temp.Left;
                    Matrix4 Right = Temp.Right;
                    Temp.Result.M11 = Left.M11 * Right.M11 + Left.M12 * Right.M21 + Left.M13 * Right.M31 + Left.M14 * Right.M41;
                   // Temp.flag[0] = 1;
                    Temp.Result.M12 = Left.M11 * Right.M12 + Left.M12 * Right.M22 + Left.M13 * Right.M32 + Left.M14 * Right.M42;
                   // Temp.flag[1] = 1;
                    Temp.Result.M13 = Left.M11 * Right.M13 + Left.M12 * Right.M23 + Left.M13 * Right.M33 + Left.M14 * Right.M43;
                  //  Temp.flag[2] = 1;
                    Temp.Result.M14 = Left.M11 * Right.M14 + Left.M12 * Right.M24 + Left.M13 * Right.M34 + Left.M14 * Right.M44;
                    // Temp.flag[3] = 1;
                    Temp.flag = true;
                }

                if (Temp.i == 1)
                {
                    Matrix4 Left = Temp.Left;
                    Matrix4 Right = Temp.Right;
                    Temp.Result.M21 = Left.M21 * Right.M11 + Left.M22 * Right.M21 + Left.M23 * Right.M31 + Left.M24 * Right.M41;
                    //Temp.flag[4] = 1;
                    Temp.Result.M22 = Left.M21 * Right.M12 + Left.M22 * Right.M22 + Left.M23 * Right.M32 + Left.M24 * Right.M42;
                    //Temp.flag[5] = 1;
                    Temp.Result.M23 = Left.M21 * Right.M13 + Left.M22 * Right.M23 + Left.M23 * Right.M33 + Left.M24 * Right.M43;
                    //Temp.flag[6] = 1;
                    Temp.Result.M24 = Left.M21 * Right.M14 + Left.M22 * Right.M24 + Left.M23 * Right.M34 + Left.M24 * Right.M44;
                    //Temp.flag[7] = 1;
                    Temp.flag = true;
                }

                if (Temp.i == 2)
                {
                    Matrix4 Left = Temp.Left;
                    Matrix4 Right = Temp.Right;
                    Temp.Result.M31 = Left.M31 * Right.M11 + Left.M32 * Right.M21 + Left.M33 * Right.M31 + Left.M34 * Right.M41;
                  //  Temp.flag[8] = 1;
                    Temp.Result.M32 = Left.M31 * Right.M12 + Left.M32 * Right.M22 + Left.M33 * Right.M32 + Left.M34 * Right.M42;
                  //  Temp.flag[9] = 1;
                    Temp.Result.M33 = Left.M31 * Right.M13 + Left.M32 * Right.M23 + Left.M33 * Right.M33 + Left.M34 * Right.M43;
                  //  Temp.flag[10] = 1;
                    Temp.Result.M34 = Left.M31 * Right.M14 + Left.M32 * Right.M24 + Left.M33 * Right.M34 + Left.M34 * Right.M44;
                    // Temp.flag[11] = 1;
                    Temp.flag = true;
                }

                if (Temp.i == 3)
                {
                    Matrix4 Left = Temp.Left;
                    Matrix4 Right = Temp.Right;
                    Temp.Result.M41 = Left.M41 * Right.M11 + Left.M42 * Right.M21 + Left.M43 * Right.M31 + Left.M44 * Right.M41;
                   // Temp.flag[12] = 1;
                    Temp.Result.M42 = Left.M41 * Right.M12 + Left.M42 * Right.M22 + Left.M43 * Right.M32 + Left.M44 * Right.M42;
                   // Temp.flag[13] = 1;
                    Temp.Result.M43 = Left.M41 * Right.M13 + Left.M42 * Right.M23 + Left.M43 * Right.M33 + Left.M44 * Right.M43;
                  //  Temp.flag[14] = 1;
                    Temp.Result.M44 = Left.M41 * Right.M14 + Left.M42 * Right.M24 + Left.M43 * Right.M34 + Left.M44 * Right.M44;
                    // Temp.flag[15] = 1;
                    Temp.flag = true;
                }
                ((AutoResetEvent)Temp.Wait).Set();
                return;
            }
        }

        static public Matrix4 AsynMult(int i, Matrix4 Left, Matrix4 Right)
        {
            Matrix4 Result = new Matrix4();
            if (i == 0)
            {
                Result.M11 = Left.M11 * Right.M11 + Left.M12 * Right.M21 + Left.M13 * Right.M31 + Left.M14 * Right.M41;
                Result.M12 = Left.M11 * Right.M12 + Left.M12 * Right.M22 + Left.M13 * Right.M32 + Left.M14 * Right.M42;
                Result.M13 = Left.M11 * Right.M13 + Left.M12 * Right.M23 + Left.M13 * Right.M33 + Left.M14 * Right.M43;
                Result.M14 = Left.M11 * Right.M14 + Left.M12 * Right.M24 + Left.M13 * Right.M34 + Left.M14 * Right.M44;
                return Result;
            }
            if (i == 1)
            {
                Result.M21 = Left.M21 * Right.M11 + Left.M22 * Right.M21 + Left.M23 * Right.M31 + Left.M24 * Right.M41;
                Result.M22 = Left.M21 * Right.M12 + Left.M22 * Right.M22 + Left.M23 * Right.M32 + Left.M24 * Right.M42;
                Result.M23 = Left.M21 * Right.M13 + Left.M22 * Right.M23 + Left.M23 * Right.M33 + Left.M24 * Right.M43;
                Result.M24 = Left.M21 * Right.M14 + Left.M22 * Right.M24 + Left.M23 * Right.M34 + Left.M24 * Right.M44;
                return Result;
            }

            if (i == 2)
            {
                Result.M31 = Left.M31 * Right.M11 + Left.M32 * Right.M21 + Left.M33 * Right.M31 + Left.M34 * Right.M41;
                Result.M32 = Left.M31 * Right.M12 + Left.M32 * Right.M22 + Left.M33 * Right.M32 + Left.M34 * Right.M42;
                Result.M33 = Left.M31 * Right.M13 + Left.M32 * Right.M23 + Left.M33 * Right.M33 + Left.M34 * Right.M43;
                Result.M34 = Left.M31 * Right.M14 + Left.M32 * Right.M24 + Left.M33 * Right.M34 + Left.M34 * Right.M44;
                return Result;
            }

            if (i == 3)
            {
                Result.M41 = Left.M41 * Right.M11 + Left.M42 * Right.M21 + Left.M43 * Right.M31 + Left.M44 * Right.M41;
                Result.M42 = Left.M41 * Right.M12 + Left.M42 * Right.M22 + Left.M43 * Right.M32 + Left.M44 * Right.M42;
                Result.M43 = Left.M41 * Right.M13 + Left.M42 * Right.M23 + Left.M43 * Right.M33 + Left.M44 * Right.M43;
                Result.M44 = Left.M41 * Right.M14 + Left.M42 * Right.M24 + Left.M43 * Right.M34 + Left.M44 * Right.M44;
                return Result;
            }
            return Right;
        }

        static public Matrix4 AMult(Matrix4 Left, Matrix4 Right)
        {
            Matrix4[] strings = new Matrix4[5];

            AsyncMult Mult0 = new AsyncMult(AsynMult);
            AsyncMult Mult1 = new AsyncMult(AsynMult);
            AsyncMult Mult2 = new AsyncMult(AsynMult);
            AsyncMult Mult3 = new AsyncMult(AsynMult);

            IAsyncResult[] str = new IAsyncResult[4];
            IAsyncResult str0 = Mult0.BeginInvoke(0, Left, Right, null, null);
            IAsyncResult str1 = Mult1.BeginInvoke(1, Left, Right, null, null);
            IAsyncResult str2 = Mult2.BeginInvoke(2, Left, Right, null, null);
            IAsyncResult str3 = Mult3.BeginInvoke(3, Left, Right, null, null);

            //Attempt to get info from the threads in async order - bad results 
            //bool check0 = false, check1 = false, check2 = false, check3 = false;
            //while ((check0 && check1 && check2 && check3) == false)
            //{
            //    if (str0.IsCompleted == true && check0 == false)
            //    {
            //        check0 = true;
            //        strings[0] = Mult0.EndInvoke(str0);
            //        strings[4] = strings[4] + strings[0];
            //    }
            //    if (str1.IsCompleted == true && check1 == false)
            //    {
            //        check1 = true;
            //        strings[1] = Mult1.EndInvoke(str1);
            //        strings[4] = strings[4] + strings[1];
            //    }
            //    if (str2.IsCompleted == true && check2 == false)
            //    {
            //        check2 = true;
            //        strings[2] = Mult2.EndInvoke(str2);
            //        strings[4] = strings[4] + strings[2];
            //    }
            //    if (str3.IsCompleted == true && check3 == false)
            //    {
            //        check3 = true;
            //        strings[3] = Mult3.EndInvoke(str3);
            //        strings[4] = strings[4] + strings[3];
            //    }
            //}

            strings[0] = Mult0.EndInvoke(str0);
            strings[1] = Mult1.EndInvoke(str1);
            strings[2] = Mult2.EndInvoke(str2);
            strings[3] = Mult3.EndInvoke(str3);
            strings[4] = strings[0] + strings[1] + strings[2] + strings[3];

            return strings[4];
        }

        // method special for Tasks paralel
        static public void TaskMult(int type, ref Matrix4 Left, ref Matrix4 Right, ref Matrix4 Result)
        {

            if (type == 0)
            {
                Result.M11 = Left.M11 * Right.M11 + Left.M12 * Right.M21 + Left.M13 * Right.M31 + Left.M14 * Right.M41;
                Result.M12 = Left.M11 * Right.M12 + Left.M12 * Right.M22 + Left.M13 * Right.M32 + Left.M14 * Right.M42;
                Result.M13 = Left.M11 * Right.M13 + Left.M12 * Right.M23 + Left.M13 * Right.M33 + Left.M14 * Right.M43;
                Result.M14 = Left.M11 * Right.M14 + Left.M12 * Right.M24 + Left.M13 * Right.M34 + Left.M14 * Right.M44;
            }
            if (type == 1)
            {
                Result.M21 = Left.M21 * Right.M11 + Left.M22 * Right.M21 + Left.M23 * Right.M31 + Left.M24 * Right.M41;
                Result.M22 = Left.M21 * Right.M12 + Left.M22 * Right.M22 + Left.M23 * Right.M32 + Left.M24 * Right.M42;
                Result.M23 = Left.M21 * Right.M13 + Left.M22 * Right.M23 + Left.M23 * Right.M33 + Left.M24 * Right.M43;
                Result.M24 = Left.M21 * Right.M14 + Left.M22 * Right.M24 + Left.M23 * Right.M34 + Left.M24 * Right.M44;
            }

            if (type == 2)
            {
                Result.M31 = Left.M31 * Right.M11 + Left.M32 * Right.M21 + Left.M33 * Right.M31 + Left.M34 * Right.M41;
                Result.M32 = Left.M31 * Right.M12 + Left.M32 * Right.M22 + Left.M33 * Right.M32 + Left.M34 * Right.M42;
                Result.M33 = Left.M31 * Right.M13 + Left.M32 * Right.M23 + Left.M33 * Right.M33 + Left.M34 * Right.M43;
                Result.M34 = Left.M31 * Right.M14 + Left.M32 * Right.M24 + Left.M33 * Right.M34 + Left.M34 * Right.M44;
            }
       
            if (type == 3)
            {
                Result.M41 = Left.M41 * Right.M11 + Left.M42 * Right.M21 + Left.M43 * Right.M31 + Left.M44 * Right.M41;
                Result.M42 = Left.M41 * Right.M12 + Left.M42 * Right.M22 + Left.M43 * Right.M32 + Left.M44 * Right.M42;
                Result.M43 = Left.M41 * Right.M13 + Left.M42 * Right.M23 + Left.M43 * Right.M33 + Left.M44 * Right.M43;
                Result.M44 = Left.M41 * Right.M14 + Left.M42 * Right.M24 + Left.M43 * Right.M34 + Left.M44 * Right.M44;
            }
        }
    }
}
