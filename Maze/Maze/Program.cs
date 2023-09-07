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

        private Random rand;

        private int coins;

        direction dir;

        Cell[,] _cells;

        private int amountOfWalls;
        private int amountOfCoins;

        private int _height;
        private int _width;

        private int fps;

        //X Y for Player
        private (int,int)[] _playerPos;

        private int frameCounter;
        private long time;
        private long checkTime, newCheckTime;
        private ConsoleKey key;
        private StringBuilder String;

        public Maze()
        {
            _width = 120;
            _height = 30;

            //Start
            Tutorial();

            //Setup
            rand = new Random();

            _cells = new Cell[_width,_height];
            for (int i = 0; i < _width; i++)
            {
                for (int x = 0; x < _height; x++)
                {
                    _cells[i, x] = new Cell();
                }
            }

            _playerPos = new (int, int)[2];

            coins = 0;

            frameCounter = 0;

            time = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond * 1000);
            checkTime = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond * 10);

            //X Y for Player
            _playerPos[0] = (_width / 2, _height / 2);

            //Walls
            GenerateWalls();

            //Coins
            GenerateCoins();

            //Screen setup
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
            string text2 = "Монети це '*'";
            string text3 = "Стiна '#'";
            string text4 = "Гравець: '@'";
            string text5 = $"Ведiть число стiнок до {_height * _width - 10}";

            string[] texts = { text1, text2, text3, text4, text5 };

            for (int i = 0; i < texts.Length; i++)
            {
                Console.SetCursorPosition(_width / 2 - text1.Length / 2, _height / 2 + i);

                WriteText(texts[i],50, ConsoleColor.Green);
            }

            Console.SetCursorPosition(_width / 2 - text1.Length / 2, _height / 2 + 5);
            amountOfWalls = Convert.ToInt32(Console.ReadLine());

            Console.SetCursorPosition(_width / 2 - text1.Length / 2, _height / 2 + 6);
            string text6 = $"Ведiть число монет до {_height * _width - 10 - amountOfWalls}";
            WriteText(text6, 50, ConsoleColor.Green);

            Console.SetCursorPosition(_width / 2 - text1.Length / 2, _height / 2 + 7);
            amountOfCoins = Convert.ToInt32(Console.ReadLine());
        }

        void WriteText(string text, int delayPerLetter, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            for (int x = 0; x < text.Length; x++)
            {
                System.Threading.Thread.Sleep(50);
                Console.Write(text[x]);
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        void GenerateWalls()
        {
            if (amountOfWalls < 0 || amountOfWalls > _height * _width - 10)
            {
                amountOfWalls = 200;
            }

            int x, y;

            for (int i = 0; i < amountOfWalls; i++)
            {
                do { 
                    x = rand.Next(0, _width);
                    y = rand.Next(0, _height);
                } while (_cells[x, y]._cell != cellType.none);

                _cells[x, y]._cell = cellType.wall;

                string text = amountOfWalls + "/" + (i + 1);

                Console.SetCursorPosition(_width / 2 - text.Length / 2, _height / 2 + 5);
                Console.Write(text);
            }
        }

        void GenerateCoins()
        {
            if (amountOfCoins < 0 || amountOfCoins > _height * _width - amountOfWalls)
            {
                coins = 5;
            }

            for (int i = 0; i < amountOfCoins; i++)
            {
                int x, y;

                do
                {
                    x = rand.Next(0, _width);
                    y = rand.Next(0, _height);
                } while (_cells[x, y]._cell != cellType.none);

                _cells[x, y]._cell = cellType.coin;

                string text = amountOfCoins + "/" + (i + 1);

                Console.SetCursorPosition(_width / 2 - text.Length / 2, _height / 2 + 7);
                Console.Write(text);
            }
        }

        void End()
        {
            Console.Clear();

            string text1 = "Ти вмер";
            string text2 = "Натисни любу кнопку щоб закiнчити";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(_width / 2 - text1.Length / 2, _height / 2 - 1);

            for (int i = 0; i < text1.Length; i++)
            {
                System.Threading.Thread.Sleep(200);
                Console.Write(text1[i]);
            }

            Console.SetCursorPosition(_width / 2 - text2.Length / 2, _height / 2);

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
                _playerPos[0].Item1++;
            }
            else if (dir == direction.Left)
            {
                _playerPos[0].Item1--;
            }
            else if (dir == direction.Down)
            {
                _playerPos[0].Item2++;
            }
            else if (dir == direction.Up)
            {
                _playerPos[0].Item2--;
            }
        }

        void Check()
        {
            if (_playerPos[0].Item1 < 0 && dir == direction.Left)
            {
                _playerPos[0].Item1 = _playerPos[1].Item1;
            }
            else if (_playerPos[0].Item2 < 0 && dir == direction.Up)
            {
                _playerPos[0].Item2 = _playerPos[1].Item2;
            }
            else if (_playerPos[0].Item1 == _width && dir == direction.Right)
            {
                _playerPos[0].Item1 = _playerPos[1].Item1;
            }
            else if (_playerPos[0].Item2 == _height && dir == direction.Down)
            {
                _playerPos[0].Item2 = _playerPos[1].Item2;
            }

            if (_cells[_playerPos[0].Item1,_playerPos[0].Item2]._cell == cellType.wall)
            {
                _playerPos[0].Item1 = _playerPos[1].Item1;
                _playerPos[0].Item2 = _playerPos[1].Item2;
            }
        }

        void CheckCoins()
        {
            if (_cells[_playerPos[0].Item1,_playerPos[0].Item2]._cell == cellType.coin)
            {
                _cells[_playerPos[0].Item1, _playerPos[0].Item2]._cell = cellType.none;
                coins++;
            }
        }

        void SavePlayerPosition()
        {
            _playerPos[1] = _playerPos[0];
        }

        void SetupBuildStr()
        {
            String = new StringBuilder();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_cells[x, y]._cell == cellType.wall)
                    {
                        String.Append('#');
                    }
                    else if (_cells[x, y]._cell == cellType.coin)
                    {
                        String.Append('*');
                    }
                    else
                    {
                        String.Append(' ');
                    }
                }
            }
        }

        void UpdateStringBuilder()
        {
            //Clear previous position
            String[_playerPos[1].Item1 + _playerPos[1].Item2 * _width] = ' ';

            //Fps
            Show(fps, 0);

            //Coins
            Show(coins, _width);

            //Player
            String[_playerPos[0].Item1 + _playerPos[0].Item2 * _width] = '@';
        }

        void Show(int value,int place)
        {
            if (value < 10)
            {
                String[place] = (char)(value + 48);
            }
            else if (value < 100)
            {
                String[place] = (char)(value / 10 + 48);
                String[place + 1] = (char)(value % 10 + 48);
            }
            else if (value < 1000)
            {
                String[place] = (char)(value / 100 + 48);
                String[place + 1] = (char)(value / 10 % 10 + 48);
                String[place + 2] = (char)(value % 10 + 48);
            }
            else if (value < 10000)
            {
                String[place] = (char)(value / 1000 + 48);
                String[place + 1] = (char)(value / 100 % 10 + 48);
                String[place + 2] = (char)(value / 10 % 10 + 48);
                String[place + 3] = (char)(value % 10 + 48);
            }
        }
    }

    class Cell
    {
        public Cell()
        {
            _cell = cellType.none;
        }

        public cellType _cell;
    }

    public enum cellType
    {
        none,
        coin,
        wall,
        player
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
