using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DraughtsGame
{

    public  class Move
    {
       

        public int fromI, fromJ, toI, toJ;
       
        
        

        public List<Move> moves = new List<Move>();

        public bool doingCapture = false;

        public Move(int fromI, int fromJ, int toI, int toJ)
        {
            this.fromI = fromI;
            this.fromJ = fromJ;
            this.toI = toI;
            this.toJ = toJ;

            if (Math.Abs(fromI - toI) == 2 && Math.Abs(fromJ - toJ) == 2)
            {
                doingCapture = true;
            }


        }

        public string toString()
        {
            return $"Move from : ({fromI}, {fromJ}) to ({toI}, {toJ}), moves.Count: {moves.Count()}.";
        }

       
    }


    public class Pair
    {
        public int i, j;
        public Pair(int i, int j)
        {
            this.i = i;
            this.j = j;
        }

      


    }
}

