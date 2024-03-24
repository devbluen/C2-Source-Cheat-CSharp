using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ExternoTeste
{
    public static class Calculate
    {
        public static Vector2 WorldToScreen(float[] matrix, Vector3 pos, Vector2 windowSize)
        {
            // calculate screenW
            float screenW = (matrix[12] * pos.X) + (matrix[13] * pos.Y) + (matrix[14] * pos.Z) + matrix[15];

            // if entity is in front of us
            if (screenW > 0.001f)
            {
                // calculate screen X and Y
                float screenX = (matrix[0] * pos.X) + (matrix[1] * pos.Y) + (matrix[2] * pos.Z) + matrix[3];
                float screenY = (matrix[4] * pos.X) + (matrix[5] * pos.Y) + (matrix[6] * pos.Z) + matrix[7];

                // perform pespective division
                float X = (windowSize.X / 2) + (windowSize.X / 2) * screenX / screenW;
                float Y = (windowSize.Y / 2) - (windowSize.Y / 2) * screenY / screenW;

                return new Vector2(X, Y);
            }
            else
            {
                // return indicate value
                return new Vector2(-99, -99);
            }
        }

        public static Vector2 WorldToScreenMatrix(ViewMatrix matrix, Vector3 pos, int widht, int height)
        {
            Vector2 screenCoordinates = new Vector2();

            // calculate screenW
            float screenW = (matrix.m41 * pos.X) + (matrix.m42 * pos.Y) + (matrix.m43 * pos.Z) + matrix.m44;

            // if entity is in front of us
            if (screenW > 0.001f)
            {
                // calculate screen X and Y
                float screenX = (matrix.m11 * pos.X) + (matrix.m12 * pos.Y) + (matrix.m13 * pos.Z) + matrix.m14;
                float screenY = (matrix.m21 * pos.X) + (matrix.m22 * pos.Y) + (matrix.m23 * pos.Z) + matrix.m24;

                // camera center
                float camX = widht / 2;
                float camY = height / 2;

                // perform pespective division
                float X = camX + (camX * screenX / screenW);
                float Y = camY - (camY * screenY / screenW);

                // return coords
                screenCoordinates.X = X;
                screenCoordinates.Y = Y;
                return screenCoordinates;
            }
            else
            {
                // return indicate value
                return new Vector2(-99, -99);
            }
        }

        public static Vector2 CalculateAngles(Vector3 from, Vector3 to)
        {
            float yaw;
            float pitch;

            float deltaX = to.X - from.X;
            float deltaY = to.Y - from.Y;
            yaw = (float)(Math.Atan2(deltaY, deltaX) * 180 / 3.14);

            float deltaZ = to.Z - from.Z;
            double distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            pitch = -(float)(Math.Atan2(deltaZ, distance) * 180 / 3.14);

            return new Vector2(yaw, pitch);
        }
    }
}
