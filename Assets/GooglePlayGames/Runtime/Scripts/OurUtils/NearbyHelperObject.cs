#if UNITY_ANDROID

using System;

using UnityEngine;

using GooglePlayGames.BasicApi.Nearby;

namespace GooglePlayGames.OurUtils {

    public sealed class NearbyHelperObject : MonoBehaviour {

        private static double                  s_advertisingRemaining = 0;
        private static INearbyConnectionClient s_client               = null;
        private static double                  s_discoveryRemaining   = 0;
        private static NearbyHelperObject      s_instance             = null;

        public static void CreateObject(INearbyConnectionClient client)
        {
            if (s_instance != null) return;
            s_client = client;
            if (Application.isPlaying) {
                var obj = new GameObject("PlayGames_NearbyHelper");
                DontDestroyOnLoad(obj);
                s_instance = obj.AddComponent<NearbyHelperObject>();
            } else {
                s_instance = new();
            }
        }

        public static void StartAdvertisingTimer(TimeSpan? span) => s_advertisingRemaining = ToSeconds(span);

        public static void StartDiscoveryTimer(TimeSpan? span) => s_discoveryRemaining = ToSeconds(span);

        private static double ToSeconds(TimeSpan? span)
        {
            if (!span.HasValue) return 0;
            if (span.Value.TotalSeconds < 0) return 0;
            return span.Value.TotalSeconds;
        }

        #region MonoBehaviour implementation

        private void Awake() => DontDestroyOnLoad(gameObject);

        private void OnDisable()
        {
            if (s_instance != this) return;
            s_instance = null;
        }

        private void Update()
        {
            if (s_advertisingRemaining > 0) {
                s_advertisingRemaining -= Time.deltaTime;
                if (s_advertisingRemaining < 0) {
                    s_client.StopAdvertising();
                }
            }
            if (s_discoveryRemaining > 0) {
                s_discoveryRemaining -= Time.deltaTime;
                if (s_discoveryRemaining < 0) {
                    s_client.StopDiscovery(s_client.GetServiceId());
                }
            }
        }

        #endregion MonoBehaviour implementation

    }

}

#endif