#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using GooglePlayGames.Android.Java.Extensions;
using GooglePlayGames.Utils;

using Logger = GooglePlayGames.Utils.Logger;

using ADS  = GooglePlayGames.Api.DataSource;
using AIE  = GooglePlayGames.Api.Events.IEvent;
using AIEC = GooglePlayGames.Api.Events.IEventsClient;
using ARS  = GooglePlayGames.Api.ResponseStatus;

using AEC = GooglePlayGames.Android.EventsClient;

using JECI = GooglePlayGames.Android.Java.EventsClient.Instance;
using JPG  = GooglePlayGames.Android.Java.PlayGames;

namespace GooglePlayGames.Android {

    internal class EventsClient : AIEC {

        private readonly JECI m_client;

        public EventsClient()
        {
            m_client = JPG.JGetEventsClient();
        }

        #region IEventsClient implementation

        public void FetchAllEvents(ADS source, Action<ARS, List<AIE>> callback)
        {
            const string method = "IEventsclient.FetchAllEvents(DataSource, Action<ResponseStatus, List<IEvent>>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            using var jTask = m_client.JLoad(source == ADS.ReadNetworkOnly);
            jTask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                var status = jData.GetResponseStatus();
                var events = null as List<AIE>;
                using (var jEvents = jData.JGet()) {
                    events = Utility.ToAndroidEvent(jEvents).ToList();
                }
                callback?.Invoke(status, events);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                callback?.Invoke(ARS.InternalError, null);
            });
        }

        public void FetchEvent(ADS source, string eventId, Action<ARS, AIE> callback)
        {
            const string method = "IEventsclient.FetchEvent(DataSource, string, Action<ResponseStatus, IEvent>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            using var jTask = m_client.JLoadByIds(source == ADS.ReadNetworkOnly, new[] { eventId });
            jTask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                var status = jData.GetResponseStatus();
                var @event = null as AIE;
                using (var jEvents = jData.JGet()) {
                    var count = jEvents.GetCount();
                    if (count <= 0) return;
                    using var jEvent = jEvents.JGet(0);
                    @event = Utility.ToAndroidEvent(jEvent);
                }
                callback.Invoke(status, @event);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                callback.Invoke(ARS.InternalError, null);
            });
        }

        public void IncrementEvent(string id, uint steps)
        {
            const string method = "IEventsclient.IncrementEvent(string, uint)";
            Logger.t($"AND: Calling {method}");

            m_client.Increment(id, (int)steps);
        }

        #endregion IEventsClient implementation

        #region Object implementation

        public override string ToString() => $"EventsClient(client: {m_client})";

        public override int GetHashCode() => HashCode.Combine(m_client);

        public override bool Equals(object other)
        {
            if (other is not AEC it) return false;
            return m_client.Equals(it.m_client);
        }

        #endregion Object implementation

    }

}

#endif