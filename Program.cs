using Raylib_cs;
using System;
using System.IO;
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
        // GAME STATE
        // ============================================================

        bool titleScreen = true;
        bool gameOver = false;
        bool resetHighScoreConfirm = false;


        // ============================================================
        // HIGH SCORE SAVE
        // ============================================================

        string saveFile = "highscore.txt";

        int highScore = 0;

        // Try to load an existing high score.
        if (File.Exists(saveFile))
        {
            string savedText =
                File.ReadAllText(saveFile);

            if (int.TryParse(savedText, out int savedScore))
            {
                highScore = savedScore;
            }
        }


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


        // ============================================================
        // GAME LOOP
        // ============================================================

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();


            // ========================================================
            // TITLE SCREEN INPUT
            // ========================================================

            if (titleScreen)
            {
                // DELETE = reset high score
                if (Raylib.IsKeyPressed(KeyboardKey.Delete))
                {
                    if (!resetHighScoreConfirm)
                    {
                        resetHighScoreConfirm = true;
                    }
                    else
                    {
                        highScore = 0;

                        File.WriteAllText(
                            saveFile,
                            highScore.ToString()
                        );

                        resetHighScoreConfirm = false;
                    }
                }

                // ENTER = start game
                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    resetHighScoreConfirm = false;

                    titleScreen = false;
                    gameOver = false;

                    score = 0;
                    misses = 0;

                    gameTime = 0.0f;
                    briefsSpeed = startingBriefsSpeed;

                    wearingBriefs = false;

                    catcherPosition = new Vector2(
                        screenWidth / 2.0f -
                        (catcherNaked.Width * catcherScale) / 2.0f,

                        screenHeight -
                        (catcherNaked.Height * catcherScale) -
                        35
                    );

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
            // ACTIVE GAME
            // ========================================================

            if (!titleScreen && !gameOver)
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


                    // Update high score immediately.
                    if (score > highScore)
                    {
                        highScore = score;

                        File.WriteAllText(
                            saveFile,
                            highScore.ToString()
                        );
                    }


                    // First successful catch puts the briefs on.
                    if (!wearingBriefs)
                    {
                        wearingBriefs = true;
                    }


                    // Reset falling briefs.
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
                // R = restart immediately.
                if (Raylib.IsKeyPressed(KeyboardKey.R))
                {
                    score = 0;
                    misses = 0;

                    gameTime = 0.0f;
                    briefsSpeed = startingBriefsSpeed;

                    wearingBriefs = false;
                    gameOver = false;

                    catcherPosition = new Vector2(
                        screenWidth / 2.0f -
                        (catcherNaked.Width * catcherScale) / 2.0f,

                        screenHeight -
                        (catcherNaked.Height * catcherScale) -
                        35
                    );

                    briefsPosition.X =
                        Raylib.GetRandomValue(
                            40,
                            screenWidth -
                            (int)(briefs.Width * briefsScale) -
                            40
                        );

                    briefsPosition.Y = -100;
                }


                // ENTER = return to title.
                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    gameOver = false;
                    titleScreen = true;
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
            // TITLE PULSE
            // ========================================================

            // Produces a smooth value between 0 and 1.
            float pulse =
                (MathF.Sin(
                    (float)Raylib.GetTime() * 3.0f
                ) + 1.0f) / 2.0f;

            // Keep it visible even at the dimmest point.
            byte pulseAlpha =
                (byte)(120 + pulse * 135);

            Color pulseColor =
     new Color(
         (byte)255,
         (byte)255,
         (byte)255,
         pulseAlpha
     );


            // ========================================================
            // DRAW
            // ========================================================

            Raylib.BeginDrawing();

            Raylib.ClearBackground(
                new Color(20, 55, 60, 255)
            );


            // ========================================================
            // TITLE SCREEN
            // ========================================================

            if (titleScreen)
            {
                // ----------------------------------------------------
                // STREWN BRIEFS
                // Deliberately messy and asymmetrical.
                // ----------------------------------------------------

                Raylib.DrawTextureEx(
                    briefs,
                    new Vector2(55, 105),
                    -27.0f,
                    1.35f,
                    Color.White
                );

                Raylib.DrawTextureEx(
                    briefs,
                    new Vector2(665, 155),
                    13.0f,
                    0.85f,
                    Color.White
                );

                Raylib.DrawTextureEx(
                    briefs,
                    new Vector2(135, 315),
                    31.0f,
                    0.75f,
                    Color.White
                );

                Raylib.DrawTextureEx(
                    briefs,
                    new Vector2(610, 370),
                    -11.0f,
                    1.25f,
                    Color.White
                );

                Raylib.DrawTextureEx(
                    briefs,
                    new Vector2(245, 470),
                    -38.0f,
                    0.95f,
                    Color.White
                );

                Raylib.DrawTextureEx(
                    briefs,
                    new Vector2(705, 500),
                    24.0f,
                    0.65f,
                    Color.White
                );

                Raylib.DrawTextureEx(
                    briefs,
                    new Vector2(525, 105),
                    42.0f,
                    0.55f,
                    Color.White
                );


                // ----------------------------------------------------
                // TITLE
                // ----------------------------------------------------

                string titleText =
                    "UNDERWEAR CATCHER";

                int titleWidth =
                    Raylib.MeasureText(
                        titleText,
                        42
                    );

                Raylib.DrawText(
                    titleText,
                    (screenWidth - titleWidth) / 2,
                    80,
                    42,
                    Color.SkyBlue
                );


                // ----------------------------------------------------
                // TAGLINE
                // ----------------------------------------------------

                string tagline =
                    "CATCH 'EM BEFORE THEY DROP!";

                int taglineWidth =
                    Raylib.MeasureText(
                        tagline,
                        18
                    );

                Raylib.DrawText(
                    tagline,
                    (screenWidth - taglineWidth) / 2,
                    140,
                    18,
                    Color.Pink
                );


                // ----------------------------------------------------
                // CAUGHT TORSO
                // ----------------------------------------------------

                const float titleTorsoScale = 2.5f;

                Vector2 titleTorsoPosition =
                    new Vector2(
                        screenWidth / 2.0f -
                        (catcherCaught.Width *
                        titleTorsoScale) / 2.0f,

                        195
                    );

                Raylib.DrawTextureEx(
                    catcherCaught,
                    titleTorsoPosition,
                    0.0f,
                    titleTorsoScale,
                    Color.White
                );


                // ----------------------------------------------------
                // HIGH SCORE
                // ----------------------------------------------------

                string highScoreText =
                    $"HIGH SCORE: {highScore}";

                int highScoreWidth =
                    Raylib.MeasureText(
                        highScoreText,
                        24
                    );

                Raylib.DrawText(
                    highScoreText,
                    (screenWidth - highScoreWidth) / 2,
                    355,
                    24,
                    Color.Yellow
                );


                // ----------------------------------------------------
                // START - PULSING
                // ----------------------------------------------------

                string startText =
                    "PRESS ENTER TO PLAY";

                int startWidth =
                    Raylib.MeasureText(
                        startText,
                        26
                    );

                Raylib.DrawText(
                    startText,
                    (screenWidth - startWidth) / 2,
                    440,
                    26,
                    pulseColor
                );


                // ----------------------------------------------------
                // RULES
                // ----------------------------------------------------

                string ruleText =
                    "3 MISSES = GAME OVER";

                int ruleWidth =
                    Raylib.MeasureText(
                        ruleText,
                        18
                    );

                    string resetText =
    resetHighScoreConfirm
    ? "PRESS DELETE AGAIN TO RESET"
    : "DELETE - RESET HIGH SCORE";

                    int resetWidth =
                        Raylib.MeasureText(
                            resetText,
                            16
                        );

                    Raylib.DrawText(
                        resetText,
                        (screenWidth - resetWidth) / 2,
                        530,
                        16,
                        resetHighScoreConfirm
                            ? Color.Pink
                            : Color.Gray
                    );

                    Raylib.DrawText(
                    ruleText,
                    (screenWidth - ruleWidth) / 2,
                    495,
                    18,
                    Color.Gray
                );
            }


            // ========================================================
            // GAMEPLAY
            // ========================================================

            if (!titleScreen && !gameOver)
            {
                // ====================================================
                // TITLE
                // ====================================================

                Raylib.DrawText(
                    "UNDERWEAR CATCHER",
                    220,
                    25,
                    32,
                    Color.SkyBlue
                );


                // ====================================================
                // SCORE
                // ====================================================

                Raylib.DrawText(
                    $"SCORE: {score}",
                    30,
                    75,
                    22,
                    Color.White
                );


                // ====================================================
                // SPEED
                // ====================================================

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


                // ====================================================
                // MISSES
                // ====================================================

                Raylib.DrawText(
                    $"MISSES: {misses}/{maxMisses}",
                    620,
                    75,
                    22,
                    Color.White
                );


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
                // ----------------------------------------------------
                // HUD
                // ----------------------------------------------------

                Raylib.DrawText(
                    "UNDERWEAR CATCHER",
                    220,
                    25,
                    32,
                    Color.SkyBlue
                );

                Raylib.DrawText(
                    $"SCORE: {score}",
                    30,
                    75,
                    22,
                    Color.White
                );

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

                Raylib.DrawText(
                    $"MISSES: {misses}/{maxMisses}",
                    620,
                    75,
                    22,
                    Color.White
                );


                // ----------------------------------------------------
                // GAME OVER
                // ----------------------------------------------------

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
                    190,
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
                    265,
                    30,
                    Color.White
                );


                // ----------------------------------------------------
                // HIGH SCORE
                // ----------------------------------------------------

                string highScoreText =
                    $"HIGH SCORE: {highScore}";

                int highScoreWidth =
                    Raylib.MeasureText(
                        highScoreText,
                        24
                    );

                Raylib.DrawText(
                    highScoreText,
                    (screenWidth - highScoreWidth) / 2,
                    315,
                    24,
                    Color.Yellow
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
                    360,
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
                    430,
                    22,
                    Color.SkyBlue
                );


                // ----------------------------------------------------
                // TITLE RETURN
                // ----------------------------------------------------

                string titleReturnText =
                    "PRESS ENTER FOR TITLE";

                int titleReturnWidth =
                    Raylib.MeasureText(
                        titleReturnText,
                        18
                    );

                Raylib.DrawText(
                    titleReturnText,
                    (screenWidth - titleReturnWidth) / 2,
                    475,
                    18,
                    Color.Gray
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