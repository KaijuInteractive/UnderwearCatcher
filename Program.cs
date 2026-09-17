using Raylib_cs;
using System.Numerics;

class Program
{
    static void Main()
    {
        // ============================================================
        // WINDOW
        // ============================================================

        const int screenWidth = 800;
        const int screenHeight = 600;

        Raylib.InitWindow(
            screenWidth,
            screenHeight,
            "Underwear Catcher"
        );

        Raylib.SetTargetFPS(60);


        // ============================================================
        // LOAD TEXTURES
        // ============================================================

        Texture2D catcherNaked =
            Raylib.LoadTexture("Assets/Catcher.png");

        Texture2D catcherCaught =
            Raylib.LoadTexture("Assets/Briefs_Caught.png");

        Texture2D briefs =
            Raylib.LoadTexture("Assets/Briefs_Catch.png");


        // ============================================================
        // PIXEL ART FILTERING
        // ============================================================

        Raylib.SetTextureFilter(
            catcherNaked,
            TextureFilter.Point
        );

        Raylib.SetTextureFilter(
            catcherCaught,
            TextureFilter.Point
        );

        Raylib.SetTextureFilter(
            briefs,
            TextureFilter.Point
        );


        // ============================================================
        // SCALE
        // ============================================================

        const float catcherScale = 3.0f;
        const float briefsScale = 3.0f;


        // ============================================================
        // PLAYER
        // ============================================================

        Vector2 catcherPosition = new Vector2(
            screenWidth / 2.0f -
            (catcherNaked.Width * catcherScale) / 2.0f,

            screenHeight -
            (catcherNaked.Height * catcherScale) -
            35
        );

        float catcherSpeed = 500.0f;

        bool wearingBriefs = false;


        // ============================================================
        // FALLING BRIEFS
        // ============================================================

        Vector2 briefsPosition = new Vector2(
            Raylib.GetRandomValue(40, 700),
            -100
        );

        float briefsSpeed = 180.0f;


        // ============================================================
        // GAME DATA
        // ============================================================

        int score = 0;
        int misses = 0;


        // ============================================================
        // GAME LOOP
        // ============================================================

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();


            // ========================================================
            // PLAYER MOVEMENT
            // ========================================================

            if (Raylib.IsKeyDown(KeyboardKey.Left) ||
                Raylib.IsKeyDown(KeyboardKey.A))
            {
                catcherPosition.X -= catcherSpeed * deltaTime;
            }

            if (Raylib.IsKeyDown(KeyboardKey.Right) ||
                Raylib.IsKeyDown(KeyboardKey.D))
            {
                catcherPosition.X += catcherSpeed * deltaTime;
            }


            // ========================================================
            // PLAYER SCREEN BOUNDARIES
            // ========================================================

            float catcherWidth =
                catcherNaked.Width * catcherScale;

            if (catcherPosition.X < 0)
            {
                catcherPosition.X = 0;
            }

            if (catcherPosition.X + catcherWidth > screenWidth)
            {
                catcherPosition.X =
                    screenWidth - catcherWidth;
            }


            // ========================================================
            // MOVE FALLING BRIEFS
            // ========================================================

            briefsPosition.Y +=
                briefsSpeed * deltaTime;


            // ========================================================
            // COLLISION RECTANGLES
            // ============================================================

            Rectangle catcherRectangle =
                new Rectangle(
                    catcherPosition.X,
                    catcherPosition.Y,
                    catcherNaked.Width * catcherScale,
                    catcherNaked.Height * catcherScale
                );

            Rectangle briefsRectangle =
                new Rectangle(
                    briefsPosition.X,
                    briefsPosition.Y,
                    briefs.Width * briefsScale,
                    briefs.Height * briefsScale
                );


            // ========================================================
            // CATCH!
            // ============================================================

            if (Raylib.CheckCollisionRecs(
                catcherRectangle,
                briefsRectangle))
            {
                score++;

                // --------------------------------------------
                // FIRST CATCH
                // --------------------------------------------

                if (!wearingBriefs)
                {
                    wearingBriefs = true;
                }


                // --------------------------------------------
                // RESET FALLING BRIEFS
                // --------------------------------------------

                briefsPosition.X =
                    Raylib.GetRandomValue(
                        40,
                        screenWidth -
                        (int)(briefs.Width * briefsScale) -
                        40
                    );

                briefsPosition.Y = -100;


                // --------------------------------------------
                // SPEED UP SLIGHTLY
                // --------------------------------------------

                briefsSpeed += 8.0f;
            }


            // ========================================================
            // MISS
            // ============================================================

            if (briefsPosition.Y > screenHeight)
            {
                misses++;

                briefsPosition.X =
                    Raylib.GetRandomValue(
                        40,
                        screenWidth -
                        (int)(briefs.Width * briefsScale) -
                        40
                    );

                briefsPosition.Y = -100;
            }


            // ========================================================
            // DRAW
            // ============================================================

            Raylib.BeginDrawing();

            Raylib.ClearBackground(
                new Color(20, 55, 60, 255)
            );


            // ========================================================
            // TITLE
            // ============================================================

            Raylib.DrawText(
                "UNDERWEAR CATCHER",
                220,
                25,
                32,
                Color.SkyBlue
            );


            // ========================================================
            // SCORE
            // ============================================================

            Raylib.DrawText(
                $"SCORE: {score}",
                30,
                75,
                22,
                Color.White
            );


            // ========================================================
            // MISSES
            // ============================================================

            Raylib.DrawText(
                $"MISSES: {misses}",
                640,
                75,
                22,
                Color.White
            );


            // ========================================================
            // FALLING BRIEFS
            // ============================================================

            Raylib.DrawTextureEx(
                briefs,
                briefsPosition,
                0.0f,
                briefsScale,
                Color.White
            );


            // ========================================================
            // PLAYER
            // ============================================================

            if (!wearingBriefs)
            {
                // BEFORE FIRST CATCH

                Raylib.DrawTextureEx(
                    catcherNaked,
                    catcherPosition,
                    0.0f,
                    catcherScale,
                    Color.White
                );
            }
            else
            {
                // AFTER FIRST CATCH
                // This becomes permanent.

                Raylib.DrawTextureEx(
                    catcherCaught,
                    catcherPosition,
                    0.0f,
                    catcherScale,
                    Color.White
                );
            }


            // ========================================================
            // FIRST-CATCH INSTRUCTION
            // ============================================================

            if (!wearingBriefs)
            {
                Raylib.DrawText(
                    "CATCH YOUR FIRST PAIR!",
                    275,
                    120,
                    20,
                    Color.Pink
                );
            }
            else
            {
                Raylib.DrawText(
                    "KEEP CATCHING!",
                    320,
                    120,
                    20,
                    Color.SkyBlue
                );
            }


            // ========================================================
            // CONTROLS
            // ============================================================

            Raylib.DrawText(
                "A/D OR ARROWS - MOVE",
                275,
                565,
                18,
                Color.Gray
            );


            Raylib.EndDrawing();
        }


        // ============================================================
        // CLEANUP
        // ============================================================

        Raylib.UnloadTexture(catcherNaked);
        Raylib.UnloadTexture(catcherCaught);
        Raylib.UnloadTexture(briefs);

        Raylib.CloseWindow();
    }
}