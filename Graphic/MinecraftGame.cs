using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace MineLib.ClientWrapper.Graphic
{
    internal enum Face : byte { Front, Right, Top, Left, Bottom, Back }

    public class MinecraftGame : GameWindow
    {
        private const float MoveSpeed = 4.317f;
        private const float SprintSpeed = 5.612f;
        private const float FlySpeed = 10.89f;

        private const float JumpSpeed  = 1.0f;
        private const float JumpMaxY = 1.252f;
        private const float JumpBoostIMaxY = 1.836f;
        private const float JumpBoostIIMaxY = 2.517f;


        private readonly Camera _camera;

        public MinecraftGame() : base(800, 600)
        {
            _camera = new Camera(Vector3.One, Vector3.Zero);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.Blend);
            //GL.Enable(EnableCap.Multisample);
            //GL.Enable(EnableCap.CullFace);
            //GL.Enable(EnableCap.ColorMaterial);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (double) Height;

            var perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float) aspectRatio, 1, 256);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }

        /// <summary>
        /// Prepares the next frame for rendering.
        /// </summary>
        /// <remarks>
        /// Place your control logic here. This is the place to respond to user input,
        /// update object positions etc.
        /// </remarks>
        private MouseState lastMouse;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Title = string.Format("123 - {0:0.0} FPS", RenderFrequency);

            var keyboard = OpenTK.Input.Keyboard.GetState();
            OpenTK.Input.Mouse.SetPosition(0, 0);

            var mouse = OpenTK.Input.Mouse.GetState();

            if (keyboard[Key.Escape])
                Exit();

            if (keyboard[Key.A])
                _camera.Move(-MoveSpeed, 0, 0);

            if (keyboard[Key.D])
                _camera.Move(MoveSpeed, 0, 0);

            if (keyboard[Key.W])
                _camera.Move(0, MoveSpeed, 0);

            if (keyboard[Key.S])
                _camera.Move(0, -MoveSpeed, 0);

            if (lastMouse.X < mouse.X)
                _camera.AddRotation(-5, 0);

            if (lastMouse.X > mouse.X)
                _camera.AddRotation(5, 0);

            if (lastMouse.Y < mouse.Y)
                _camera.AddRotation(0, -10);

            if (lastMouse.Y > mouse.Y)
                _camera.AddRotation(0, 10);


            lastMouse = mouse;
        }


        /// <summary>
        /// Place your rendering code here.
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var lookat = _camera.GetViewMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Silver);
            GL.End();

            SwapBuffers();
            //Thread.Sleep(1);
        }

    }
}