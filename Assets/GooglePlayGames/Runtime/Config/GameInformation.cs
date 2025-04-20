using System;
using UnityEngine;

namespace GooglePlayGames.Config {

    [CreateAssetMenu(fileName = FileName, menuName = "Google/Play Games/Game Information")]
    public class GameInformation : ScriptableObject {

        private const string FileName = "GooglePlayGames.Config.GameInformation";

        private static GameInformation s_instance;

        public static GameInformation Instance
        {
            get {
                if (s_instance == null) s_instance = Resources.Load<GameInformation>(FileName);
                return s_instance;
            }
        }

        public static bool HasInstance => Instance != null;

        [SerializeField] private string m_appId;
        [SerializeField] private string m_iosClientId;
        [SerializeField] private string m_nearbyConnectionsServiceId;
        [SerializeField] private string m_webClientId;

        public string AppId    => m_appId;
        public string IosId    => m_iosClientId;
        public string NearbyId => m_nearbyConnectionsServiceId;
        public string WebId    => m_webClientId;

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