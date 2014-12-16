using OpenTK;

namespace MineLib.ClientWrapper.Graphic
{
    public static class Vector3Converter
    {
        public static Vector3 ToOpenTKVector3(this Network.Data.Vector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Network.Data.Vector3 ToMineLibVector3(this Vector3 vector)
        {
            return new Network.Data.Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}
