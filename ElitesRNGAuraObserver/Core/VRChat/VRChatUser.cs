namespace ElitesRNGAuraObserver.Core.VRChat;

/// <summary>
/// VRChatユーザーの情報を格納するレコード
/// </summary>
internal record VRChatUser
{
    /// <summary>
    /// ユーザー名
    /// </summary>
    /// <example>Tomachi</example>
    public required string UserName { get; init; }

    /// <summary>
    /// ユーザID
    /// </summary>
    /// <example>usr_0b83d9be-9852-42dd-98e2-625062400acc</example>
    public required string UserId { get; init; }

    /// <summary>
    /// VRChatUserの等価性を比較する
    /// </summary>
    /// <param name="other">比較対象のVRChatUser</param>
    /// <returns>等価であればtrue、そうでなければfalse</returns>
    public virtual bool Equals(VRChatUser? other) => other != null && UserId == other.UserId;

    /// <summary>
    /// ハッシュコードを取得する
    /// </summary>
    /// <returns>ハッシュコード</returns>
    public override int GetHashCode() => UserId.GetHashCode();
}
