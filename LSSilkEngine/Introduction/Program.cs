//#define INTRO01_CREATE_WINDOW
#define INTRO02_CREATE_BASIC_GEOMETRY
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using Silk.NET.Maths;

#if INTRO01_CREATE_WINDOW
namespace Introduction
{
    class Program
    {
        private static IWindow? window;
        private static void Main()
        {
            // Create a window
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "LSSilkEngine - Introduction";
            window = Window.Create(options);

            //Assign events
            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;


            window.Run();
        }

        private static void OnRender(double obj)
        {

        }
        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                window.Close();
            }
        }
        private static void OnUpdate(double obj)
        {

        }
        private static void OnLoad()
        {
            var input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }
        }
    }
}
#endif
#if INTRO02_CREATE_BASIC_GEOMETRY
namespace Introduction
{
    class Program
    {
        private static IWindow? window;

        // need to instantiate GL
        private static GL? GL;

        private static uint vbo;
        private static uint ebo;
        private static uint vao;
        private static uint shaderID;

        //Vertex shaders are run on each vertex.
        private static readonly string VertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec4 vPos;

        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

        //Fragment shaders are run on each fragment/pixel of the geometry.
        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }
        ";

        //Vertex data, uploaded to the VBO.
        private static readonly float[] Vertices =
        {
            //X    Y      Z
             0.5f,  0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.5f
        };

        //Index data, uploaded to the EBO.
        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };


        private static void Main()
        {
            // Create a window
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "LSSilkEngine - Introduction";
            window = Window.Create(options);

            //Assign events
            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Closing += OnClose;

            window.Run();
        }

        private static void OnClose()
        {
            if (GL == null) return;
            //Remember to delete the buffers.
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteVertexArray(vao);
            GL.DeleteProgram(shaderID);
        }

        private static unsafe void OnRender(double obj)
        {
            // check if GL is initialized
            if (GL == null) return;
            GL.Clear((uint)ClearBufferMask.ColorBufferBit);

            // Bind the geometry and shader
            GL.UseProgram(shaderID);
            GL.BindVertexArray(vao);

            // draw the geometry
            GL.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
        }
        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (window == null) return;
            if (arg2 == Key.Escape)
            {
                window.Close();
            }
        }
        private static void OnUpdate(double obj)
        {

        }
        private static unsafe void OnLoad()
        {
            if (window == null) return;
            var input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }

            GL = GL.GetApi(window);
            // Creating vertex array
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            // init vertex buffer 
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

            // set buffer data
            fixed (void* v = &Vertices[0])
            {
                GL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(Vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
            }

            // init element buffer 
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            fixed (void* v = &Indices[0])
            {
                GL.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
            }

            // create shader program
            uint vertShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertShader, VertexShaderSource);
            GL.CompileShader(vertShader);

            string infoLog = GL.GetShaderInfoLog(vertShader);
            if (!string.IsNullOrEmpty(infoLog)) Console.WriteLine($"Error compiling shader: \n {infoLog}");

            uint fragShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragShader, FragmentShaderSource);
            GL.CompileShader(fragShader);

            infoLog = GL.GetShaderInfoLog(fragShader);
            if (!string.IsNullOrEmpty(infoLog)) Console.WriteLine($"Error compiling shader: \n {infoLog}");


            shaderID = GL.CreateProgram();
            GL.AttachShader(shaderID, vertShader);
            GL.AttachShader(shaderID, fragShader);
            GL.LinkProgram(shaderID);

            GL.GetProgram(shaderID, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader! {GL.GetProgramInfoLog(shaderID)}");
            }


            GL.DetachShader(shaderID, vertShader);
            GL.DetachShader(shaderID, fragShader);
            GL.DeleteShader(vertShader);
            GL.DeleteShader(fragShader);


            //Tell opengl how to give the data to the shaders.
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            GL.EnableVertexAttribArray(0);
        }
    }
}
#endif
