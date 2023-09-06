using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    #region Program
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            var Maze = new Maze();
            Maze.Start();
        }
    }
    #endregion

    #region Maze
    public class Maze: GameLoop
    {
        private string text;

        private Random rand;

        private int apples;
        private int direction;

        private int Xapple;
        private int Yapple;

        private int amount;

        private int fps = 0;

        //Cursor Position
        private int CursorPositionX;
        private int CursorPositionY;

        //X Y for Cursor
        private int[] X;
        private int[] Y;

        private char marker;
        private int frameCounter;
        private long time;
        private int[] XWall, YWall;
        private ConsoleKey key;
        private StringBuilder String;

        public Maze()
        {
            text = "У тебе :   яблок";

            rand = new Random();

            apples = 0;
            direction = 0;

            Tutorial();

            amount = Convert.ToInt32(Console.ReadLine());

            //Cursor Position
            CursorPositionX = Console.WindowWidth / 2;
            CursorPositionY = Console.WindowHeight / 2;

            //X Y for Cursor
            X = new int[10];
            Y = new int[10];

            X[0] = CursorPositionX;
            Y[0] = CursorPositionY;

            //Create Walls
            Walls();

            //Apple
            Xapple = rand.Next(0, Console.WindowWidth);
            Yapple = rand.Next(0, Console.WindowHeight);

            while (Xapple == X[0])
            {
                Xapple = rand.Next(0, Console.WindowWidth);
            }
            while (Yapple == Y[0])
            {
                Yapple = rand.Next(0, Console.WindowHeight);
            }

            //Some other things
            marker = ' ';
            frameCounter = 0;
            time = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond * 1000);
        }

        protected override void Input()
        {
            key = ConsoleKey.NoName;

            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(true).Key;
            }

        }

        protected override void Update(out bool Death)
        {
            Death = false;
            
            Move();

            X[0] = CursorPositionX;
            Y[0] = CursorPositionY;


            Swap();

            if (Check())
            {
                End();
                Death = true;
                //break;
            }

            CheckApple();

            //Frame 

            frameCounter++;
            var newTime = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond * 1000);

            if (time != newTime)
            {
                time = newTime;
                fps = frameCounter;

                //timeToWait = frameCounter * 100;
                //timeToWait = 100;

                frameCounter = 0;
            }

            SetupBuildStr();
        }

        protected override void Draw()
        {
            Console.Write(String);
            Console.SetCursorPosition(0, 0);
        }

        void Tutorial()
        {
            string text1 = "Щоб рухатись використовуйте стрiлки";
            string text2 = "Яблуко це '*'";
            string text3 = "Стiна '#'";
            string text4 = "Голова змiйки: ':' та тiло 'O'";
            string text5 = $"Ведiть число стiнок до {Console.WindowHeight * Console.WindowWidth - 10}";

            string[] texts = { text1, text2, text3, text4, text5 };

            Console.ForegroundColor = ConsoleColor.Green;

            for (int i = 0; i < texts.Length; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - text1.Length / 2, Console.WindowHeight / 2 + i);

                for (int x = 0; x < texts[i].Length; x++)
                {
                    System.Threading.Thread.Sleep(50);
                    Console.Write(texts[i][x]);
                }
            }
            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(Console.WindowWidth / 2 - text1.Length / 2, Console.WindowHeight / 2 + 5);
        }

        void Walls()
        {
            if (amount < 0 ^ amount > Console.WindowHeight * Console.WindowWidth - 10)
            {
                amount = 0;
            }

            XWall = new int[amount];
            YWall = new int[amount];

            for (int i = 0; i < amount; i++)
            {
                XWall[i] = rand.Next(0, Console.WindowWidth);
                YWall[i] = rand.Next(0, Console.WindowHeight);
            }

            //Check
            for (int i = 0; i < XWall.Length; i++)
            {
                for (int y = 1; y < XWall.Length; y++)
                {
                    while (true)
                    {
                        if (XWall[i] == XWall[y] && YWall[i] == YWall[y] && i != y)
                        {
                            XWall[y] = rand.Next(0, Console.WindowWidth);
                            YWall[y] = rand.Next(0, Console.WindowHeight);
                        }
                        else if (XWall[y] > Console.WindowWidth / 2 - 2 && XWall[y] < Console.WindowWidth / 2 + 2 && YWall[y] > Console.WindowHeight / 2 - 2 && YWall[y] < Console.WindowHeight / 2 + 2)
                        {
                            XWall[y] = rand.Next(0, Console.WindowWidth);
                            YWall[y] = rand.Next(0, Console.WindowHeight);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                string text = amount + "/" + i;

                Console.SetCursorPosition(Console.WindowWidth / 2 - text.Length / 2, Console.WindowHeight / 2 + 5);
                Console.Write(amount + "/" + i);
            }
        }

        void End()
        {
            Console.Clear();

            string text1 = "Ти вмер";
            string text2 = "Натисни любу кнопку щоб закiнчити";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(Console.WindowWidth / 2 - text1.Length / 2, Console.WindowHeight / 2 - 1);

            for (int i = 0; i < text1.Length; i++)
            {
                System.Threading.Thread.Sleep(200);
                Console.Write(text1[i]);
            }

            Console.SetCursorPosition(Console.WindowWidth / 2 - text2.Length / 2, Console.WindowHeight / 2);

            for (int i = 0; i < text2.Length; i++)
            {
                System.Threading.Thread.Sleep(200);
                Console.Write(text2[i]);
            }

            Console.ReadKey();
        }

        void Move()
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    direction = 4;
                    break;
                case ConsoleKey.DownArrow:
                    direction = 3;
                    break;
                case ConsoleKey.LeftArrow:
                    direction = 2;
                    break;
                case ConsoleKey.RightArrow:
                    direction = 1;
                    break;
            }

            if (direction == 1)
            {
                CursorPositionX++;
            }
            else if (direction == 2)
            {
                CursorPositionX--;
            }
            else if (direction == 3)
            {
                CursorPositionY++;
            }
            else if (direction == 4)
            {
                CursorPositionY--;
            }
        }

        bool Check()
        {
            if (CursorPositionX < 0 && direction == 2)
            {
                return true;
            }
            else if (CursorPositionY < 0 && direction == 4)
            {
                return true;
            }
            else if (CursorPositionX == 120 && direction == 1)
            {
                return true;
            }
            else if (CursorPositionY == 30 && direction == 3)
            {
                return true;
            }

            for (int i = 2; i < apples + 2; i++)
            {
                if (X[1] == X[i])
                {
                    for (int x = 2; x < apples; x++)
                    {
                        if (Y[1] == Y[i])
                        {
                            return true;
                        }
                    }
                }
            }

            for (int i = 0; i < XWall.Length; i++)
            {
                if (X[1] == XWall[i] && Y[1] == YWall[i])
                {
                    return true;
                }
            }

            return false;
        }

        void CheckApple() { 

            if (X[1] == Xapple && Y[1] == Yapple)
            {
                apples++;

                bool IsAppleEated = true;

                while (IsAppleEated == true)
                {
                    IsAppleEated = false;

                    Xapple = rand.Next(0, Console.WindowWidth);
                    Yapple = rand.Next(0, Console.WindowHeight);

                    for (int x = 1; x < apples + 2; x++)
                    {
                        if (Yapple == Y[x] && Xapple == X[x])
                        {
                            IsAppleEated = true;
                            break;
                        }
                    }
                }
            }
        }

        void Swap()
        {
            int[] Xsaver = new int[apples + 3];

            for (int i = 1; i < apples + 2; i++)
            {
                Xsaver[i] = X[i - 1];
            }

            X = Xsaver;

            int[] Ysaver = new int[apples + 3];

            for (int i = 1; i < apples + 2; i++)
            {
                Ysaver[i] = Y[i - 1];
            }

            Y = Ysaver;

            X[0] = 0;
            Y[0] = 0;
        }

        void SetupBuildStr()
        {
            String = new StringBuilder();

            //Empty
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                String.Append(' ', Console.WindowWidth);
            }

            //Board
            int textPosition = 60;

            for (int i = 0; i < text.Length; i++)
            {
                if (i == 9)
                {
                    String[textPosition] = Convert.ToChar(apples.ToString());
                }
                else
                {
                    String[textPosition] = text[i];
                }

                textPosition++;
            }

            //Walls
            for (int i = 0; i < XWall.Length; i++)
            {
                String[XWall[i] + YWall[i] * Console.WindowWidth] = '#';
            }

            //Apple
            String[Xapple + Yapple * Console.WindowWidth] = '*';

            //Snake
            for (int i = 1; i < apples + 2; i++)
            {
                if (i == 1)
                {
                    String[X[i] + Y[i] * Console.WindowWidth] = ':';
                }
                else
                {
                    String[X[i] + Y[i] * Console.WindowWidth] = 'O';
                }
            }

            //fps
            String[0] = (char)(Math.Floor(Convert.ToDecimal(fps / 100 + 48)));
            String[1] = (char)(Math.Floor(Convert.ToDecimal(fps / 10 % 10 + 48)));
            String[2] = (char)(fps % 100 % 10 + 48);
        }
    }
    #endregion

    #region GameLoop Library
    public abstract class GameLoop
    {
        public void Start()
        {
            bool Death = false;
            while (Death == false)
            {
                Timing();
                Input();
                Update(out Death);
                Draw();
            }
        }

        private void Timing()
        {
            //System.Threading.Thread.Sleep(50);
        }
        protected abstract void Input();
        protected abstract void Update(out bool Death);
        protected abstract void Draw();
    }
    #endregion
}
