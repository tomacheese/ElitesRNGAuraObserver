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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
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
        label1.Location = new Point(30, 139);
        label1.Name = "label1";
        label1.Size = new Size(151, 25);
        label1.TabIndex = 0;
        label1.Text = "Watching LogFile:";
        // 
        // textBoxWatchingFilePath
        // 
        textBoxWatchingFilePath.Location = new Point(33, 167);
        textBoxWatchingFilePath.Margin = new Padding(3, 4, 3, 4);
        textBoxWatchingFilePath.Multiline = true;
        textBoxWatchingFilePath.Name = "textBoxWatchingFilePath";
        textBoxWatchingFilePath.ReadOnly = true;
        textBoxWatchingFilePath.Size = new Size(726, 68);
        textBoxWatchingFilePath.TabIndex = 1;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(30, 14);
        label2.Name = "label2";
        label2.Size = new Size(118, 25);
        label2.TabIndex = 2;
        label2.Text = "LogDirectory:";
        // 
        // textBoxLogDir
        // 
        textBoxLogDir.Location = new Point(33, 42);
        textBoxLogDir.Margin = new Padding(3, 4, 3, 4);
        textBoxLogDir.Multiline = true;
        textBoxLogDir.Name = "textBoxLogDir";
        textBoxLogDir.Size = new Size(726, 68);
        textBoxLogDir.TabIndex = 3;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(30, 264);
        label3.Name = "label3";
        label3.Size = new Size(195, 25);
        label3.TabIndex = 4;
        label3.Text = "Discord Webhook URL:";
        // 
        // textBoxDiscordWebhookUrl
        // 
        textBoxDiscordWebhookUrl.Location = new Point(33, 292);
        textBoxDiscordWebhookUrl.Margin = new Padding(3, 4, 3, 4);
        textBoxDiscordWebhookUrl.Multiline = true;
        textBoxDiscordWebhookUrl.Name = "textBoxDiscordWebhookUrl";
        textBoxDiscordWebhookUrl.Size = new Size(726, 68);
        textBoxDiscordWebhookUrl.TabIndex = 5;
        // 
        // buttonSave
        // 
        buttonSave.Location = new Point(653, 397);
        buttonSave.Margin = new Padding(3, 4, 3, 4);
        buttonSave.Name = "buttonSave";
        buttonSave.Size = new Size(103, 53);
        buttonSave.TabIndex = 6;
        buttonSave.Text = "Save";
        buttonSave.UseVisualStyleBackColor = true;
        buttonSave.Click += OnSaveButtonClicked;
        // 
        // SettingsForm
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 486);
        Controls.Add(buttonSave);
        Controls.Add(textBoxDiscordWebhookUrl);
        Controls.Add(label3);
        Controls.Add(textBoxLogDir);
        Controls.Add(label2);
        Controls.Add(textBoxWatchingFilePath);
        Controls.Add(label1);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(3, 4, 3, 4);
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