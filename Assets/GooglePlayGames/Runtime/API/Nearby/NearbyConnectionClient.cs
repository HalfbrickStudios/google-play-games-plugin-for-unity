using System;

using GooglePlayGames.Utils;

using AED  = GooglePlayGames.Api.Nearby.EndpointDetails;
using AIDL = GooglePlayGames.Api.Nearby.IDiscoveryListener;
using AIML = GooglePlayGames.Api.Nearby.IMessageListener;
using AUDL = GooglePlayGames.Api.Nearby.UiDiscoveryListener;
using AUML = GooglePlayGames.Api.Nearby.UiMessageListener;

namespace GooglePlayGames.Api.Nearby {

    internal sealed class UiDiscoveryListener : AIDL {

        private readonly AIDL m_listener;

        public UiDiscoveryListener(AIDL listener)
        {
            m_listener = Misc.CheckNotNull(listener);
        }

        public void OnEndpointFound(AED details) => Utility.RunUiAction(() => m_listener.OnEndpointFound(details));

        public void OnEndpointLost(string endpointId) => Utility.RunUiAction(() => m_listener.OnEndpointLost(endpointId));

        #region Object implementation

        public override string ToString() => $"UiDiscoveryListener(listener: {m_listener})";

        public override int GetHashCode() => m_listener.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is not AUDL it) return false;
            return m_listener.Equals(it.m_listener);
        }

        #endregion Object implementation

    }

    internal sealed class UiMessageListener : AIML {

        private readonly AIML m_listener;

        public UiMessageListener(AIML listener)
        {
            m_listener = Misc.CheckNotNull(listener);
        }

        public void OnMessageReceived(string endpointId, byte[] data, bool reliable) => Utility.RunUiAction(() => m_listener.OnMessageReceived(endpointId, data, reliable));

        public void OnRemoteEndpointDisconnected(string endpointId) => Utility.RunUiAction(() => m_listener.OnRemoteEndpointDisconnected(endpointId));

        #region Object implementation

        public override string ToString() => $"UiMessageListener(listener: {m_listener})";
        
        public override int GetHashCode() => m_listener.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is not AUML it) return false;
            return m_listener.Equals(it.m_listener);
        }

        #endregion Object implementation

    }

}