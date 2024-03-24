using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ClickableTransparentOverlay;
using Swed64;
using ImGuiNET;
using System.Runtime.InteropServices;

namespace ExternoTeste
{
    public class Renderer : Overlay
    {
        Swed swed = new Swed("cs2");

        // Render Variaveis
        public Vector2 screenSize = new Vector2(1920, 1080);

        // Entidades Copy (Verificar metodo depois)
        private ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
        private Entity localPlayer = new Entity();
        private readonly object entityLock = new Object();

        // Gui Elements

        // Wallhack
        private int wallHackStyle = 0;
        private bool enableBox = true;
        public bool enableGlow = false;
        private bool ignoreTeamESP = true;
        private bool enableSkeleton = true;
        private bool enableHealthBar = true;
        private bool enableLines = false;
        private bool enableInfos = false;
        private float boneThickkness = 1;
        public bool IgnoreWalls = true;
        private bool enableTigger = false;
        private bool WallColors = true;

        // Aimbot
        private bool enableAimbot = true;
        private bool wallAimbotIgnore = false;
        private float aimbotFov = 30;

        // Misc
        public bool enableBhop = false;

        // Imgui
        private bool InGuiMenu = true;
        private Vector4 enemyColor = new Vector4(1, 0, 0, 1);               // Vermelho
        private Vector4 teamColor = new Vector4(0, 1, 0, 1);                // Verde
        private Vector4 heathBarColor = new Vector4(255, 255, 255, 255);    // Verde
        private Vector4 distanceColor = new Vector4(255, 255, 255, 255);    // Branco
        private Vector4 fovColor = new Vector4(255, 255, 255, 255);         // Branco
        private Vector4 boxColor_Wall = new Vector4(0, 0, 0, 255);          // Preto
        private Vector4 skeletonColor = new Vector4(255, 255, 255, 255);    // Branco
        private Vector4 linesColor = new Vector4(255, 255, 255, 255);       // Branco

        static readonly string[] Ranks = {
            "Unranked",
            "Silver1",
            "Silver2",
            "Silver3",
            "Silver4",
            "Silver Elite",
            "Silver Elite Master",

            "GOLD NOVA 1",
            "GOLD NOVA 2",
            "GOLD NOVA 3",
            "GOLD NOVA Master",
            "MG1",
            "MG2",

            "MGE",
            "DMG",
            "LE",
            "LEM",
            "SMFC",
            "GE"
        };

        // Draw List
        ImDrawListPtr drawList;

        protected override void Render()
        {
            if (InGuiMenu)
            {
                ImGui.Begin("Rippzy C. V.1");
                if (ImGui.BeginTabBar("Tabs"))
                {
                    if (ImGui.BeginTabItem("Menu"))
                    {
                        if (ImGui.CollapsingHeader("Aimbot Config"))
                        {
                            ImGui.Checkbox("Aimbot", ref enableAimbot);
                            ImGui.Checkbox("TriggerBot", ref enableTigger);
                            ImGui.Checkbox("Ignore Walls [Aim]", ref wallAimbotIgnore);
                            ImGui.SliderFloat("FOV", ref aimbotFov, 10, 300);
                        }

                        if (ImGui.CollapsingHeader("Wallhack"))
                        {
                            ImGui.Checkbox("Box", ref enableBox);
                            ImGui.Checkbox("Glow", ref enableGlow);
                            ImGui.Checkbox("Skeleton", ref enableSkeleton);
                            ImGui.Checkbox("Lines", ref enableLines);
                            ImGui.Checkbox("HealthBar", ref enableHealthBar);
                            ImGui.Checkbox("Infos", ref enableInfos);
                            
                            if (ImGui.CollapsingHeader("Config [Wallhack]"))
                            {
                                ImGui.Text("WallHack Style");
                                if (ImGui.Button("-"))
                                {
                                    if (wallHackStyle >= 1)
                                        wallHackStyle--;
                                }
                                ImGui.SameLine();
                                ImGui.Text($"{wallHackStyle}");
                                ImGui.SameLine();
                                if (ImGui.Button("+"))
                                {
                                    if (wallHackStyle < 1)
                                        wallHackStyle++;
                                }

                                ImGui.Checkbox("Ignore Team", ref ignoreTeamESP);
                                ImGui.Checkbox("Ignore Walls", ref IgnoreWalls);
                                ImGui.Checkbox("Wall Colors", ref WallColors);
                                ImGui.SliderFloat("Bone Thickness", ref boneThickkness, 1, 3);
                            }
                        }
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Colors"))
                    {
                        ImGui.Text("Team Color");
                        ImGui.Dummy(new Vector2(0.0f, 2.0f));
                        ImGui.ColorPicker4("##teamcolor", ref teamColor);
                        ImGui.Dummy(new Vector2(0.0f, 20.0f));

                        ImGui.Text("Enemy Color");
                        ImGui.Dummy(new Vector2(0.0f, 2.0f));
                        ImGui.ColorPicker4("##enemycolor", ref enemyColor);
                        ImGui.Dummy(new Vector2(0.0f, 20.0f));

                        ImGui.Text("Enemy Box Color In Wall");
                        ImGui.Dummy(new Vector2(0.0f, 2.0f));
                        ImGui.ColorPicker4("##boxcolor_wall", ref boxColor_Wall);
                        ImGui.Dummy(new Vector2(0.0f, 20.0f));

                        ImGui.Text("Skeleton Color");
                        ImGui.Dummy(new Vector2(0.0f, 2.0f));
                        ImGui.ColorPicker4("##skeleton", ref skeletonColor);
                        ImGui.Dummy(new Vector2(0.0f, 20.0f));

                        ImGui.Text("Health Color");
                        ImGui.Dummy(new Vector2(0.0f, 2.0f));
                        ImGui.ColorPicker4("##healthColor", ref heathBarColor);
                        ImGui.Dummy(new Vector2(0.0f, 20.0f));

                        ImGui.Text("Distance Color");
                        ImGui.Dummy(new Vector2(0.0f, 2.0f));
                        ImGui.ColorPicker4("##distanceColor", ref distanceColor);
                        ImGui.Dummy(new Vector2(0.0f, 20.0f));

                        ImGui.Text("FOV Color");
                        ImGui.Dummy(new Vector2(0.0f, 2.0f));
                        ImGui.ColorPicker4("##fovcolor", ref fovColor);
                        ImGui.Dummy(new Vector2(0.0f, 20.0f));

                        ImGui.Text("Lines");
                        ImGui.Dummy(new Vector2(0.0f, 2.0f));
                        ImGui.ColorPicker4("##linesColor", ref linesColor);
                        ImGui.Dummy(new Vector2(0.0f, 20.0f));

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Configuração"))
                    {
                        ImGui.Checkbox("BHOP", ref enableBhop);
                        ImGui.EndTabItem();
                    }
                }
            }

            DrawOverlay(screenSize); // Draw Overlay
            drawList = ImGui.GetWindowDrawList();

            if (enableAimbot)
                DrawFov();

            if (GetAsyncKeyState(Offsets.imguiKey) < 0)
            {
                if (InGuiMenu) InGuiMenu = false;
                else InGuiMenu = true;
                Thread.Sleep(20);
            }

            foreach (var entity in entities)
            {
                if (ignoreTeamESP)
                    if (localPlayer.team == entity.team)
                        continue;

                if (EntityOnScreen(entity))
                {
                    DrawCheats(entity);

                    if (enableAimbot && GetAsyncKeyState(0x01) < 0)
                    {
                        if (wallAimbotIgnore || !wallAimbotIgnore && entity.spotted) 
                        { 
                            Vector3 playerView = Vector3.Add(localPlayer.position, localPlayer.viewOffset);
                            Vector3 entityView = Vector3.Add(entity.position, entity.viewOffset);

                            if (entity.pixelDistance < aimbotFov)
                            {
                                Vector2 newAngles = Calculate.CalculateAngles(playerView, entity.head);
                                Vector3 newAnglesVec3 = new Vector3(newAngles.Y, newAngles.X, 0.0f);
                                swed.WriteVec(Offsets.client, Offsets.dwViewAngles, newAnglesVec3);
                            }
                        }
                    }

                    if (enableTigger && GetAsyncKeyState(0x12) < 0)
                    {
                        int entIndex = swed.ReadInt(Offsets.PlayerPawn, Offsets.m_iIDEntIndex);
                        Console.Clear();
                        Console.WriteLine(entIndex);
                        if (entIndex != -1)
                        {
                            swed.WriteInt(Offsets.forceAttack, 65537);
                            Thread.Sleep(1);
                            swed.WriteInt(Offsets.forceAttack, 256);
                        }
                        Thread.Sleep(1);
                    }
                }
            }
        }

        // functions
        void AimAt(Vector3 angles)
        {
            swed.WriteFloat(Offsets.client, Offsets.dwViewAngles, angles.Y);
            swed.WriteFloat(Offsets.client, Offsets.dwViewAngles + 0x4, angles.X);
        }

        public float CalculateMagnitude(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2) + Math.Pow(v2.Z - v1.Z, 2));
        }

        // check position
        bool EntityOnScreen(Entity entity)
        {
            if (entity.position2D.X > 0 && entity.position2D.X < screenSize.X && entity.position2D.Y > 0 && entity.position2D.Y < screenSize.Y)
            {
                return true;
            }
            return false;
        }

        // drawing methods
        public void DrawFov()
        {
            drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), aimbotFov, ImGui.ColorConvertFloat4ToU32(fovColor));
        }

        public List<Vector3> ReadBones(IntPtr boneAddress)
        {
            byte[] boneBytes = swed.ReadBytes(boneAddress, 27 * 32 + 16);
            List<Vector3> bones = new List<Vector3>();
            foreach (var boneId in Enum.GetValues(typeof(BonesIds)))
            {
                float x = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 0);
                float y = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 4);
                float z = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 8);
                Vector3 currentBone = new Vector3(x, y, z);
                bones.Add(currentBone);
            }
            return bones;
        }

        public List<Vector2> ReadBones2d(List<Vector3> bones, ViewMatrix viewMatrix, Vector2 screenSize)
        {
            List<Vector2> bones2d = new List<Vector2>();
            foreach( Vector3 bone in bones)
            {
                Vector2 bone2d = Calculate.WorldToScreenMatrix(viewMatrix, bone, (int)screenSize.X, (int)screenSize.Y);
                bones2d.Add(bone2d);
            }
            return bones2d;
        }
        
        public ViewMatrix readMatrix(IntPtr matrixAddress)
        {
            var viewMatrix = new ViewMatrix();
            var matrix = swed.ReadMatrix(matrixAddress);

            viewMatrix.m11 = matrix[0];
            viewMatrix.m12 = matrix[1];
            viewMatrix.m13 = matrix[2];
            viewMatrix.m14 = matrix[3];

            viewMatrix.m21 = matrix[4];
            viewMatrix.m22 = matrix[5];
            viewMatrix.m23 = matrix[6];
            viewMatrix.m24 = matrix[7];

            viewMatrix.m31 = matrix[8];
            viewMatrix.m32 = matrix[9];
            viewMatrix.m33 = matrix[10];
            viewMatrix.m34 = matrix[11];

            viewMatrix.m41 = matrix[12];
            viewMatrix.m42 = matrix[13];
            viewMatrix.m43 = matrix[14];
            viewMatrix.m44 = matrix[15];
            return viewMatrix;
        }

        private void DrawCheats(Entity entity)
        {
            if (!IgnoreWalls && !entity.spotted)
                return;

            // Calculate box Height
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            /*
             *              Wallhack
             */
            float cornerLength = 10.0f;
            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);
            Vector2 rectBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);
            Vector4 boxColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            if(enableBox) 
            {
                uint wallColorF;
                if (WallColors) { wallColorF = entity.spotted ? (ImGui.ColorConvertFloat4ToU32(boxColor)) : (ImGui.ColorConvertFloat4ToU32(boxColor_Wall)); }
                else { wallColorF = ImGui.ColorConvertFloat4ToU32(boxColor); }

                switch (wallHackStyle)
                {
                    case 0:
                        drawList.AddRect(rectTop, rectBottom, wallColorF);       // Caixa Completa
                        break;
                    case 1:
                        drawList.AddLine(rectTop, new Vector2(rectTop.X, rectTop.Y + cornerLength), wallColorF); // Vertical
                        drawList.AddLine(rectTop, new Vector2(rectTop.X + cornerLength, rectTop.Y), wallColorF); // Horizontal

                        // Calcular e desenhar os segmentos de canto superior direito
                        Vector2 topRight = new Vector2(rectBottom.X, rectTop.Y);
                        drawList.AddLine(topRight, new Vector2(topRight.X, topRight.Y + cornerLength), wallColorF); // Vertical
                        drawList.AddLine(topRight, new Vector2(topRight.X - cornerLength, topRight.Y), wallColorF); // Horizontal

                        // Calcular e desenhar os segmentos de canto inferior esquerdo
                        Vector2 bottomLeft = new Vector2(rectTop.X, rectBottom.Y);
                        drawList.AddLine(bottomLeft, new Vector2(bottomLeft.X, bottomLeft.Y - cornerLength), wallColorF); // Vertical
                        drawList.AddLine(bottomLeft, new Vector2(bottomLeft.X + cornerLength, bottomLeft.Y), wallColorF); // Horizontal

                        // Calcular e desenhar os segmentos de canto inferior direito
                        drawList.AddLine(rectBottom, new Vector2(rectBottom.X, rectBottom.Y - cornerLength), wallColorF); // Vertical
                        drawList.AddLine(rectBottom, new Vector2(rectBottom.X - cornerLength, rectBottom.Y), wallColorF); // Horizontal
                        break;
                }
            }

            /*
             *              Informações de Vida
             */

            if (enableHealthBar) { 
                float barWidth = 2.0f; // Largura da barra de saúde
                float gap = 2.0f; // Distância entre a caixa e a barra de saúde
                float barPercent = entity.health / 100f;
                Vector2 barEnd = new Vector2(rectTop.X - gap, rectTop.Y);
                Vector2 barStart = new Vector2(rectTop.X - gap - barWidth, rectTop.Y + entityHeight * barPercent);
                drawList.AddRectFilled(barStart, barEnd, ImGui.ColorConvertFloat4ToU32(heathBarColor));
            }

            /*
             *              Informações do Jogador
             */

            if (enableInfos) { 
                float infoBoxHeight = 60.0f; // Suficiente para três linhas de texto
                float infoBoxWidth = rectBottom.X - rectTop.X + 10.0f; // Largura da caixa da entidade + margem
                Vector2 infoBoxTopLeft = new Vector2(rectTop.X - 5.0f, rectBottom.Y + 5.0f); // Inicia um pouco à esquerda e abaixo da caixa da entidade
                Vector2 infoBoxBottomRight = new Vector2(infoBoxTopLeft.X + infoBoxWidth, infoBoxTopLeft.Y + infoBoxHeight);
                Vector4 infoBoxColor = new Vector4(0, 0, 0, 0.5f); // Preto com transparência
                drawList.AddRectFilled(infoBoxTopLeft, infoBoxBottomRight, ImGui.ColorConvertFloat4ToU32(infoBoxColor));

                string distanceText = $"Dist.: {entity.distance:F}";
                string rankText = $"Rank: {Ranks[entity.rank]}";

                drawList.AddText(new Vector2(infoBoxTopLeft.X + 5, infoBoxTopLeft.Y + 5), ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)), distanceText);
                drawList.AddText(new Vector2(infoBoxTopLeft.X + 5, infoBoxTopLeft.Y + 25), ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)), rankText);
            }

            if (enableSkeleton) {
                drawList.AddLine(entity.bones2d[1], entity.bones2d[2], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // neck to head
                drawList.AddLine(entity.bones2d[1], entity.bones2d[3], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // neck to left shoulder
                drawList.AddLine(entity.bones2d[1], entity.bones2d[6], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // neck to right shoulder
                drawList.AddLine(entity.bones2d[3], entity.bones2d[4], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // shoulderLeft to armLeft
                drawList.AddLine(entity.bones2d[6], entity.bones2d[7], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // shoulderRight to armRight
                drawList.AddLine(entity.bones2d[4], entity.bones2d[5], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // armLeft to handLeft
                drawList.AddLine(entity.bones2d[7], entity.bones2d[8], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // armRight to handRight
                drawList.AddLine(entity.bones2d[1], entity.bones2d[0], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // neck to waist
                drawList.AddLine(entity.bones2d[0], entity.bones2d[9], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // waist to kneeLeft
                drawList.AddLine(entity.bones2d[0], entity.bones2d[11], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // waist to kneeRight
                drawList.AddLine(entity.bones2d[9], entity.bones2d[10], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // kneeLeft to feetLeft
                drawList.AddLine(entity.bones2d[11], entity.bones2d[12], ImGui.ColorConvertFloat4ToU32(skeletonColor), boneThickkness);  // kneeLeft to feetLeft
            }

            if (enableLines) { 
                drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y), entity.position2D, ImGui.ColorConvertFloat4ToU32(linesColor));
            }
        }

        // Transfer entity methods
        public void UpdateEntities(IEnumerable<Entity> newEntities)  // Update Entitys
        {
            entities = new ConcurrentQueue<Entity>(newEntities);
        }

        public void UpdateLocalPlayer(Entity newEntity)
        {
            lock(entityLock)
            {
                localPlayer = newEntity;
            }
        }

        void DrawOverlay(Vector2 screenSize)
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse);
        }   // ImGui Overlay

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);
    }
}