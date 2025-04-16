using System;

using UnityEngine;

using GooglePlayGames.Api.Nearby;

using UGO = UnityEngine.GameObject;
using UMO = UnityEngine.MonoBehaviour;

using GPGNHO = GooglePlayGames.Utils.NearbyHelperObject;

using AINCC = GooglePlayGames.Api.Nearby.INearbyConnectionClient;

namespace GooglePlayGames.Utils {

    public sealed class NearbyHelperObject : UMO {

        private static double s_advertisingTimer = 0;
        private static AINCC  s_client           = null;
        private static double s_discoveryTimer   = 0;
        private static GPGNHO s_instance         = null;

        public static void CreateObject(AINCC client)
        {
            if (s_instance != null) return;
            s_client = client;
            if (Application.isPlaying) {
                var obj = new UGO("GpgNearbyHelper");
                DontDestroyOnLoad(obj);
                s_instance = obj.AddComponent<GPGNHO>();
            } else {
                s_instance = new();
            }
        }

        public static void StartAdvertisingTimer(TimeSpan? span) => s_advertisingTimer = ToSeconds(span);

        public static void StartDiscoveryTimer(TimeSpan? span) => s_discoveryTimer = ToSeconds(span);

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
            if (s_advertisingTimer > 0) {
                s_advertisingTimer -= Time.deltaTime;
                if (s_advertisingTimer < 0) {
                    s_client.StopAdvertising();
                }
            }
            if (s_discoveryTimer > 0) {
                s_discoveryTimer -= Time.deltaTime;
                if (s_discoveryTimer < 0) {
                    s_client.StopDiscovery(s_client.GetServiceId());
                }
            }
        }

        #endregion MonoBehaviour implementation

    }

}