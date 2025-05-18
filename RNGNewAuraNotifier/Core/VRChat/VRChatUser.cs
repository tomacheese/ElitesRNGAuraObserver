namespace RNGNewAuraNotifier.Core.VRChat;

/// <summary>
/// VRChatユーザーの情報を格納するクラス
/// </summary>
internal class VRChatUser
{
    /// <summary>
    /// ユーザー名
    /// </summary>
    /// <example>Tomachi</example>
    public required string UserName { get; set; }

    /// <summary>
    /// ユーザID
    /// </summary>
    /// <example>usr_0b83d9be-9852-42dd-98e2-625062400acc</example>
    public required string UserId { get; set; }
}
