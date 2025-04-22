using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace RNGNewAuraNotifier
{
    /// <summary>
    /// Auraの名前を管理するクラス
    /// </summary>
    internal static class Aura
    {
        /// <summary>
        /// 指定されたIDに対応するAuraの名前を取得する
        /// </summary>
        /// <param name="id">AuraのID</param>
        /// <returns>Auraの名前</returns>
        /// <remarks>存在しないIDの場合はnullを返す。マッピングデータは Resources/Auras.json から取得される。</remarks>
        public static string GetAuraName(string id)
        {
            var jsonContent = Encoding.UTF8.GetString(Properties.Resources.Auras);
            Dictionary<string, string> auras = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent) ?? new Dictionary<string, string>();

            return auras.TryGetValue(id, out var name) ? name : null;
        }
    }
}
