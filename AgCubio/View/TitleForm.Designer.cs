namespace View
{
    partial class TitleForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.HostNameTextBox = new System.Windows.Forms.TextBox();
            this.PlayerNameTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label2.Location = new System.Drawing.Point(321, 245);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Server";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label1.Location = new System.Drawing.Point(238, 165);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "Enter Username";
            // 
            // HostNameTextBox
            // 
            this.HostNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.HostNameTextBox.Location = new System.Drawing.Point(411, 242);
            this.HostNameTextBox.MaximumSize = new System.Drawing.Size(200, 50);
            this.HostNameTextBox.MinimumSize = new System.Drawing.Size(100, 40);
            this.HostNameTextBox.Name = "HostNameTextBox";
            this.HostNameTextBox.Size = new System.Drawing.Size(200, 30);
            this.HostNameTextBox.TabIndex = 5;
            this.HostNameTextBox.Text = "localhost";
            this.HostNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkKeyPress);
            // 
            // PlayerNameTextBox
            // 
            this.PlayerNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.PlayerNameTextBox.Location = new System.Drawing.Point(411, 162);
            this.PlayerNameTextBox.MaximumSize = new System.Drawing.Size(200, 100);
            this.PlayerNameTextBox.MinimumSize = new System.Drawing.Size(100, 40);
            this.PlayerNameTextBox.Name = "PlayerNameTextBox";
            this.PlayerNameTextBox.Size = new System.Drawing.Size(200, 30);
            this.PlayerNameTextBox.TabIndex = 4;
            this.PlayerNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkKeyPress);
            // 
            // TitleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 470);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.HostNameTextBox);
            this.Controls.Add(this.PlayerNameTextBox);
            this.Name = "TitleForm";
            this.Text = "OpeningForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox HostNameTextBox;
        private System.Windows.Forms.TextBox PlayerNameTextBox;
    }
}