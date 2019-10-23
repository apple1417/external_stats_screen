namespace external_stats_screen {
  partial class MainForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.LevelLabel = new System.Windows.Forms.Label();
      this.LevelStats = new System.Windows.Forms.Label();
      this.TotalLabel = new System.Windows.Forms.Label();
      this.GameStats = new System.Windows.Forms.Label();
      this.LevelName = new System.Windows.Forms.Label();
      this.TotalTitle = new System.Windows.Forms.Label();
      this.PlayerBox = new System.Windows.Forms.ComboBox();
      this.Title = new System.Windows.Forms.Label();
      this.GeneralLabel = new System.Windows.Forms.Label();
      this.GeneralStats = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // LevelLabel
      // 
      this.LevelLabel.AutoSize = true;
      this.LevelLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.LevelLabel.Location = new System.Drawing.Point(6, 129);
      this.LevelLabel.Name = "LevelLabel";
      this.LevelLabel.Size = new System.Drawing.Size(47, 75);
      this.LevelLabel.TabIndex = 0;
      this.LevelLabel.Text = "Score:\r\nDeaths:\r\nKills:\r\nSecrets:\r\nTime:";
      // 
      // LevelStats
      // 
      this.LevelStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.LevelStats.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.LevelStats.Location = new System.Drawing.Point(59, 129);
      this.LevelStats.Name = "LevelStats";
      this.LevelStats.Size = new System.Drawing.Size(70, 75);
      this.LevelStats.TabIndex = 0;
      this.LevelStats.Text = "0\r\n0\r\n0/0\r\n0/0\r\n00:00:00";
      this.LevelStats.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // TotalLabel
      // 
      this.TotalLabel.AutoSize = true;
      this.TotalLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TotalLabel.Location = new System.Drawing.Point(135, 129);
      this.TotalLabel.Name = "TotalLabel";
      this.TotalLabel.Size = new System.Drawing.Size(47, 75);
      this.TotalLabel.TabIndex = 0;
      this.TotalLabel.Text = "Score:\r\nDeaths:\r\nKills:\r\nSecrets:\r\nTime:";
      // 
      // GameStats
      // 
      this.GameStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.GameStats.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.GameStats.Location = new System.Drawing.Point(188, 129);
      this.GameStats.Name = "GameStats";
      this.GameStats.Size = new System.Drawing.Size(61, 75);
      this.GameStats.TabIndex = 0;
      this.GameStats.Text = "0\r\n0\r\n0/0\r\n0/0\r\n00:00:00";
      this.GameStats.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // LevelName
      // 
      this.LevelName.AutoSize = true;
      this.LevelName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.LevelName.Location = new System.Drawing.Point(6, 114);
      this.LevelName.Name = "LevelName";
      this.LevelName.Size = new System.Drawing.Size(94, 15);
      this.LevelName.TabIndex = 1;
      this.LevelName.Text = "Unknown Level";
      // 
      // TotalTitle
      // 
      this.TotalTitle.AutoSize = true;
      this.TotalTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TotalTitle.Location = new System.Drawing.Point(135, 114);
      this.TotalTitle.Name = "TotalTitle";
      this.TotalTitle.Size = new System.Drawing.Size(34, 15);
      this.TotalTitle.TabIndex = 1;
      this.TotalTitle.Text = "Total";
      // 
      // PlayerBox
      // 
      this.PlayerBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.PlayerBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PlayerBox.FormattingEnabled = true;
      this.PlayerBox.Items.AddRange(new object[] {
            "Squad"});
      this.PlayerBox.Location = new System.Drawing.Point(6, 88);
      this.PlayerBox.Name = "PlayerBox";
      this.PlayerBox.Size = new System.Drawing.Size(243, 23);
      this.PlayerBox.TabIndex = 2;
      // 
      // Title
      // 
      this.Title.AutoSize = true;
      this.Title.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
      this.Title.Location = new System.Drawing.Point(6, 6);
      this.Title.Name = "Title";
      this.Title.Size = new System.Drawing.Size(245, 19);
      this.Title.TabIndex = 1;
      this.Title.Text = "External Stats Screen - Not Hooked";
      // 
      // GeneralLabel
      // 
      this.GeneralLabel.AutoSize = true;
      this.GeneralLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.GeneralLabel.Location = new System.Drawing.Point(6, 25);
      this.GeneralLabel.Name = "GeneralLabel";
      this.GeneralLabel.Size = new System.Drawing.Size(78, 60);
      this.GeneralLabel.TabIndex = 0;
      this.GeneralLabel.Text = "Total Score:\r\nDifficulty:\r\nStarted:\r\nPlaying Time:";
      // 
      // GeneralStats
      // 
      this.GeneralStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.GeneralStats.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.GeneralStats.Location = new System.Drawing.Point(90, 25);
      this.GeneralStats.Name = "GeneralStats";
      this.GeneralStats.Size = new System.Drawing.Size(159, 60);
      this.GeneralStats.TabIndex = 0;
      this.GeneralStats.Text = "0\r\nNormal\r\nNot Started\r\n00:00:00";
      this.GeneralStats.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // MainForm
      // 
      this.ClientSize = new System.Drawing.Size(258, 212);
      this.Controls.Add(this.PlayerBox);
      this.Controls.Add(this.LevelName);
      this.Controls.Add(this.LevelLabel);
      this.Controls.Add(this.LevelStats);
      this.Controls.Add(this.TotalLabel);
      this.Controls.Add(this.GameStats);
      this.Controls.Add(this.TotalTitle);
      this.Controls.Add(this.Title);
      this.Controls.Add(this.GeneralLabel);
      this.Controls.Add(this.GeneralStats);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "MainForm";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label LevelLabel;
    private System.Windows.Forms.Label LevelStats;
    private System.Windows.Forms.Label TotalLabel;
    private System.Windows.Forms.Label GameStats;
    private System.Windows.Forms.Label LevelName;
    private System.Windows.Forms.Label TotalTitle;
    private System.Windows.Forms.ComboBox PlayerBox;
    private System.Windows.Forms.Label Title;
    private System.Windows.Forms.Label GeneralLabel;
    private System.Windows.Forms.Label GeneralStats;
  }
}