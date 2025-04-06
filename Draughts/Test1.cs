namespace Draughts
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void IsTrapped_ShouldReturnTrue_WhenPieceIsSurrounded()
        {

            int[,] v = new int[8, 8]{
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 }
            };

            v[4, 3] = 1;
            v[5, 2] = 2;
            v[6, 1] = 2;
            v[5, 4] = 2;
            v[6, 5] = 2;

            //piece represent a man and because he can not move down it should give false
            bool result_true = DraughtsGame.Form1.IsTrapped(v, 4, 3, 1);

            v[6, 5] = 3;
            //I removed one opponent piece now it's able to capture opponent's piece, so it's not trapped
            bool result_false = DraughtsGame.Form1.IsTrapped(v, 4, 3, 1);

            Assert.IsTrue(result_true);
            Assert.IsFalse(result_false);
        }
        [TestMethod]
        public void IsValid_ShouldReturnTrue_WhenPieceCanDoTheMove()
        {

            int[,] v = new int[8, 8]{
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 4 },
                { 3, 0, 3, 0, 3, 0, 3, 0 },
                { 0, 3, 0, 1, 0, 3, 0, 3 },
                { 3, 0, 2, 0, 2, 0, 3, 0 },
                { 0, 2, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 }
            };
            //allowed capture => true
            bool result_true1 = DraughtsGame.Form1.isvalid(v,4,3,6,5,true);
            //Can not capture beacuse of opponents piece => false
            bool result_false1 = DraughtsGame.Form1.isvalid(v,4,3,6,1,true);
            //Moving outside the board => false
            bool result_false2 = DraughtsGame.Form1.isvalid(v,2,7,1,8,true);
            //king can move in every direction, this is a simple move => true
            bool result_true2 = DraughtsGame.Form1.isvalid(v,2,7,1,6,true);

           

            

            Assert.IsTrue(result_true1);
            Assert.IsTrue(result_true2);

            Assert.IsFalse(result_false1);
            Assert.IsFalse(result_false1);
            
        }

        [TestMethod]
        public void CalculateMoves_ShouldReturnTrue_WhenNumberOfMovesIsEqual()
        {
            int[,] v = new int[8, 8]{
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 3, 0, 3, 0, 3, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 2, 0, 2, 0, 2, 0, 2, 0 },
                { 0, 2, 0, 2, 0, 2, 0, 2 },
                { 2, 0, 2, 0, 2, 0, 2, 0 }
            };

            //This is starting state so both should have equal number of possible moves
            var movesWhite = DraughtsGame.Form1.CalculateMoves(v, true);
            var movesRed = DraughtsGame.Form1.CalculateMoves(v, false);

            Assert.AreEqual(movesRed.Count, movesWhite.Count);

        }
        [TestMethod]
        public void ReformMoves_ShouldReturnTrue_WhenNumberOfMovesIsEqual()
        {
            int[,] v = new int[8, 8]{
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 1, 0, 1, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 1, 0, 3, 0 },
                { 0, 3, 0, 2, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 }
            };
            
            //one pice has multiple ending positions, should disasamble the ito two moves with same start and differnt ending

            var movesRed = DraughtsGame.Form1.ReformMoves( DraughtsGame.Form1.CalculateMoves(v, false));

            Assert.AreEqual(movesRed.Count, 2);
           

        }

        [TestMethod]
        public void TurnPathToMoves_ShouldReturnTrue_IfMoveWasConstructedWell()
        {

            List<DraughtsGame.Move> path = new List<DraughtsGame.Move>();

            path.Add(new DraughtsGame.Move(0,1,2,3));
            path.Add(new DraughtsGame.Move(2,3,4,5));
            path.Add(new DraughtsGame.Move(4,5,6,7));
            
            //should create single move from multiple moves provided
            DraughtsGame.Move m = DraughtsGame.Form1.TurnPathToMoves(path);
            bool res = (m.fromI == 0 && m.fromJ == 1 && m.toI == 2 && m.toJ == 3) && (m.moves[0].fromI == 2 && m.moves[0].fromJ == 3 && m.moves[0].toI == 4 && m.moves[0].toJ == 5) &&
                        (m.moves[0].moves[0].fromI == 4 && m.moves[0].moves[0].fromJ == 5 && m.moves[0].moves[0].toI == 6 && m.moves[0].moves[0].toJ == 7);

            Assert.IsTrue(res);
        }


        [TestMethod]
        public void GetForcedCaptures_ShouldReturnTrue_WhenNumberOfCapturesIsEqual()
        {

            int[,] v = new int[8, 8]{
                { 0, 3, 0, 3, 0, 4, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 2, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 2, 0 },
                { 0, 3, 0, 1, 0, 3, 0, 3 },
                { 3, 0, 2, 0, 2, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 }
            };
            //one piece has 2 captures and the other can do 4 so total is 6
            int ret = DraughtsGame.Form1.GetForcedCaptures(v, 1, true);

            Assert.AreEqual(6, ret);
           
        }

        [TestMethod]
        public void GetPath_ShouldReturnTrue_WhenPathIsCorrect()
        {
            //Tests if it can find correct path through path tree to get to the ending position
            int[,] v = new int[8, 8]{
                { 0, 3, 0, 3, 0, 4, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 2, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 2, 0, 3, 0, 2, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 2, 0, 2, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 }
            };

            var mov = DraughtsGame.Form1.CalculateMoves(v, true);

            DraughtsGame.Move m = mov[0];
            var path1 = DraughtsGame.Form1.GetPath(m, 6, 3);
            var path2 = DraughtsGame.Form1.GetPath(m, 6, 7);
           
            bool ret = (path1.Count == 3 && path2.Count == 3) && path1[1].fromI == 2 && path1[1].fromJ == 7 && path2[1].toI == 4 && path2[1].toJ == 5 && path1[2].toJ == 3 && path2[2].toJ == 7;

          

            Assert.IsTrue(ret);
          

        }

        [TestMethod]
        public void GetPathAI_ShouldReturnTrue_WhenPathIsCorrect()
        {

            int[,] v = new int[8, 8]{
                { 0, 3, 0, 3, 0, 4, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 2, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 2, 0, 3, 0, 2, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 2, 0, 2, 0 },
                { 0, 3, 0, 3, 0, 3, 0, 3 },
                { 3, 0, 3, 0, 3, 0, 3, 0 }
            };

            var mov = DraughtsGame.Form1.ReformMoves(DraughtsGame.Form1.CalculateMoves(v, true));

            DraughtsGame.Move m1 = mov[0];
            DraughtsGame.Move m2 = mov[1];
            var path1 = DraughtsGame.Form1.GetPathAi(m1);
            var path2 = DraughtsGame.Form1.GetPathAi(m2);

            bool ret = (path1.Count == 3 && path2.Count == 3) && path1[1].fromI == 2 && path1[1].fromJ == 7 && path2[1].toI == 4 && path2[1].toJ == 5 && ((path1[2].toJ == 3 && path2[2].toJ == 7) || (path1[2].toJ == 7 && path2[2].toJ == 3));



            Assert.IsTrue(ret);


        }

    }
}
