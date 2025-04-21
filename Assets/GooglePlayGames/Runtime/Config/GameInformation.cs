using System;

using UnityEditor;
using UnityEngine;

namespace GooglePlayGames.Config {

    [CreateAssetMenu(fileName = FileName, menuName = "Google/Play Games/Game Information")]
    public class GameInformation : ScriptableObject {

        public const string FileName = "GooglePlayGames.GameInformation";

        private static GameInformation s_instance;

        public static GameInformation Instance
        {
            get {
                if (s_instance == null) s_instance = Resources.Load<GameInformation>(FileName);
                return s_instance;
            }
        }

        public static bool HasInstance => Instance != null;

        public static GameInformation CreateAsset()
        {
            var asset = CreateInstance<GameInformation>();
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(asset, $"Assets/Resources/{FileName}.asset");
            AssetDatabase.SaveAssets();
#endif
            return Instance;
        }

        [SerializeField] private string m_appId;
        [SerializeField] private string m_iosClientId;
        [SerializeField] private string m_nearbyConnectionsServiceId;
        [SerializeField] private string m_webClientId;

        public string AppId    { get => m_appId;                      set => m_appId                      = value; }
        public string IosId    { get => m_iosClientId;                set => m_iosClientId                = value; }
        public string NearbyId { get => m_nearbyConnectionsServiceId; set => m_nearbyConnectionsServiceId = value; }
        public string WebId    { get => m_webClientId;                set => m_webClientId                = value; }

        public bool HasAppId    => !string.IsNullOrEmpty(AppId);
        public bool HasIosId    => !string.IsNullOrEmpty(IosId);
        public bool HasNearbyId => !string.IsNullOrEmpty(NearbyId);
        public bool HasWebId    => !string.IsNullOrEmpty(WebId);

        #region Backwards compatibility

        [Obsolete("Use Instance.AppId instead")]    public static string ApplicationId             => HasInstance ? Instance.AppId : string.Empty;
        [Obsolete("Use Instance.WebId instead")]    public static string WebClientId               => HasInstance ? Instance.WebId : string.Empty;
        [Obsolete("Use Instance.NearbyId instead")] public static string NearbyConnectionServiceId => HasInstance ? Instance.NearbyId : string.Empty;

        [Obsolete("Use Instance.HasAppId instead")]    public static bool ApplicationIdInitialized()     => HasInstance && Instance.HasAppId;
        [Obsolete("Use Instance.HasNearbyId instead")] public static bool NearbyConnectionsInitialized() => HasInstance && Instance.HasNearbyId;
        [Obsolete("Use Instance.HasWebId instead")]    public static bool WebClientIdInitialized()       => HasInstance && Instance.HasWebId;

        #endregion Backward compatibility

    }

}