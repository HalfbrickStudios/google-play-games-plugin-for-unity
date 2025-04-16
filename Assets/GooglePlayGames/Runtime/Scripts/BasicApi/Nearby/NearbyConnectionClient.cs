using GooglePlayGames.OurUtils;

using AED  = GooglePlayGames.BasicApi.Nearby.EndpointDetails;
using AIDL = GooglePlayGames.BasicApi.Nearby.IDiscoveryListener;
using AIML = GooglePlayGames.BasicApi.Nearby.IMessageListener;

namespace GooglePlayGames.BasicApi.Nearby {

    internal sealed class UiDiscoveryListener : AIDL {

        private readonly AIDL m_listener;

        public UiDiscoveryListener(AIDL listener)
        {
            m_listener = Misc.CheckNotNull(listener);
        }

        public void OnEndpointFound(AED details) => Convert.RunUiAction(() => m_listener.OnEndpointFound(details));

        public void OnEndpointLost(string id) => Convert.RunUiAction(() => m_listener.OnEndpointLost(id));

    }

    internal sealed class UiMessageListener : AIML {

        private readonly AIML m_listener;

        public UiMessageListener(AIML listener)
        {
            m_listener = Misc.CheckNotNull(listener);
        }

        public void OnMessageReceived(string id, byte[] data, bool reliable) => Convert.RunUiAction(() => m_listener.OnMessageReceived(id, data, reliable));

        public void OnRemoteEndpointDisconnected(string id) => Convert.RunUiAction(() => m_listener.OnRemoteEndpointDisconnected(id));

    }

}