using Swed64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternoTeste
{
    public class Offsets
    {
        public const int imguiKey = 0x2D;
        public const int aimbotKey = 0x06;
        public const int jumpKey = 0x20;

        public static IntPtr client { get; set; }

        public static Entity? localPlayer { get; set; }
        public static Swed? swed { get; set; }

        // offsets.cs
        public static int dwViewAngles = 0x19309B0;
        public static int dwEntityList = 0x18C2D58;
        public static int dwViewMatrix = 0x19241A0;
        public static int dwLocalPlayerPawn = 0x17371A8;
        public static int dwForceJump = 0x1730530;
        public static int dwForceAttack = 0x1730020;

        // client.dll.cs
        public static int m_vOldOrigin = 0x127C;
        public static int m_iTeamNum = 0x3CB;
        public static int m_lifeState = 0x338;
        public static int m_hPlayerPawn = 0x7E4;
        public static int m_vecViewOffset = 0xC58;
        public static int m_iHealth = 0x334;
        public static int m_iCompetitiveRanking = 0x768;
        public static int m_pGameSceneNode = 0x318;
        public static int m_modelState = 0x160;
        public static int m_entitySpottedState = 0x1698;
        public static int m_bSpotted = 0x8;
        public static int m_flDetectedByEnemySensorTime = 0x1440;
        public static int m_fFlags = 0x3D4;
        public static int m_iIDEntIndex = 0x15A4;

        // config [my]
        public static IntPtr PlayerPawn { get; set; }
        public static IntPtr forceAttack = Offsets.client + Offsets.dwForceAttack;
    }
}
