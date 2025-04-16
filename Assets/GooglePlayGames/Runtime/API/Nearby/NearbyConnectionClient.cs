using GooglePlayGames.Utils;

using AED  = GooglePlayGames.Api.Nearby.EndpointDetails;
using AIDL = GooglePlayGames.Api.Nearby.IDiscoveryListener;
using AIML = GooglePlayGames.Api.Nearby.IMessageListener;

namespace GooglePlayGames.Api.Nearby {

    internal sealed class UiDiscoveryListener : AIDL {

        private readonly AIDL m_listener;

        public UiDiscoveryListener(AIDL listener)
        {
            m_listener = Misc.CheckNotNull(listener);
        }

        public void OnEndpointFound(AED details) => Utility.RunUiAction(() => m_listener.OnEndpointFound(details));

        public void OnEndpointLost(string endpointId) => Utility.RunUiAction(() => m_listener.OnEndpointLost(endpointId));

    }

    internal sealed class UiMessageListener : AIML {

        private readonly AIML m_listener;

        public UiMessageListener(AIML listener)
        {
            m_listener = Misc.CheckNotNull(listener);
        }

        public void OnMessageReceived(string endpointId, byte[] data, bool reliable) => Utility.RunUiAction(() => m_listener.OnMessageReceived(endpointId, data, reliable));

        public void OnRemoteEndpointDisconnected(string endpointId) => Utility.RunUiAction(() => m_listener.OnRemoteEndpointDisconnected(endpointId));

    }

}