namespace RNGNewAuraNotifier
{
    /// <summary>
    /// VRChatのユーザ情報を格納するクラス
    /// <summary>
    public class VRChatUser
    {
        /// <summary>
        /// ユーザ名
        /// </summary>
        /// <example>Tomachi</example>
        public string UserName { get; set; }

        /// <summary>
        /// ユーザID
        /// </summary>
        /// <example>usr_0b83d9be-9852-42dd-98e2-625062400acc</example>
        public string UserId { get; set; }
    }
}
