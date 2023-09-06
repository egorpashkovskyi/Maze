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
        enum direction {
            None,
            Up,
            Down,
            Left,
            Right
        }

        private string text;

        private Random rand;

        private int coins;

        direction dir;

        private int[] coinsX;
        private int[] coinsY;

        private int amount;

        private int fps;

        //X Y for Player
        private int[] X;
        private int[] Y;

        private char marker;
        private int frameCounter;
        private long time;
        private long checkTime, newCheckTime;
        private int[] XWall, YWall;
        private ConsoleKey key;
        private StringBuilder String;

        public Maze()
        {
            //Start
            Tutorial();
            amount = Convert.ToInt32(Console.ReadLine());

            //Random
            rand = new Random();

            //X Y for Player
            X = new int[2];
            Y = new int[2];

            X[0] = Console.WindowWidth / 2;
            Y[0] = Console.WindowHeight / 2;

            //Create Walls
            Walls();

            //Coins
            coinsX = new int[5];
            coinsY = new int[5];

            for (int i = 0; i < coinsX.Length; i++)
            {
                do {
                    coinsX[i] = rand.Next(0, Console.WindowWidth);
                } while (coinsX[i] == X[0]);

                do
                {
                    coinsY[i] = rand.Next(0, Console.WindowHeight);
                } while (coinsY[i] == Y[0]);
            }


            //Some other things
            coins = 0;

            marker = ' ';

            frameCounter = 0;

            time = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond * 1000);
            checkTime = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond * 10);

            text = "";

            SetupBuildStr();
        }

        protected override void Input()
        {
            key = ConsoleKey.NoName;

            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(true).Key;
            }

        }

        protected override void Update(out bool end)
        {
            end = false;

            SavePlayerPosition();

            if (CheckTime())
            {
                Move();
            }

            Check();

            CheckCoins();

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

            UpdateStringBuilder();
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

        bool CheckTime()
        {
            newCheckTime = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond * 10);

            return newCheckTime - checkTime > 2;
        }

        void Move()
        {
            //Move
            dir = direction.None;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    dir = direction.Up;
                    break;
                case ConsoleKey.DownArrow:
                    dir = direction.Down;
                    break;
                case ConsoleKey.LeftArrow:
                    dir = direction.Left;
                    break;
                case ConsoleKey.RightArrow:
                    dir = direction.Right;
                    break;
            }

            if (dir != direction.None)
                checkTime = newCheckTime;

            if (dir == direction.Right)
            {
                X[0]++;
            }
            else if (dir == direction.Left)
            {
                X[0]--;
            }
            else if (dir == direction.Down)
            {
                Y[0]++;
            }
            else if (dir == direction.Up)
            {
                Y[0]--;
            }
        }

        void Check()
        {
            if (X[0] < 0 && dir == direction.Left)
            {
                X[0] = X[1];
            }
            else if (Y[0] < 0 && dir == direction.Up)
            {
                Y[0] = Y[1];
            }
            else if (X[0] == Console.WindowWidth && dir == direction.Right)
            {
                X[0] = X[1];
            }
            else if (Y[0] == Console.WindowHeight && dir == direction.Down)
            {
                Y[0] = Y[1];
            }

            for (int i = 0; i < XWall.Length; i++)
            {
                if (X[0] == XWall[i] && Y[0] == YWall[i])
                {
                    X[0] = X[1];
                    Y[0] = Y[1];
                }
            }
        }

        void CheckCoins()
        {
            for (int i = 0; i < coinsX.Length; i++)
            {
                if (X[1] == coinsX[i] && Y[1] == coinsY[i])
                {
                    coins++;

                    coinsX[i] = 0;
                    coinsY[i] = 0;  
                }
            }

        }

        void SavePlayerPosition()
        {
            X[1] = X[0];
            Y[1] = Y[0];
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
                    String[textPosition] = Convert.ToChar(coins.ToString());
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

            //Coins
            for (int i = 0; i < coinsX.Length; i++)
            {
                String[coinsX[i] + coinsY[i] * Console.WindowWidth] = '*';
            }
        }

        void UpdateStringBuilder()
        {
            //Clear previous position
            String[X[1] + Y[1] * Console.WindowWidth] = ' ';

            //Fps
            String[0] = (char)(fps / 100 + 48);
            String[1] = (char)(fps / 10 % 10 + 48);
            String[2] = (char)(fps % 100 % 10 + 48);

            //Coins
            String[Console.WindowWidth] = (char)(coins + 48);

            //Player
            String[X[0] + Y[0] * Console.WindowWidth] = '@';
        }
    }
    #endregion

    #region GameLoop Library
    public abstract class GameLoop
    {
        public void Start()
        {
            bool end = false;
            while (!end)
            {
                Timing();
                Input();
                Update(out end);
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
