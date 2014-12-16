using System;
using OpenTK;

namespace MineLib.ClientWrapper.Graphic
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 Orientation;
        public float MoveSpeed = 0.2f;
        public float MouseSensitivity = 0.01f;

        public Camera(Vector3 position, Vector3 orientation)
        {
            Position = position;
            Orientation = orientation;
        }

        public void Move(float x, float y, float z)
        {
            var offset = new Vector3();

            var forward = new Vector3((float) Math.Sin(Orientation.X), 0, (float) Math.Cos(Orientation.X));
            var right = new Vector3(-forward.Z, 0, forward.X);

            offset += x * right;
            offset += y * forward;
            offset.Y += z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

            Position += offset;
        }

        public void ToPos(Vector3 vector3)
        {
            Position = vector3;
        }

        public void AddRotation(float x, float y)
        {
            x = x * MouseSensitivity;
            y = y * MouseSensitivity;

            Orientation.X = (Orientation.X + x) % ((float) Math.PI * 2.0f);
            Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, (float) Math.PI / 2.0f - 0.1f), (float) -Math.PI / 2.0f + 0.1f);
        }

        public Matrix4 GetViewMatrix()
        {
            var lookat = new Vector3(
                (float) (Math.Sin(Orientation.X) * Math.Cos(Orientation.Y)),
                (float)  Math.Sin(Orientation.Y),
                (float) (Math.Cos(Orientation.X) * Math.Cos(Orientation.Y))
            );

            return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
        }
    }
}