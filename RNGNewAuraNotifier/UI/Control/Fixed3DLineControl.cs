using System;
using System.Drawing;
using System.Windows.Forms;

namespace RNGNewAuraNotifier.UI.Controls
{
    /// <summary>
    /// Fixed3D風の水平線を描画するユーザーコントロール
    /// </summary>
    public class Fixed3DLineControl : Control
    {
        public Fixed3DLineControl()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Height = 15;
            this.Width = 100;
        }

        /// <summary>
        /// コントロールの描画を行うメソッド
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            int centerY = this.Height / 2;

            // 上: 明るい色, 下: 暗い色（2px分の線を中央に描画）
            g.DrawLine(SystemPens.ControlLightLight, 0, centerY - 1, this.Width, centerY - 1);
            g.DrawLine(SystemPens.ControlDark, 0, centerY, this.Width, centerY);
        }
    }
}
