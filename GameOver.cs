using DraughtsGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DraughtsGame
{
    public partial class GameOver : Form
    {

        public GameOver(string Winner)
        {
            InitializeComponent();

            if (Winner == "White")
            {
                lbwinner.Text = "The white player has won the game";
                pbWinnerPhoto.Image = Resources.ie3;
            }
            else
            {
                lbwinner.Text = "The red player has won the game";
                pbWinnerPhoto.Image = Resources.fx3;
            }
            lbRestart.Text = "Do you want to restart the game?";



        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            
            this.DialogResult = DialogResult.Yes;

            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {

            this.DialogResult= DialogResult.No;

            this.Close();
        }
    }
}
