namespace RNGNewAuraNotifier
{
    partial class SettingsForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxWatchingFilePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLogDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDiscordWebhookUrl = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Watching LogFile:";
            // 
            // textBoxWatchingFilePath
            // 
            this.textBoxWatchingFilePath.Location = new System.Drawing.Point(33, 120);
            this.textBoxWatchingFilePath.Multiline = true;
            this.textBoxWatchingFilePath.Name = "textBoxWatchingFilePath";
            this.textBoxWatchingFilePath.ReadOnly = true;
            this.textBoxWatchingFilePath.Size = new System.Drawing.Size(726, 50);
            this.textBoxWatchingFilePath.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "LogDirectory:";
            // 
            // textBoxLogDir
            // 
            this.textBoxLogDir.Location = new System.Drawing.Point(33, 30);
            this.textBoxLogDir.Multiline = true;
            this.textBoxLogDir.Name = "textBoxLogDir";
            this.textBoxLogDir.Size = new System.Drawing.Size(726, 50);
            this.textBoxLogDir.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 18);
            this.label3.TabIndex = 4;
            this.label3.Text = "Discord Webhook URL:";
            // 
            // textBoxDiscordWebhookUrl
            // 
            this.textBoxDiscordWebhookUrl.Location = new System.Drawing.Point(33, 210);
            this.textBoxDiscordWebhookUrl.Multiline = true;
            this.textBoxDiscordWebhookUrl.Name = "textBoxDiscordWebhookUrl";
            this.textBoxDiscordWebhookUrl.Size = new System.Drawing.Size(726, 50);
            this.textBoxDiscordWebhookUrl.TabIndex = 5;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(653, 286);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(103, 38);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.OnSaveButtonClicked);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 350);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxDiscordWebhookUrl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxLogDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxWatchingFilePath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "RNGNewAuraNotifier Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

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
}

