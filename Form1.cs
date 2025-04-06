using DraughtsGame.Properties;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Common;
using System;
using System.Diagnostics;



namespace DraughtsGame
{
    public partial class Form1 : Form
    {

        public int[,] GameState;
        bool WhiteTurn = true;
        

        List<PictureBox> boxes = new List<PictureBox>();
        List<Move> possibleMoves = new List<Move>();
        List<Pair> highLight = new List<Pair>();




        List<Task> tasks = new List<Task>();

        private Move bestMove = new Move(5, 0, 4, 1); //Dummy move
        private double bestScore = double.MinValue;

        private static bool timeExpired = false;

        private static int maxTimeMs = 100;


        public Form1()
        {
            InitializeComponent();

            GameState = new int[8, 8]{
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 3, 0, 3, 0, 3, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 2, 0, 2, 0, 2, 0, 2, 0 },
                { 0, 2, 0, 2, 0, 2, 0, 2 },
                { 2, 0, 2, 0, 2, 0, 2, 0 }
            };

            //Adding click events for necesery fields
            for (int i = 1; i <= 64; i++)
            {
                Control? ctrl = this.Controls["pb" + i];
                if (ctrl is PictureBox pictureBox)
                {

                    boxes.Add(pictureBox);
                    if (GameState[(i - 1) / 8, (i - 1) % 8] != 0)
                    {

                        pictureBox.Click += PictureBox_Click;
                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox.BackColor = Color.Black;
                    }
                    else
                    {
                        pictureBox.BackColor = Color.White;
                    }
                }
            }

            rbWhite.Checked = true;


        }
        
        //after a game it clears all reamining pieces
        void ClearBoard()
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i].Image = null;
            }
        }

        //Sets all variables for starting game again
        public void Restart()
        {
            rbWhite.Enabled = true;
            rbRed.Enabled = true;
            GameState = new int[8, 8]{
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 3, 0, 3, 0, 3, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 2, 0, 2, 0, 2, 0, 2, 0 },
                { 0, 2, 0, 2, 0, 2, 0, 2 },
                { 2, 0, 2, 0, 2, 0, 2, 0 }
            };

            rbWhite.Checked = true;

            cbUseAI.Checked = false;
            possibleMoves.Clear();
            removeHighLight();
            lbPointsRed.Text = "0";
            lbWhitePoints.Text = "0";


            ClearBoard();

        }

        //Returns new 2D matrix, with same values as the one passed to it
        public static int[,] Copymatrix(int[,] a)
        {
            int[,] b = new int[8, 8]{
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 3, 0, 3, 0, 3, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 2, 0, 2, 0, 2, 0, 2, 0 },
                { 0, 2, 0, 2, 0, 2, 0, 2 },
                { 2, 0, 2, 0, 2, 0, 2, 0 }
            };

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    b[i, j] = a[i, j];
                }
            }

            return b;
        }

        //looks at each piece of player and checkes for possible moves on every diagonal 2 fields away
        //if there are capturing moves, only them stay as possible move

        public static List<Move> calculatePossibleMoves(int[,] trGameState, int row, int col, int SearchLevel)
        {


            List<Move> retVal = new List<Move>();
            List<Move> capturingMoves = new List<Move>();
            bool whiteTurn = false;

            if (IsWhite(trGameState, row, col) == 1) { whiteTurn = true; }






            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (Math.Abs(i) != Math.Abs(j)) continue;


                    if (isvalid(trGameState, row, col, row + i, col + j, whiteTurn))
                    {

                        retVal.Add(new Move(row, col, row + i, col + j));
                    }

                }
            }





            //Leaves only capturing if it can do it
            bool capTrue = false;
            foreach (Move m in retVal)
            {
                if (m.doingCapture)
                {
                    capTrue = true;
                    capturingMoves.Add(m);
                }
            }


            if (!capTrue)
            {
                if (SearchLevel >= 1)
                {
                    retVal.Clear();
                    return retVal;
                }
                else
                {
                    return retVal;
                }
            }


            retVal.Clear();

            if (capTrue)
            {
                foreach (Move m in capturingMoves)
                {
                    retVal.Add((Move)m);
                }
            }

            //If there is capture 




            foreach (Move m in retVal)
            {

                int[,] newState = Copymatrix(trGameState);


                newState[(m.toI + m.fromI) / 2, (m.toJ + m.fromJ) / 2] = 3; // mask the captured piece

                newState[(m.toI), (m.toJ)] = newState[m.fromI, m.fromJ];

                newState[m.fromI, m.fromJ] = 3;


                List<Move> capture2 = calculatePossibleMoves(newState, m.toI, m.toJ, SearchLevel + 1);


                m.moves = capture2;

            }





            return retVal;

        }



        //wrapper for calculating all moves,used for Ai calculations
        public static List<Move> CalculateMoves(int[,] trGameState, bool WhitePlayer)
        {
            List<Move> ans = new List<Move>();
            List<Move> newMoves = new List<Move>();
            int lookingForColor = 0;
            if (WhitePlayer) lookingForColor = 1;
            if (!WhitePlayer) lookingForColor = 2;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (IsWhite(trGameState, i, j) == lookingForColor)
                    {

                        List<Move> posMov = calculatePossibleMoves(trGameState, i, j, 0);



                        if (posMov.Count() == 0) continue;



                        foreach (Move m in posMov)
                        {
                            if (m.doingCapture)
                            {
                                newMoves.Add(m);
                            }
                            ans.Add(m);
                        }

                    }



                }
            }

            if (newMoves.Count() > 0)
            {
                return newMoves;
            }
            return ans;
        }

        //called after each round,finds possible moves for current player and if AI is enabled sends possble moves to him
        void nextRound(int round)
        {







            if (WhiteTurn && round != 0)
            {
                WhiteTurn = false;


            }
            else if (!WhiteTurn && round != 0)
            {

                WhiteTurn = true;

            }


            if (possibleMoves.Count() != 0) possibleMoves.Clear();
            lbPlayersTurn.Text = (WhiteTurn) ? "White's turn" : "Red's turn";

            possibleMoves = CalculateMoves(GameState, WhiteTurn);


            GameOver();

            if (cbUseAI.Checked)
            {
                if (rbWhite.Checked)
                {
                    if (!WhiteTurn)
                    {

                        possibleMoves = ReformMoves(possibleMoves);
                        AiTurn();
                    }
                }
                else if (rbRed.Checked)
                {
                    if (WhiteTurn)
                    {
                        possibleMoves = ReformMoves(possibleMoves);
                        AiTurn();
                    }
                }
            }





        }

        //finds all endings for AI reforming
        public static List<Pair> mapEnds(Move move)
        {
            List<Pair> ans = new List<Pair>();
            if (move.moves.Count == 0)
            {
                ans.Add(new Pair(move.toI, move.toJ));
                return ans;
            }
            foreach (Move m in move.moves)
            {

                if (m.moves.Count() == 0)
                {
                    ans.Add(new Pair(m.toI, m.toJ));
                }
                if (m.moves.Count() != 0)
                { //we have multijump
                    ans.AddRange(mapEnds(m));
                }

            }

            return ans;
        }

        //Beacuse of recursive calls it can happen that ending position is added twice
        public static List<Pair> RemoveDuplicates(List<Pair> ul)
        {
            List<Pair> iz = new List<Pair>();

            foreach (Pair m in ul)
            {
                bool isin = false;

                foreach (Pair p in iz)
                {
                    if (p.i == m.i && p.j == m.j) isin = true;
                }

                if (!isin)
                {

                    iz.Add(new Pair(m.i, m.j));

                }


            }

            return iz;
        }

        //because move can have more ending positions, for AI im transforming so that every move has only one ending position
        //that way its easier for Ai to evaluate each one of them
        public static List<Move> ReformMoves(List<Move> ulazni)
        {
            List<Move> newPosMoves = new List<Move>();



            foreach (Move move in ulazni)
            {


                if (!move.doingCapture)
                {
                    newPosMoves.Add(move);
                }
                else
                {


                    List<Pair> prpairs = mapEnds(move);

                    List<Pair> pairs = RemoveDuplicates(prpairs);



                    foreach (Pair p in pairs)
                    {

                        List<Move> mm = GetPath(move, p.i, p.j);

                        if (mm.Count > 0)
                        {
                            Move mov = TurnPathToMoves(mm);
                            Move trav = mov;


                            newPosMoves.Add(mov);
                        }
                    }



                }

            }



            return newPosMoves;

        }
        //helper for above function
        public static Move TurnPathToMoves(List<Move> path)
        {
            Move ans = new Move(path[0].fromI, path[0].fromJ, path[0].toI, path[0].toJ);

            for (int i = 1; i < path.Count(); i++)
            {
                AddAsLastChild(ans, path[i]);
            }

            return ans;

        }
        //helper for function above
        public static void AddAsLastChild(Move start, Move ch)
        {
            if (start.moves.Count == 0)
            {
                start.moves.Add(new Move(ch.fromI, ch.fromJ, ch.toI, ch.toJ));
            }
            else
            {
                AddAsLastChild(start.moves[0], ch);
            }
        }

        //changes backcolor of a field so it becomes visible
        void HighLightboxes()
        {

            foreach (Pair p in highLight)
            {
                int index = (p.i * 8) + p.j;

                boxes[index].BackColor = Color.LightGray;




            }

        }

        //returns fields to normal state after a move
        void removeHighLight()
        {
            foreach (Pair p in highLight)
            {
                int index = (p.i * 8) + p.j;

                boxes[index].BackColor = Color.Black;
            }

            highLight.Clear();
        }

        //finds all possible endings for selected piece 
        void addBoxesForHighLithing(int row, int col)
        {
            highLight.Add(new Pair(row, col));      //adding self
            foreach (Move m in possibleMoves)
            {
                if (m.fromI == row && m.fromJ == col)
                {
                    if (m.moves.Count() == 0)
                    {
                        highLight.Add(new Pair(m.toI, m.toJ));
                    }
                    if (m.moves.Count() != 0)
                    {
                        searchThroughforHighLight(m.moves);
                    }
                }
            }
        }

        //finds all possible endings for selected piece 
        void searchThroughforHighLight(List<Move> mm)
        {
            foreach (Move m in mm)
            {
                if (m.moves.Count() == 0)
                {
                    highLight.Add(new Pair(m.toI, m.toJ));
                }
                else
                {
                    searchThroughforHighLight(m.moves);
                }
            }

        }


        //Handles clicking on fields by human
        private void PictureBox_Click(object? sender, EventArgs e)
        {
            if (sender is PictureBox clickedBox)
            {
                int index = int.Parse(clickedBox.Name.Replace("pb", "")) - 1;
                int row = index / 8;
                int col = index % 8;




                if (highLight.Count() == 0)
                {



                    foreach (Move m in possibleMoves)
                    {
                        if (m.fromI == row && m.fromJ == col)
                        {
                            lbTakeOpponent.Text = "";
                            addBoxesForHighLithing(row, col);
                            break;
                        }
                    }
                    if (highLight.Count() == 0)
                    {
                        if (possibleMoves[0].doingCapture)
                        {
                            lbTakeOpponent.Text = "You have to take your opponent's piece!";
                        }
                        else
                        {
                            lbTakeOpponent.Text = "You can not move this piece!";
                        }
                    }
                    else
                    {
                        HighLightboxes();
                    }
                }
                else
                {

                    for (int i = 1; i < highLight.Count(); i++)
                    {
                        if (highLight[i].i == row && highLight[i].j == col)//This is valid move 
                        {
                            foreach (Move m in possibleMoves)
                            {
                                if (m.fromI == highLight[0].i && m.fromJ == highLight[0].j && MoveEndingWith(m, row, col))
                                {
                                    MovePiece(m, row, col);

                                    removeHighLight();
                                    nextRound(1);
                                    return;
                                }
                            }
                        }
                    }



                    removeHighLight();
                    return;

                }

            }
        }

        //There is possibility of multiple strating position to have the same ending position
        //This checks that particular move actually ends with the clicked field
        bool MoveEndingWith(Move m, int r, int c)
        {
            if (m.toI == r && m.toJ == c && m.moves.Count() == 0)
            {
                return true;
            }
            bool retVal = false;
            foreach (Move mm in m.moves)
            {
                if (MoveEndingWith(mm, r, c)) retVal = true;
            }
            return retVal;
        }

        //Gets a list of moves that lead from starting point to clicked ending point
        public static List<Move> GetPath(Move m, int r, int c)
        {
            List<Move> path = new List<Move>();

            if (m.toI == r && m.toJ == c)
            {
                path.Insert(0, new Move(m.fromI, m.fromJ, m.toI, m.toJ));
                return path;
            }



            foreach (Move mm in m.moves)
            {
                List<Move> odg = GetPath(mm, r, c);
                if (odg.Count() > 0)
                {

                    odg.Insert(0, new Move(m.fromI, m.fromJ, m.toI, m.toJ));
                    return odg;
                }

            }

            return path;
        }



        //unwraps the move tree for AI (different then for player)
        public static List<Move> GetPathAi(Move M)
        {
            List<Move> ans = new List<Move>();
            

            if (M.moves.Count() == 0)
            {
                ans.Add(M);
                return ans;
            }
            else
            {
                ans.AddRange(GetPathAi(M.moves[0]));
            }

            ans.Insert(0, M);

            return ans;
            
        }


        //Takes move object and unwraps it,if there are more then one move (multicaptur) executes them in right order
        void MovePiece(Move m, int zavRow, int zavCol)
        {




            if (m.moves.Count() == 0)
            {
                MovePiece(m.fromI, m.fromJ, m.toI, m.toJ);
            }
            else
            {
                List<Move> mov = GetPath(m, zavRow, zavCol);





                foreach (Move mm in mov)
                {

                    MovePiece(mm.fromI, mm.fromJ, mm.toI, mm.toJ);
                }

            }




        }


        //handels moving of pieces for both sides, changes GameState
        void MovePiece(int trRow, int trColumn, int toRow, int toColumn)
        {


            int ColDist = toColumn - trColumn;
            int rowDist = toRow - trRow;

            int indexFrom = trRow * 8 + trColumn;
            int indexTo = toRow * 8 + toColumn;

            boxes[indexTo].Image = boxes[indexFrom].Image;

            boxes[indexFrom].Image = null;

            GameState[toRow, toColumn] = GameState[trRow, trColumn];
            GameState[trRow, trColumn] = 3;

            if (toRow == 7 && GameState[toRow, toColumn] == 1)
            {
                GameState[toRow, toColumn] = 4;

                boxes[indexTo].Image = Resources.ie3;
            }

            if (toRow == 0 && GameState[toRow, toColumn] == 2)
            {
                GameState[toRow, toColumn] = 5;

                boxes[indexTo].Image = Resources.fx3;
            }



            if (rowDist == 2 || rowDist == -2)
            { //For jumps

                int middleRow = (toRow + trRow) / 2, middleCol = (toColumn + trColumn) / 2;


                boxes[middleRow * 8 + middleCol].Image = null;

                if (GameState[middleRow, middleCol] == 4 || GameState[middleRow, middleCol] == 5)
                {
                    updateScore(IsWhite(GameState, toRow, toColumn) == 1 ? true : false, 3);
                }
                else
                {
                    updateScore(IsWhite(GameState, toRow, toColumn) == 1 ? true : false, 1);
                }

                GameState[middleRow, middleCol] = 3;



            }




        }


        //checks if move is valid
        public static bool isvalid(int[,] trGameState, int trRow, int trColumn, int toRow, int toCol, bool WhitesMove)
        {



            bool isKing = false;


            if (trGameState[trRow, trColumn] == 4 || trGameState[trRow, trColumn] == 5)
            {
                isKing = true;
            }




            if (toRow < 0 || toRow > 7 || toCol < 0 || toCol > 7) return false;

            if (WhitesMove)
            {

                if (Math.Abs(trRow - toRow) > 2 || Math.Abs(trColumn - toCol) > 2) return false;
                if (Math.Abs(trRow - toRow) != Math.Abs(trColumn - toCol)) return false;




                if (toRow - trRow < 0 && !isKing) return false;

                if (trGameState[toRow, toCol] != 3) return false;





                if (Math.Abs(trRow - toRow) == 1 && Math.Abs(trColumn - toCol) == 1) return true;





                int middleRow = (trRow + toRow) / 2, middleColumn = (trColumn + toCol) / 2;

                if (IsWhite(trGameState, middleRow, middleColumn) == 1 || IsWhite(trGameState, middleRow, middleColumn) == 3) return false;





                return true;
            }
            else
            {

                if (Math.Abs(trRow - toRow) > 2 || Math.Abs(trColumn - toCol) > 2) return false;

                if (Math.Abs(trRow - toRow) != Math.Abs(trColumn - toCol)) return false;



                if (toRow - trRow > 0 && !isKing) return false;


                if (trGameState[toRow, toCol] != 3) return false;


                int middleRow = (trRow + toRow) / 2, middleColumn = (trColumn + toCol) / 2;

                if (Math.Abs(trRow - toRow) == 1 && Math.Abs(trColumn - toCol) == 1) return true;

                if (IsWhite(trGameState, middleRow, middleColumn) == 3) return false;




                if (IsWhite(trGameState, middleRow, middleColumn) == 2) return false;




                return true;


            }





        }


        //helper function to detrmine which side piece on (r(row),c(column)) belongs
        public static int IsWhite(int[,] trGameState, int r, int c)
        {
            if (trGameState[r, c] == 1 || trGameState[r, c] == 4) return 1;
            if (trGameState[r, c] == 2 || trGameState[r, c] == 5) return 2;
            return 3;
        }





        //Sets important variables on the strat and strats the game
        private void btnPlay_Click(object sender, EventArgs e)
        {
            DrawInitialPictures();

            WhiteTurn = true;

            nextRound(0);



            rbRed.Enabled = false;
            rbWhite.Enabled = false;

        }


        //draws pictures on board depending on GameState table
        private void DrawInitialPictures()
        {


            for (int i = 0; i < 8; i++)
            {

                for (int j = 0; j < 8; j++)
                {
                    Control? ctrl = this.Controls["pb" + (i * 8 + j + 1)];
                    if (ctrl is PictureBox pictureBox)
                    {
                        if (GameState[i, j] == 1)
                        {
                            pictureBox.Image = Resources.ie1;


                        }
                        else if (GameState[i, j] == 2)
                        {
                            pictureBox.Image = Resources.fx1;

                        }
                        else if (GameState[i, j] == 4)
                        {
                            pictureBox.Image = Resources.ie3;
                        }
                        else if (GameState[i, j] == 5)
                        {
                            pictureBox.Image = Resources.fx3;
                        }
                        else
                        {
                            pictureBox.Image = null;
                        }


                    }


                }

            }




        }


        //Checkes for avalible moves and calls GameOver Dialog if there are no more possible moves
        void GameOver()
        {
            if (CalculateMoves(GameState, true).Count() == 0)
            {
                Form go = new GameOver("Red");
                go.StartPosition = FormStartPosition.CenterParent;
                if (go.ShowDialog() == DialogResult.Yes)
                {

                    Restart();

                }
                else
                {
                    this.Close();
                }
            }

            if (CalculateMoves(GameState, false).Count() == 0)
            {
                Form go = new GameOver("White");
                go.StartPosition = FormStartPosition.CenterParent;
                if (go.ShowDialog() == DialogResult.Yes)
                {

                    Restart();

                }
                else
                {
                    this.Close();
                }
            }
        }

        //After capturing updates score 
        void updateScore(bool ForWhite, int points)
        {
            if (ForWhite)
            {
                lbWhitePoints.Text = $"{points + int.Parse(lbWhitePoints.Text)}";
            }
            else
            {
                lbPointsRed.Text = $"{points + int.Parse(lbPointsRed.Text)}";
            }
        }

        //Used a helper functions in MiniMax calculations
        private double Max(double a, double b)
        {
            if (a >= b) return a;
            else return b;
        }

        private double Min(double a, double b)
        {
            if (a <= b) return a;
            else return b;
        }


        //Calls minimax using multithreading and after the set time it executes the best move AI found

        private void AiTurn()
        {
            timeExpired = false;
            maxTimeMs = int.Parse(numUpDownMS.Value.ToString());

            System.Threading.Timer timer = new System.Threading.Timer(_ => timeExpired = true, null, maxTimeMs, Timeout.Infinite);

            bestScore = double.MinValue;
            foreach (var move in possibleMoves)
            {
                Move moveCopy = move;
                int[,] newBoard = ApplyMove(Copymatrix(GameState), move);

                tasks.Add(Task.Run(() =>
                {
                    int depth = maxTimeMs / (possibleMoves.Count * 10);





                    double score = Minimax(newBoard, depth, true, double.MinValue, double.MaxValue);




                    if (!timeExpired && score > bestScore)
                    {

                        bestScore = score;
                        bestMove = moveCopy;
                    }

                }));
            }


            Task.WaitAll(tasks.ToArray());

            timer.Dispose();

            List<Move> path = GetPathAi(bestMove);

            AddHighLigh(bestMove);
            HighLightboxes();

            System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
            timer2.Interval = 550;          ///By changing this number you are changing time that Ai moves the piece
            timer2.Tick += (s, e) =>
            {
                timer2.Stop();

                foreach (Move mm in path)
                {
                    MovePiece(mm.fromI, mm.fromJ, mm.toI, mm.toJ);
                }


                removeHighLight();

                nextRound(122);


            };
            timer2.Start();


        }


        //Used for Highlighting move of AI
        void AddHighLigh(Move m)
        {
            while (true)
            {

                highLight.Add(new Pair(m.toI, m.toJ));


                if (m.moves.Count > 0)
                {
                    m = m.moves[0];
                }
                else
                {
                    break;
                }
            }
        }



        //implements Minimax algorithm with alpha-beta pruning
        private double Minimax(int[,] board, int depth, bool isMaximizing, double alpha, double beta)
        {




            if (depth == 0 || GameOver(board) || timeExpired)
                return EvaluateBoard(board);

            if (isMaximizing)
            {
                double maxEval = double.MinValue;
                foreach (var move in ReformMoves(GetAllPossibleMoves(board, 2,rbWhite.Checked)))
                {
                    int[,] newBoard = ApplyMove(board, move);
                    double eval = Minimax(newBoard, depth - 1, false, alpha, beta);
                    maxEval = Max(maxEval, eval);
                    alpha = Max(alpha, eval);
                    if (beta <= alpha) break;
                }
                return maxEval;
            }
            else
            {

                double minEval = double.MaxValue;
                foreach (var move in ReformMoves(GetAllPossibleMoves(board, 1, rbWhite.Checked)))
                {

                    int[,] newBoard = ApplyMove(board, move);
                    double eval = Minimax(newBoard, depth - 1, true, alpha, beta);
                    minEval = Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha) break;
                }
                return minEval;
            }
        }


        //Next six functions are used for evaluating AI move
        private bool GameOver(int[,] board)
        {

            if (ReformMoves(GetAllPossibleMoves(board, 2, rbWhite.Checked)).Count == 0 || ReformMoves(GetAllPossibleMoves(board, 1, rbWhite.Checked)).Count == 0)
            {
                return true;
            }

            return false;

        }
        private double EvaluateBoard(int[,] board)
        {
            double score = 0;
            int playerPieces = 0, opponentPieces = 0;
            int playerKings = 0, opponentKings = 0;
            int playerMobility = 0, opponentMobility = 0;
            int playerCenterControl = 0, opponentCenterControl = 0;
            int playerKingSafety = 0, opponentKingSafety = 0;
            int playerTrappedPieces = 0, opponentTrappedPieces = 0;
            int playerForcedCaptures = 0, opponentForcedCaptures = 0;




            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    int piece = board[row, col];


                    bool isCenter = (col >= 2 && col <= 5) && (row >= 2 && row <= 5);
                    if (rbWhite.Checked)
                    {
                        switch (piece)
                        {
                            case 1:
                                playerPieces++;
                                score += 1 + (row * 0.1);
                                if (row >= 6) score += 0.5;
                                if (isCenter) playerCenterControl += 1;
                                if (IsTrapped(board, row, col, 1)) playerTrappedPieces++;
                                break;

                            case 4:
                                playerKings++;
                                score += 2.2;
                                if (col == 0 || col == 7) playerKingSafety += 1;
                                if (isCenter) playerCenterControl += 1;
                                break;

                            case 2:
                                opponentPieces++;
                                score -= 1.2 + ((7 - row) * 0.1);
                                if (row <= 1) score -= 0.5;
                                if (isCenter) opponentCenterControl += 1;
                                if (IsTrapped(board, row, col, -1)) opponentTrappedPieces++;
                                break;

                            case 5:
                                opponentKings++;
                                score -= 2.2;
                                if (col == 0 || col == 7) opponentKingSafety += 1;
                                if (isCenter) opponentCenterControl += 1;
                                break;
                        }
                    }
                    else
                    {
                        switch (piece)
                        {
                            case 2:
                                playerPieces++;
                                score += 1.2 + ((7 - row) * 0.1);
                                if (row <= 1) score += 0.5;
                                if (isCenter) playerCenterControl += 1;
                                if (IsTrapped(board, row, col, 1)) playerTrappedPieces++;
                                break;

                            case 5:
                                playerKings++;
                                score += 2.2;
                                if (col == 0 || col == 7) playerKingSafety += 1;
                                if (isCenter) playerCenterControl += 1;
                                break;

                            case 1:
                                opponentPieces++;
                                score -= 1.2 + (row * 0.1);
                                if (row >= 6) score -= 0.5;
                                if (isCenter) opponentCenterControl += 1;
                                if (IsTrapped(board, row, col, -1)) opponentTrappedPieces++;
                                break;

                            case 4:
                                opponentKings++;
                                score -= 2.2;
                                if (col == 0 || col == 7) opponentKingSafety += 1;
                                if (isCenter) opponentCenterControl += 1;
                                break;
                        }
                    }
                }
            }

            if (rbWhite.Checked)
            {

                playerMobility = ReformMoves(GetAllPossibleMoves(board, 1, rbWhite.Checked)).Count;
                opponentMobility = ReformMoves(GetAllPossibleMoves(board, 2, rbWhite.Checked)).Count;
                score += (playerMobility - opponentMobility) * 0.5;


                playerForcedCaptures = GetForcedCaptures(board, 1, rbWhite.Checked);
                opponentForcedCaptures = GetForcedCaptures(board, 2, rbWhite.Checked);
                score += (playerForcedCaptures - opponentForcedCaptures) * 1.5;

            }
            else
            {

                playerMobility = ReformMoves(GetAllPossibleMoves(board, 2, rbWhite.Checked)).Count;
                opponentMobility = ReformMoves(GetAllPossibleMoves(board, 1, rbWhite.Checked)).Count;
                score += (playerMobility - opponentMobility) * 0.5;


                playerForcedCaptures = GetForcedCaptures(board, 2, rbWhite.Checked);
                opponentForcedCaptures = GetForcedCaptures(board, 1, rbWhite.Checked);
                score += (playerForcedCaptures - opponentForcedCaptures) * 1.5;

            }

            score -= playerTrappedPieces * 1.0;
            score += opponentTrappedPieces * 1.0;


            score += playerKingSafety * 0.3;
            score -= opponentKingSafety * 0.3;


            score += (playerCenterControl - opponentCenterControl) * 2.0;

            return score;



        }
        private int[,] ApplyMove(int[,] board, Move move)
        {

            List<Move> path = GetPathAi(move);
            foreach (Move m in path)
            {
                int toRow = m.toI;
                int toColumn = m.toJ;
                int trRow = m.fromI;
                int trColumn = m.fromJ;

                int ColDist = toColumn - trColumn;
                int rowDist = toRow - trRow;

                board[toRow, toColumn] = board[trRow, trColumn];
                board[trRow, trColumn] = 3;

                if (toRow == 7 && GameState[toRow, toColumn] == 1)
                {
                    board[toRow, toColumn] = 4;


                }

                if (toRow == 0 && board[toRow, toColumn] == 2)
                {
                    board[toRow, toColumn] = 5;


                }
                if (rowDist == 2 || rowDist == -2)
                { //For jumps

                    int middleRow = (toRow + trRow) / 2, middleCol = (toColumn + trColumn) / 2;




                    board[middleRow, middleCol] = 3;



                }

            }

            return board;


        }
        public static List<Move> GetAllPossibleMoves(int[,] board, int Player, bool WhiteChecked)
        {
            bool Whites = false;

            if ((Player == 1 && WhiteChecked) || (Player == 2 && !WhiteChecked))
            {
                Whites = true;
            }
            List<Move> moves = CalculateMoves(board, Whites);

            return moves;


        }



        public static bool IsTrapped(int[,] board, int row, int col, int player)
        {
            // If the piece has no valid moves, it's trapped
            List<Move> moves = calculatePossibleMoves(board, row, col, 0);
            return moves.Count == 0;
        }

        public static int GetForcedCaptures(int[,] board, int player, bool whiteChecked)
        {
            int count = 0;



            foreach (Move move in ReformMoves(GetAllPossibleMoves(board, player, whiteChecked)))
            {
                if (move.doingCapture)
                {

                    count += GetPathAi(move).Count();        //Bigger capture more points
                }
            }
            return count;
        }



        //Opens Info page
        private void btnInfo_Click(object sender, EventArgs e)
        {
            Form inf = new Info();

            inf.Show();
        }

     
    }
}