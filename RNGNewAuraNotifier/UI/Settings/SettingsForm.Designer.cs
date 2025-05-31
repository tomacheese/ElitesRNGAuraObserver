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
        label3 = new Label();
        textBoxDiscordWebhookUrl = new TextBox();
        buttonSave = new Button();
        checkBoxToastNotification = new CheckBox();
        checkBoxStartup = new CheckBox();
        label4 = new Label();
        textBoxConfigDir = new TextBox();
        buttonConfigDirBrowse = new Button();
        label5 = new Label();
        labelJsonVersion = new Label();
        folderBrowserDialog = new FolderBrowserDialog();
        label6 = new Label();
        labelAppVersion = new Label();
        label7 = new Label();
        label8 = new Label();
        label9 = new Label();
        fixed3dLineControl1 = new RNGNewAuraNotifier.UI.Controls.Fixed3DLineControl();
        fixed3dLineControl2 = new RNGNewAuraNotifier.UI.Controls.Fixed3DLineControl();
        fixed3dLineControl3 = new RNGNewAuraNotifier.UI.Controls.Fixed3DLineControl();
        label10 = new Label();
        fixed3dLineControl4 = new RNGNewAuraNotifier.UI.Controls.Fixed3DLineControl();
        buttonSendTest = new Button();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(12, 46);
        label1.Margin = new Padding(2, 0, 2, 0);
        label1.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        label1.Name = "label1";
        label1.Size = new Size(114, 15);
        label1.TabIndex = 0;
        label1.Text = "Monitoring Log File:";
        // 
        // textBoxWatchingFilePath
        // 
        textBoxWatchingFilePath.Location = new Point(162, 43);
        textBoxWatchingFilePath.Margin = new Padding(2, 10, 2, 2);
        textBoxWatchingFilePath.Multiline = true;
        textBoxWatchingFilePath.Name = "textBoxWatchingFilePath";
        textBoxWatchingFilePath.ReadOnly = true;
        textBoxWatchingFilePath.Size = new Size(474, 23);
        textBoxWatchingFilePath.TabIndex = 1;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(12, 121);
        label3.Margin = new Padding(2, 0, 2, 0);
        label3.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        label3.Name = "label3";
        label3.Size = new Size(128, 15);
        label3.TabIndex = 4;
        label3.Text = "Discord Webhook URL:";
        // 
        // textBoxDiscordWebhookUrl
        // 
        textBoxDiscordWebhookUrl.Location = new Point(162, 118);
        textBoxDiscordWebhookUrl.Margin = new Padding(2);
        textBoxDiscordWebhookUrl.Multiline = true;
        textBoxDiscordWebhookUrl.Name = "textBoxDiscordWebhookUrl";
        textBoxDiscordWebhookUrl.PlaceholderText = "Paste your Discord Webhook URL here";
        textBoxDiscordWebhookUrl.Size = new Size(474, 69);
        textBoxDiscordWebhookUrl.TabIndex = 5;
        // 
        // buttonSave
        // 
        buttonSave.Location = new Point(593, 492);
        buttonSave.Margin = new Padding(2);
        buttonSave.Name = "buttonSave";
        buttonSave.Size = new Size(128, 23);
        buttonSave.TabIndex = 6;
        buttonSave.Text = "Save";
        buttonSave.UseVisualStyleBackColor = true;
        buttonSave.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        buttonSave.Click += OnSaveButtonClicked;
        // 
        // checkBoxToastNotification
        // 
        checkBoxToastNotification.AutoSize = true;
        checkBoxToastNotification.Location = new Point(13, 275);
        checkBoxToastNotification.Name = "checkBoxToastNotification";
        checkBoxToastNotification.Size = new Size(154, 19);
        checkBoxToastNotification.TabIndex = 11;
        checkBoxToastNotification.Text = "Enable Toast notification";
        checkBoxToastNotification.UseVisualStyleBackColor = true;
        checkBoxToastNotification.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        // 
        // checkBoxStartup
        // 
        checkBoxStartup.AutoSize = true;
        checkBoxStartup.Location = new Point(13, 303);
        checkBoxStartup.Margin = new Padding(3, 6, 3, 3);
        checkBoxStartup.Name = "checkBoxStartup";
        checkBoxStartup.Size = new Size(165, 19);
        checkBoxStartup.TabIndex = 12;
        checkBoxStartup.Text = "Start when Windows starts";
        checkBoxStartup.UseVisualStyleBackColor = true;
        checkBoxStartup.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(13, 344);
        label4.Margin = new Padding(2, 6, 2, 0);
        label4.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        label4.Name = "label4";
        label4.Size = new Size(116, 15);
        label4.TabIndex = 13;
        label4.Text = "Config File Directory:";
        // 
        // textBoxConfigDir
        // 
        textBoxConfigDir.Location = new Point(162, 341);
        textBoxConfigDir.Margin = new Padding(3, 6, 3, 3);
        textBoxConfigDir.Name = "textBoxConfigDir";
        textBoxConfigDir.PlaceholderText = "Enter the path to the config file";
        textBoxConfigDir.Size = new Size(474, 23);
        textBoxConfigDir.TabIndex = 14;
        // 
        // buttonConfigDirBrowse
        // 
        buttonConfigDirBrowse.Location = new Point(641, 341);
        buttonConfigDirBrowse.Name = "buttonConfigDirBrowse";
        buttonConfigDirBrowse.Size = new Size(80, 23);
        buttonConfigDirBrowse.TabIndex = 15;
        buttonConfigDirBrowse.Text = "Browse...";
        buttonConfigDirBrowse.UseVisualStyleBackColor = true;
        buttonConfigDirBrowse.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        buttonConfigDirBrowse.Click += ButtonConfigDirBrowse_Click;
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Location = new Point(13, 445);
        label5.Margin = new Padding(3, 6, 3, 0);
        label5.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        label5.Name = "label5";
        label5.Size = new Size(102, 15);
        label5.TabIndex = 18;
        label5.Text = "Aura data version:";
        // 
        // labelJsonVersion
        // 
        labelJsonVersion.AutoSize = true;
        labelJsonVersion.Location = new Point(130, 445);
        labelJsonVersion.Margin = new Padding(3, 6, 3, 0);
        labelJsonVersion.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        labelJsonVersion.Name = "labelJsonVersion";
        labelJsonVersion.Size = new Size(61, 15);
        labelJsonVersion.TabIndex = 19;
        labelJsonVersion.Text = "0000.00.00";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Location = new Point(12, 424);
        label6.Margin = new Padding(3, 6, 3, 0);
        label6.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        label6.Name = "label6";
        label6.Size = new Size(112, 15);
        label6.TabIndex = 20;
        label6.Text = "Application version:";
        // 
        // labelAppVersion
        // 
        labelAppVersion.AutoSize = true;
        labelAppVersion.Location = new Point(130, 424);
        labelAppVersion.Margin = new Padding(3, 6, 3, 0);
        labelAppVersion.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        labelAppVersion.Name = "labelAppVersion";
        labelAppVersion.Size = new Size(31, 15);
        labelAppVersion.TabIndex = 21;
        labelAppVersion.Text = "0.0.0";
        // 
        // label7
        // 
        label7.AutoSize = true;
        label7.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        label7.Location = new Point(12, 15);
        label7.Margin = new Padding(3, 6, 3, 6);
        label7.Name = "label7";
        label7.Size = new Size(134, 15);
        label7.TabIndex = 22;
        label7.Text = "Monitoring Information";
        // 
        // label8
        // 
        label8.AutoSize = true;
        label8.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        label8.Location = new Point(13, 98);
        label8.Margin = new Padding(3, 6, 3, 6);
        label8.Name = "label8";
        label8.Size = new Size(94, 15);
        label8.TabIndex = 23;
        label8.Text = "Discord Settings";
        // 
        // label9
        // 
        label9.AutoSize = true;
        label9.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        label9.Location = new Point(12, 251);
        label9.Margin = new Padding(3, 6, 3, 6);
        label9.Name = "label9";
        label9.Size = new Size(114, 15);
        label9.TabIndex = 24;
        label9.Text = "Application Settings";
        // 
        // fixed3dLineControl1
        // 
        fixed3dLineControl1.Location = new Point(159, 15);
        fixed3dLineControl1.Name = "fixed3dLineControl1";
        fixed3dLineControl1.Size = new Size(562, 15);
        fixed3dLineControl1.TabIndex = 25;
        fixed3dLineControl1.Text = "fixed3dLineControl1";
        // 
        // fixed3dLineControl2
        // 
        fixed3dLineControl2.Location = new Point(113, 98);
        fixed3dLineControl2.Margin = new Padding(3, 30, 3, 3);
        fixed3dLineControl2.Name = "fixed3dLineControl2";
        fixed3dLineControl2.Size = new Size(608, 15);
        fixed3dLineControl2.TabIndex = 26;
        fixed3dLineControl2.Text = "fixed3dLineControl2";
        // 
        // fixed3dLineControl3
        // 
        fixed3dLineControl3.Location = new Point(132, 251);
        fixed3dLineControl3.Margin = new Padding(3, 30, 3, 3);
        fixed3dLineControl3.Name = "fixed3dLineControl3";
        fixed3dLineControl3.Size = new Size(589, 15);
        fixed3dLineControl3.TabIndex = 27;
        fixed3dLineControl3.Text = "fixed3dLineControl3";
        // 
        // label10
        // 
        label10.AutoSize = true;
        label10.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        label10.Location = new Point(13, 397);
        label10.Margin = new Padding(3, 6, 3, 6);
        label10.Name = "label10";
        label10.Size = new Size(40, 15);
        label10.TabIndex = 28;
        label10.Text = "About";
        // 
        // fixed3dLineControl4
        // 
        fixed3dLineControl4.Location = new Point(59, 397);
        fixed3dLineControl4.Margin = new Padding(3, 30, 3, 3);
        fixed3dLineControl4.Name = "fixed3dLineControl4";
        fixed3dLineControl4.Size = new Size(662, 15);
        fixed3dLineControl4.TabIndex = 29;
        fixed3dLineControl4.Text = "fixed3dLineControl4";
        // 
        // buttonSendTest
        // 
        buttonSendTest.Location = new Point(162, 195);
        buttonSendTest.Margin = new Padding(3, 6, 3, 3);
        buttonSendTest.Name = "buttonSendTest";
        buttonSendTest.Size = new Size(474, 23);
        buttonSendTest.TabIndex = 30;
        buttonSendTest.Text = "Send test message";
        buttonSendTest.UseVisualStyleBackColor = true;
        buttonSendTest.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        buttonSendTest.Click += SendTestMessageAsync;
        // 
        // SettingsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(733, 526);
        Controls.Add(buttonSendTest);
        Controls.Add(fixed3dLineControl4);
        Controls.Add(label10);
        Controls.Add(fixed3dLineControl3);
        Controls.Add(fixed3dLineControl2);
        Controls.Add(fixed3dLineControl1);
        Controls.Add(label9);
        Controls.Add(label8);
        Controls.Add(label7);
        Controls.Add(labelAppVersion);
        Controls.Add(label6);
        Controls.Add(labelJsonVersion);
        Controls.Add(label5);
        Controls.Add(buttonConfigDirBrowse);
        Controls.Add(textBoxConfigDir);
        Controls.Add(label4);
        Controls.Add(checkBoxStartup);
        Controls.Add(checkBoxToastNotification);
        Controls.Add(buttonSave);
        Controls.Add(textBoxDiscordWebhookUrl);
        Controls.Add(label3);
        Controls.Add(textBoxWatchingFilePath);
        Controls.Add(label1);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(2);
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
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textBoxDiscordWebhookUrl;
    private System.Windows.Forms.Button buttonSave;
    private CheckBox checkBoxToastNotification;
    private CheckBox checkBoxStartup;
    private Label label4;
    private TextBox textBoxConfigDir;
    private Button buttonConfigDirBrowse;
    private Label label5;
    private Label labelJsonVersion;
    private FolderBrowserDialog folderBrowserDialog;
    private Label label6;
    private Label labelAppVersion;
    private Label label7;
    private Label label8;
    private Label label9;
    private Controls.Fixed3DLineControl fixed3dLineControl1;
    private Controls.Fixed3DLineControl fixed3dLineControl2;
    private Controls.Fixed3DLineControl fixed3dLineControl3;
    private Label label10;
    private Controls.Fixed3DLineControl fixed3dLineControl4;
    private Button buttonSendTest;
}
