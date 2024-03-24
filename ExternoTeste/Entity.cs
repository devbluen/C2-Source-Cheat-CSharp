using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ExternoTeste
{
    public class Entity
    {
        public Vector3 position { get; set; }
        public Vector3 viewOffset { get; set; }
        public Vector2 position2D { get; set; }
        public Vector2 viewPosition2D { get; set; }

        public Vector3 head { get; set; }
        public Vector2 head2d { get; set; }

        public List<Vector3> bones { get; set; }
        public List<Vector2> bones2d { get; set; }

        public float distance { get; set; }
        public float pixelDistance { get; set; }
        public int health { get; set; }
        public int rank { get; set; }
        public int team { get; set; }

        public float magnitude { get; set; }
        public bool spotted {  get; set; }
    }

    public enum BonesIds
    { 
        Waist = 0,
        Neck = 5,
        Head = 6,
        ShoulderLeft = 8,
        ForeLeft = 9,
        HandLeft = 11,
        ShoulderRight = 13,
        ForeRight = 14,
        HandRight = 15,
        KneeLeft = 23,
        FeetLeft = 24,
        KneeRight = 26,
        FeetRight = 27
    }
}
