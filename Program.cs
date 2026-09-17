using Raylib_cs;
using System;
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
        const float briefsScale = 2.0f;


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

        const float catcherSpeed = 500.0f;

        bool wearingBriefs = false;


        // ============================================================
        // FALLING BRIEFS
        // ============================================================

        Vector2 briefsPosition = new Vector2(
            Raylib.GetRandomValue(
                40,
                screenWidth -
                (int)(briefs.Width * briefsScale) -
                40
            ),
            -100
        );


        // ============================================================
        // DIFFICULTY
        // ============================================================

        float gameTime = 0.0f;

        const float startingBriefsSpeed = 180.0f;
        const float accelerationRate = 0.018f;
        const float maxBriefsSpeed = 900.0f;

        float briefsSpeed = startingBriefsSpeed;


        // ============================================================
        // GAME DATA
        // ============================================================

        int score = 0;
        int misses = 0;

        const int maxMisses = 3;

        bool gameOver = false;


        // ============================================================
        // GAME LOOP
        // ============================================================

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();


            // ========================================================
            // ACTIVE GAME
            // ============================================================

            if (!gameOver)
            {
                // ====================================================
                // TIMER
                // ====================================================

                gameTime += deltaTime;


                // ====================================================
                // EXPONENTIAL DIFFICULTY
                // ====================================================

                briefsSpeed =
                    startingBriefsSpeed *
                    MathF.Pow(
                        1.0f + accelerationRate,
                        gameTime
                    );

                briefsSpeed =
                    MathF.Min(
                        briefsSpeed,
                        maxBriefsSpeed
                    );


                // ====================================================
                // PLAYER MOVEMENT
                // ====================================================

                if (Raylib.IsKeyDown(KeyboardKey.Left) ||
                    Raylib.IsKeyDown(KeyboardKey.A))
                {
                    catcherPosition.X -=
                        catcherSpeed * deltaTime;
                }

                if (Raylib.IsKeyDown(KeyboardKey.Right) ||
                    Raylib.IsKeyDown(KeyboardKey.D))
                {
                    catcherPosition.X +=
                        catcherSpeed * deltaTime;
                }


                // ====================================================
                // PLAYER SCREEN BOUNDARIES
                // ====================================================

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


                // ====================================================
                // MOVE FALLING BRIEFS
                // ====================================================

                briefsPosition.Y +=
                    briefsSpeed * deltaTime;


                // ====================================================
                // COLLISION RECTANGLES
                // ====================================================

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


                // ====================================================
                // CATCH!
                // ====================================================

                if (Raylib.CheckCollisionRecs(
                    catcherRectangle,
                    briefsRectangle))
                {
                    score++;


                    // -----------------------------------------------
                    // FIRST CATCH
                    // -----------------------------------------------

                    if (!wearingBriefs)
                    {
                        wearingBriefs = true;
                    }


                    // -----------------------------------------------
                    // RESET FALLING BRIEFS
                    // -----------------------------------------------

                    briefsPosition.X =
                        Raylib.GetRandomValue(
                            40,
                            screenWidth -
                            (int)(briefs.Width * briefsScale) -
                            40
                        );

                    briefsPosition.Y = -100;
                }


                // ====================================================
                // MISS
                // ====================================================

                if (briefsPosition.Y > screenHeight)
                {
                    misses++;


                    // -----------------------------------------------
                    // GAME OVER?
                    // -----------------------------------------------

                    if (misses >= maxMisses)
                    {
                        gameOver = true;
                    }
                    else
                    {
                        briefsPosition.X =
                            Raylib.GetRandomValue(
                                40,
                                screenWidth -
                                (int)(briefs.Width * briefsScale) -
                                40
                            );

                        briefsPosition.Y = -100;
                    }
                }
            }


            // ========================================================
            // GAME OVER INPUT
            // ========================================================

            if (gameOver)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.R))
                {
                    // -----------------------------------------------
                    // RESET GAME DATA
                    // -----------------------------------------------

                    score = 0;
                    misses = 0;

                    gameTime = 0.0f;

                    briefsSpeed =
                        startingBriefsSpeed;

                    wearingBriefs = false;

                    gameOver = false;


                    // -----------------------------------------------
                    // RESET PLAYER
                    // -----------------------------------------------

                    catcherPosition = new Vector2(
                        screenWidth / 2.0f -
                        (catcherNaked.Width * catcherScale) / 2.0f,

                        screenHeight -
                        (catcherNaked.Height * catcherScale) -
                        35
                    );


                    // -----------------------------------------------
                    // RESET FALLING BRIEFS
                    // -----------------------------------------------

                    briefsPosition.X =
                        Raylib.GetRandomValue(
                            40,
                            screenWidth -
                            (int)(briefs.Width * briefsScale) -
                            40
                        );

                    briefsPosition.Y = -100;
                }
            }


            // ========================================================
            // SPEED DISPLAY
            // ========================================================

            int speedPercent =
                (int)(
                    briefsSpeed /
                    startingBriefsSpeed *
                    100.0f
                );


            // ========================================================
            // DRAW
            // ========================================================

            Raylib.BeginDrawing();

            Raylib.ClearBackground(
                new Color(20, 55, 60, 255)
            );


            // ========================================================
            // TITLE
            // ========================================================

            Raylib.DrawText(
                "UNDERWEAR CATCHER",
                220,
                25,
                32,
                Color.SkyBlue
            );


            // ========================================================
            // SCORE
            // ========================================================

            Raylib.DrawText(
                $"SCORE: {score}",
                30,
                75,
                22,
                Color.White
            );


            // ========================================================
            // SPEED
            // ========================================================

            string speedText =
                $"SPEED: {speedPercent}%";

            int speedTextWidth =
                Raylib.MeasureText(
                    speedText,
                    20
                );

            Raylib.DrawText(
                speedText,
                (screenWidth - speedTextWidth) / 2,
                77,
                20,
                Color.Yellow
            );


            // ========================================================
            // MISSES
            // ========================================================

            Raylib.DrawText(
                $"MISSES: {misses}/{maxMisses}",
                620,
                75,
                22,
                Color.White
            );


            // ========================================================
            // ACTIVE GAME DRAWING
            // ========================================================

            if (!gameOver)
            {
                // ====================================================
                // FALLING BRIEFS
                // ====================================================

                Raylib.DrawTextureEx(
                    briefs,
                    briefsPosition,
                    0.0f,
                    briefsScale,
                    Color.White
                );


                // ====================================================
                // PLAYER
                // ====================================================

                if (!wearingBriefs)
                {
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
                    Raylib.DrawTextureEx(
                        catcherCaught,
                        catcherPosition,
                        0.0f,
                        catcherScale,
                        Color.White
                    );
                }


                // ====================================================
                // INSTRUCTION
                // ====================================================

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


                // ====================================================
                // CONTROLS
                // ====================================================

                Raylib.DrawText(
                    "A/D OR ARROWS - MOVE",
                    275,
                    565,
                    18,
                    Color.Gray
                );
            }


            // ========================================================
            // GAME OVER SCREEN
            // ========================================================

            if (gameOver)
            {
                string gameOverText =
                    "GAME OVER";

                int gameOverWidth =
                    Raylib.MeasureText(
                        gameOverText,
                        50
                    );

                Raylib.DrawText(
                    gameOverText,
                    (screenWidth - gameOverWidth) / 2,
                    210,
                    50,
                    Color.Pink
                );


                // ----------------------------------------------------
                // FINAL SCORE
                // ----------------------------------------------------

                string finalScoreText =
                    $"FINAL SCORE: {score}";

                int finalScoreWidth =
                    Raylib.MeasureText(
                        finalScoreText,
                        30
                    );

                Raylib.DrawText(
                    finalScoreText,
                    (screenWidth - finalScoreWidth) / 2,
                    285,
                    30,
                    Color.White
                );


                // ----------------------------------------------------
                // FINAL SPEED
                // ----------------------------------------------------

                string finalSpeedText =
                    $"YOU REACHED {speedPercent}% SPEED";

                int finalSpeedWidth =
                    Raylib.MeasureText(
                        finalSpeedText,
                        20
                    );

                Raylib.DrawText(
                    finalSpeedText,
                    (screenWidth - finalSpeedWidth) / 2,
                    335,
                    20,
                    Color.Yellow
                );


                // ----------------------------------------------------
                // RESTART
                // ----------------------------------------------------

                string restartText =
                    "PRESS R TO TRY AGAIN";

                int restartWidth =
                    Raylib.MeasureText(
                        restartText,
                        22
                    );

                Raylib.DrawText(
                    restartText,
                    (screenWidth - restartWidth) / 2,
                    410,
                    22,
                    Color.SkyBlue
                );
            }


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