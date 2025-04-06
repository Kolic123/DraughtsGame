namespace DraughtsGame
{
    partial class GameOver
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pbWinnerPhoto = new PictureBox();
            btnYes = new Button();
            btnNo = new Button();
            lbwinner = new Label();
            lbRestart = new Label();
            ((System.ComponentModel.ISupportInitialize)pbWinnerPhoto).BeginInit();
            SuspendLayout();
            // 
            // pbWinnerPhoto
            // 
            pbWinnerPhoto.Location = new Point(124, 12);
            pbWinnerPhoto.Name = "pbWinnerPhoto";
            pbWinnerPhoto.Size = new Size(96, 89);
            pbWinnerPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
            pbWinnerPhoto.TabIndex = 0;
            pbWinnerPhoto.TabStop = false;
            // 
            // btnYes
            // 
            btnYes.Location = new Point(12, 203);
            btnYes.Name = "btnYes";
            btnYes.Size = new Size(142, 23);
            btnYes.TabIndex = 1;
            btnYes.Text = "Yes, Restart the game";
            btnYes.UseVisualStyleBackColor = true;
            btnYes.Click += btnYes_Click;
            // 
            // btnNo
            // 
            btnNo.Location = new Point(227, 203);
            btnNo.Name = "btnNo";
            btnNo.Size = new Size(75, 23);
            btnNo.TabIndex = 2;
            btnNo.Text = "No, Exit";
            btnNo.UseVisualStyleBackColor = true;
            btnNo.Click += btnNo_Click;
            // 
            // lbwinner
            // 
            lbwinner.AutoSize = true;
            lbwinner.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbwinner.Location = new Point(12, 117);
            lbwinner.Name = "lbwinner";
            lbwinner.Size = new Size(0, 25);
            lbwinner.TabIndex = 3;
            // 
            // lbRestart
            // 
            lbRestart.AutoSize = true;
            lbRestart.Location = new Point(12, 169);
            lbRestart.Name = "lbRestart";
            lbRestart.Size = new Size(182, 15);
            lbRestart.TabIndex = 4;
            lbRestart.Text = "Do you want to restart the game?";
            // 
            // GameOver
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(349, 250);
            Controls.Add(lbRestart);
            Controls.Add(lbwinner);
            Controls.Add(btnNo);
            Controls.Add(btnYes);
            Controls.Add(pbWinnerPhoto);
            Name = "GameOver";
            Text = "GameOver";
            ((System.ComponentModel.ISupportInitialize)pbWinnerPhoto).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pbWinnerPhoto;
        private Button btnYes;
        private Button btnNo;
        private Label lbwinner;
        private Label lbRestart;
    }
}