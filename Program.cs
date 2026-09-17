using System;
using System.Threading;

class Program
{
    static void Main()
    {
        const int width = 50;
        const int height = 22;

        // ============================================
        // PLAYER / CATCHER
        // ============================================

        int playerX = width / 2;
        const int playerWidth = 7;
        const int playerSpeed = 1;

        // ============================================
        // FALLING UNDERWEAR
        // ============================================

        int underwearX = Random.Shared.Next(2, width - 8);
        int underwearY = 3;

        // ============================================
        // SCORE
        // ============================================

        int score = 0;
        int misses = 0;

        Console.CursorVisible = false;

        // ============================================
        // GAME LOOP
        // ============================================

        while (true)
        {
            // ========================================
            // INPUT
            // ========================================

            int direction = 0;

            // Drain the keyboard buffer.
            // The newest direction pressed wins.
            while (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.LeftArrow)
                {
                    direction = -1;
                }

                if (key == ConsoleKey.RightArrow)
                {
                    direction = 1;
                }

                if (key == ConsoleKey.Escape)
                {
                    Console.CursorVisible = true;
                    return;
                }
            }

            playerX += direction * playerSpeed;

            // Keep catcher inside the screen
            playerX = Math.Clamp(
                playerX,
                0,
                width - playerWidth
            );

            // ========================================
            // UPDATE UNDERWEAR
            // ========================================

            underwearY++;

            // Has the underwear reached the catcher?
            if (underwearY >= height - 5)
            {
                bool caught =
                    underwearX + 6 >= playerX &&
                    underwearX <= playerX + playerWidth - 1;

                if (caught)
                {
                    score++;
                }
                else
                {
                    misses++;
                }

                // Spawn another pair
                underwearX = Random.Shared.Next(2, width - 8);
                underwearY = 3;
            }

            // ========================================
            // DRAW
            // ========================================

            Console.SetCursorPosition(0, 0);

            Console.WriteLine("UNDERWEAR CATCHER");
            Console.WriteLine(
                $"Score: {score}     Misses: {misses}"
            );
            Console.WriteLine();

            for (int y = 0; y < height; y++)
            {
                char[] line =
                    new string(' ', width).ToCharArray();

                // ====================================
                // DRAW FALLING BRIEFS
                // ====================================

                if (y == underwearY)
                {
                    DrawString(
                        line,
                        underwearX,
                        "|-----|"
                    );
                }

                if (y == underwearY + 1)
                {
                    DrawString(
                        line,
                        underwearX,
                        @" \   / "
                    );
                }

                if (y == underwearY + 2)
                {
                    DrawString(
                        line,
                        underwearX,
                        @"  \_/  "
                    );
                }

                // ====================================
                // DRAW CATCHER
                // ====================================

                if (y == height - 2)
                {
                    DrawString(
                        line,
                        playerX,
                        "[=====]"
                    );
                }

                Console.WriteLine(line);
            }

            Console.WriteLine();

            Console.WriteLine(
                "LEFT/RIGHT - Move     ESC - Quit"
            );

            // Roughly 10 updates per second
            Thread.Sleep(100);
        }
    }

    // ================================================
    // DRAW STRING HELPER
    // ================================================

    static void DrawString(
        char[] line,
        int x,
        string text
    )
    {
        for (int i = 0; i < text.Length; i++)
        {
            int position = x + i;

            if (position >= 0 &&
                position < line.Length)
            {
                line[position] = text[i];
            }
        }
    }
}