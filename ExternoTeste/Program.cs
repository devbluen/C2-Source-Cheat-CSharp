using ExternoTeste;
using Swed64;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

// init swed
Offsets.swed = new Swed("cs2");
Swed swed = Offsets.swed;

// get client module
Offsets.client = swed.GetModuleBase("client.dll");

// store entities
List<Entity> entities = new List<Entity>();
Entity localPlayer = new Entity();

// init render
Renderer renderer = new Renderer();
Thread renderThread = new Thread(new ThreadStart(renderer.Start().Wait));
renderThread.Start();

// get screen size from renderer
Vector2 screenSize = renderer.screenSize;
Console.WriteLine("Olá, mundo!");

const uint STANDING = 65665;
const uint CROUCHING = 65667;
const uint PLUS_JUMP = 65537; // + jump
const uint MINUS_JUMP = 256;  // - jump

while (true)
{
    entities.Clear();

    // get entity list
    IntPtr entityList = swed.ReadPointer(Offsets.client, Offsets.dwEntityList);

    // make entry
    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

    // get localplayer
    IntPtr localPlayerPawn = swed.ReadPointer(Offsets.client, Offsets.dwLocalPlayerPawn);
    IntPtr localJumping = Offsets.client + Offsets.dwForceJump;

    Offsets.PlayerPawn = localPlayerPawn;

    // get flag
    uint fFlag = swed.ReadUInt(localPlayerPawn, Offsets.m_fFlags);

    // get team
    localPlayer.team = swed.ReadInt(localPlayerPawn, Offsets.m_iTeamNum);
    localPlayer.position = swed.ReadVec(localPlayerPawn, Offsets.m_vOldOrigin);
    localPlayer.viewOffset = swed.ReadVec(localPlayerPawn, Offsets.m_vecViewOffset);

    // loop entity list
    for (int i = 0; i < 64; i++)
    {
        // get cururent controller
        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
        if (currentController == IntPtr.Zero) continue;

        int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
        if (pawnHandle == 0) continue;
    
        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
        if (listEntry2 == IntPtr.Zero) continue;

        // get current pawn
        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == IntPtr.Zero) continue;

        // check if lifestate
        int lifeState = swed.ReadInt(currentPawn, Offsets.m_lifeState);
        if (lifeState != 256) continue;

        // get matrix
        float[] viewMatrix = swed.ReadMatrix(Offsets.client + Offsets.dwViewMatrix);
        ViewMatrix viewMatrix2 = renderer.readMatrix(Offsets.client + Offsets.dwViewMatrix);

        // scaneNode
        IntPtr scaneNode = swed.ReadPointer(currentPawn, Offsets.m_pGameSceneNode);
        IntPtr boneMatrix = swed.ReadPointer(scaneNode, Offsets.m_modelState + 0x80);

        // polulate entity
        Entity entity = new Entity();

        entity.team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
        entity.health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
        entity.rank = swed.ReadInt(currentPawn, Offsets.m_iCompetitiveRanking);

        entity.position = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
        entity.position2D = Calculate.WorldToScreen(viewMatrix, entity.position, screenSize);

        entity.viewOffset = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
        entity.viewPosition2D = Calculate.WorldToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), screenSize);

        entity.distance = Vector3.Distance(entity.position, localPlayer.position);
        entity.magnitude = renderer.CalculateMagnitude(localPlayer.position, entity.position);

        entity.spotted = swed.ReadBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted);


        // Bones [??]
        entity.bones = renderer.ReadBones(boneMatrix);
        entity.bones2d = renderer.ReadBones2d(entity.bones, viewMatrix2, screenSize);

        // TODO: Verificar futuramente esses dois códigos e unificar.
        // Heads [??]
        entity.head = swed.ReadVec(boneMatrix, 6 * 32);
        entity.head2d = Calculate.WorldToScreenMatrix(viewMatrix2, entity.head, (int)screenSize.X, (int)screenSize.Y);
        entity.pixelDistance = Vector2.Distance(entity.head2d, new Vector2(screenSize.X / 2, screenSize.Y / 2));

        if (renderer.enableGlow)
        {
            if (renderer.IgnoreWalls || !renderer.IgnoreWalls && entity.spotted)
            {
                swed.WriteFloat(currentPawn, Offsets.m_flDetectedByEnemySensorTime, 86400);
            }
        }

        entities.Add(entity);
    }

    if (renderer.enableBhop)
    {
        if (GetAsyncKeyState(Offsets.jumpKey) < 0)
        {
            if (fFlag == STANDING || fFlag == CROUCHING)
            {
                swed.WriteUInt(localJumping, MINUS_JUMP);
                Thread.Sleep(1);
                swed.WriteUInt(localJumping, PLUS_JUMP);
            }
            else
            {
                swed.WriteUInt(localJumping, MINUS_JUMP);
            }
        }
    }

    // update renderer data
    renderer.UpdateLocalPlayer(localPlayer);
    renderer.UpdateEntities(entities);
    
    // Thread Sleep
    // Thread.Sleep(5);    // Opcional
}

[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);