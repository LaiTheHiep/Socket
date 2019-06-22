namespace Multichat
{
    partial class Sever
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
            this.button1 = new System.Windows.Forms.Button();
            this.txbMessage = new System.Windows.Forms.TextBox();
            this.lsvView = new System.Windows.Forms.ListView();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(376, 267);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 32);
            this.button1.TabIndex = 5;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txbMessage
            // 
            this.txbMessage.Location = new System.Drawing.Point(12, 267);
            this.txbMessage.Multiline = true;
            this.txbMessage.Name = "txbMessage";
            this.txbMessage.Size = new System.Drawing.Size(341, 32);
            this.txbMessage.TabIndex = 4;
            // 
            // lsvView
            // 
            this.lsvView.Location = new System.Drawing.Point(12, 12);
            this.lsvView.Name = "lsvView";
            this.lsvView.Size = new System.Drawing.Size(439, 240);
            this.lsvView.TabIndex = 3;
            this.lsvView.UseCompatibleStateImageBehavior = false;
            this.lsvView.View = System.Windows.Forms.View.List;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(483, 267);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 32);
            this.button2.TabIndex = 6;
            this.button2.Text = "View IP";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Sever
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 308);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txbMessage);
            this.Controls.Add(this.lsvView);
            this.Name = "Sever";
            this.Text = "Sever";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Sever_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txbMessage;
        private System.Windows.Forms.ListView lsvView;
        private System.Windows.Forms.Button button2;
    }
}