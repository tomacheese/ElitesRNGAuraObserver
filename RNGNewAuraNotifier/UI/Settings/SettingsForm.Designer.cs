namespace RNGNewAuraNotifier.UI.Settings;
partial class SettingsForm
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
        label1 = new Label();
        textBoxWatchingFilePath = new TextBox();
        label2 = new Label();
        textBoxLogDir = new TextBox();
        label3 = new Label();
        textBoxDiscordWebhookUrl = new TextBox();
        buttonSave = new Button();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(21, 83);
        label1.Margin = new Padding(2, 0, 2, 0);
        label1.Name = "label1";
        label1.Size = new Size(102, 15);
        label1.TabIndex = 0;
        label1.Text = "Watching LogFile:";
        // 
        // textBoxWatchingFilePath
        // 
        textBoxWatchingFilePath.Location = new Point(23, 100);
        textBoxWatchingFilePath.Margin = new Padding(2, 2, 2, 2);
        textBoxWatchingFilePath.Multiline = true;
        textBoxWatchingFilePath.Name = "textBoxWatchingFilePath";
        textBoxWatchingFilePath.ReadOnly = true;
        textBoxWatchingFilePath.Size = new Size(509, 42);
        textBoxWatchingFilePath.TabIndex = 1;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(21, 8);
        label2.Margin = new Padding(2, 0, 2, 0);
        label2.Name = "label2";
        label2.Size = new Size(78, 15);
        label2.TabIndex = 2;
        label2.Text = "LogDirectory:";
        // 
        // textBoxLogDir
        // 
        textBoxLogDir.Location = new Point(23, 25);
        textBoxLogDir.Margin = new Padding(2, 2, 2, 2);
        textBoxLogDir.Multiline = true;
        textBoxLogDir.Name = "textBoxLogDir";
        textBoxLogDir.Size = new Size(509, 42);
        textBoxLogDir.TabIndex = 3;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(21, 158);
        label3.Margin = new Padding(2, 0, 2, 0);
        label3.Name = "label3";
        label3.Size = new Size(128, 15);
        label3.TabIndex = 4;
        label3.Text = "Discord Webhook URL:";
        // 
        // textBoxDiscordWebhookUrl
        // 
        textBoxDiscordWebhookUrl.Location = new Point(23, 175);
        textBoxDiscordWebhookUrl.Margin = new Padding(2, 2, 2, 2);
        textBoxDiscordWebhookUrl.Multiline = true;
        textBoxDiscordWebhookUrl.Name = "textBoxDiscordWebhookUrl";
        textBoxDiscordWebhookUrl.Size = new Size(509, 42);
        textBoxDiscordWebhookUrl.TabIndex = 5;
        // 
        // buttonSave
        // 
        buttonSave.Location = new Point(457, 238);
        buttonSave.Margin = new Padding(2, 2, 2, 2);
        buttonSave.Name = "buttonSave";
        buttonSave.Size = new Size(72, 32);
        buttonSave.TabIndex = 6;
        buttonSave.Text = "Save";
        buttonSave.UseVisualStyleBackColor = true;
        buttonSave.Click += OnSaveButtonClicked;
        // 
        // SettingsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(560, 292);
        Controls.Add(buttonSave);
        Controls.Add(textBoxDiscordWebhookUrl);
        Controls.Add(label3);
        Controls.Add(textBoxLogDir);
        Controls.Add(label2);
        Controls.Add(textBoxWatchingFilePath);
        Controls.Add(label1);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(2, 2, 2, 2);
        MaximizeBox = false;
        Name = "SettingsForm";
        Text = "RNGNewAuraNotifier Settings";
        FormClosing += OnFormClosing;
        FormClosed += OnFormClosed;
        Load += OnLoad;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textBoxWatchingFilePath;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBoxLogDir;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textBoxDiscordWebhookUrl;
    private System.Windows.Forms.Button buttonSave;
}
