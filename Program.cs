using System;
using static Program;
using static Program.Cell;

class Program
{
    public class Board
    {
        internal int height;
        internal int width;
        private float enemySpawnKoef;

        internal Cell[,] Cells;

        internal Random random = new Random();

        public Board(int height, int width, float enemySpawnKoef)
        {
            this.height = height; this.width = width; Cells = new Cell[height, width]; this.enemySpawnKoef = enemySpawnKoef;
        }

        public void CellsInitialize()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i != height - 1 && i != height - 2 && i != height - 3 && random.Next(1,101) >= 100 - enemySpawnKoef)
                    {
                        Cells[i, j] = new Cell(Cell.CellType.enemy);
                        Cells[i, j].enemyHealth = 3;
                    }
                    else if (i == height - 1 && j == width/2) Cells[i, j] = new Cell(Cell.CellType.player);

                    else Cells[i, j] = new Cell(Cell.CellType.invisible);
                }
            }
        }

        public void DrawBoard()
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Console.Write(Cells[i, j].view);
                }
                Console.Write("\n");
            }
        }
    }

    public class Cell
    {
        public enum CellType
        {
            invisible,
            enemy,
            player,
            explosion
        }

        internal string? view;

        internal int enemyHealth;

        internal int playerHealth;

        internal CellType? cellType;

        public Cell(CellType cellType)
        {
            this.cellType = cellType;
            initializeView(cellType);
        }

        public void initializeView(CellType cellType)
        {
            switch (cellType)
            {
                case CellType.invisible:
                    view = "  ";
                    break;
                case CellType.enemy:
                    view = "👾";
                    break;
                case CellType.player:
                    view = "🚀";
                    break;
                case CellType.explosion:
                    view = "💥";
                    break;
                default:
                    view = "  ";
                    break;
            }
        }
    }

    class Enemy : Cell
    {
        private Board board;

        public Enemy(Board board) : base(CellType.enemy)
        {
            this.board = board;
        }

        public void EnemyControl()
        {
            while (true)
            {
                for (int i = 0; i < board.height; i++)
                {
                    for (int j = 0; j < board.width; j++)
                    {
                        if (board.Cells[i, j].cellType == CellType.enemy)
                        {
                            if (board.random.Next(1, 101) >= 50)
                            {
                                if (j > (board.width - board.width) + 2)
                                {
                                    if (board.Cells[i, j - 1].cellType == CellType.invisible && board.Cells[i, j - 2].cellType == CellType.invisible)
                                    {
                                        board.Cells[i, j].cellType = CellType.invisible;
                                        board.Cells[i, j].initializeView(CellType.invisible);
                                        board.Cells[i, j - 1].cellType = CellType.enemy;
                                        board.Cells[i, j - 1].enemyHealth = board.Cells[i, j].enemyHealth;
                                        board.Cells[i, j - 1].initializeView(CellType.enemy);
                                    }
                                }
                            }
                            else
                            {
                                if (j < board.width - 2)
                                {
                                    if (board.Cells[i, j + 1].cellType == CellType.invisible && board.Cells[i, j + 2].cellType == CellType.invisible)
                                    {
                                        board.Cells[i, j].cellType = CellType.invisible;
                                        board.Cells[i, j].initializeView(CellType.invisible);
                                        board.Cells[i, j + 1].cellType = CellType.enemy;
                                        board.Cells[i, j + 1].enemyHealth = board.Cells[i, j].enemyHealth;
                                        board.Cells[i, j + 1].initializeView(CellType.enemy);
                                    }
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
    }

    class Player
    {
        private Board board;

        private int playerX;
        private int playerY;

        public Player(Board board)
        {
            this.board = board;
        }

        internal void PlayerControl()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    LocatePlayer();
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.RightArrow)
                    {
                        PlayerMove("right");
                    }
                    if (key.Key == ConsoleKey.LeftArrow)
                    {
                        PlayerMove("left");
                    }
                }
            }
        }

        private void LocatePlayer()
        {
            for (int i = 0; i < board.height; i++)
            {
                for (int j = 0; j < board.width; j++)
                {
                    if (board.Cells[i, j].cellType == CellType.player)
                    {
                        playerX = i;
                        playerY = j;
                    }
                }
            }
        }

        private void PlayerMove(string direction)
        {
            if (direction == "right" && playerY < board.width - 1)
            {
                board.Cells[playerX, playerY].cellType = CellType.invisible;
                board.Cells[playerX, playerY].initializeView(CellType.invisible);

                board.Cells[playerX, playerY + 1].cellType = CellType.player;
                board.Cells[playerX, playerY + 1].playerHealth = board.Cells[playerX, playerY].playerHealth;
                board.Cells[playerX, playerY + 1].initializeView(CellType.player);
            }
            else if (direction == "left" && playerY != 0)
            {
                board.Cells[playerX, playerY].cellType = CellType.invisible;
                board.Cells[playerX, playerY].initializeView(CellType.invisible);

                board.Cells[playerX, playerY - 1].cellType = CellType.player;
                board.Cells[playerX, playerY - 1].playerHealth = board.Cells[playerX, playerY].playerHealth;
                board.Cells[playerX, playerY - 1].initializeView(CellType.player);
            }
        }
    }

    static void Main(string[] args)
    {
        // 🚀 🔸 👾 💥
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        Board board = new Board(10, 21, 4.5f);
        board.CellsInitialize();

        Enemy enemy = new Enemy(board);
        Player player = new Player(board);

        Thread t1 = new Thread(player.PlayerControl);
        Thread t2 = new Thread(enemy.EnemyControl);
        t1.Start();
        t2.Start();

        while (true)
        {
            board.DrawBoard();
            Thread.Sleep(1);
        }
    }
}
