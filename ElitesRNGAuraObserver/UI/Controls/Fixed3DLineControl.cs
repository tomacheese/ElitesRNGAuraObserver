namespace ElitesRNGAuraObserver.UI.Controls;

/// <summary>
/// Fixed3D風の水平線を描画するユーザーコントロール
/// </summary>
internal class Fixed3DLineControl : Control
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Fixed3DLineControl()
    {
        SetStyle(ControlStyles.ResizeRedraw, true);
        Height = 15;
        Width = 100;
    }

    /// <summary>
    /// コントロールの描画を行うメソッド
    /// </summary>
    /// <param name="e">ペイントイベント引数</param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        var centerY = Height / 2;

        // 上: 明るい色, 下: 暗い色（2px分の線を中央に描画）
        g.DrawLine(SystemPens.ControlLightLight, 0, centerY - 1, Width, centerY - 1);
        g.DrawLine(SystemPens.ControlDark, 0, centerY, Width, centerY);
    }
}
