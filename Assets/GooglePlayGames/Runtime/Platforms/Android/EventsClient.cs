#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using GooglePlayGames.Android.Java;
using GooglePlayGames.Utils;

using ADS  = GooglePlayGames.Api.DataSource;
using AIE  = GooglePlayGames.Api.Events.IEvent;
using AIEC = GooglePlayGames.Api.Events.IEventsClient;
using ARS  = GooglePlayGames.Api.ResponseStatus;

using JECI = GooglePlayGames.Android.Java.EventsClient.Instance;

namespace GooglePlayGames.Android {

    internal class EventsClient : AIEC {

        private readonly JECI m_jEventsClient;

        public EventsClient()
        {
            m_jEventsClient = PlayGames.JGetEventsClient();
        }

        #region IEventsClient implementation

        public void FetchAllEvents(ADS source, Action<ARS, List<AIE>> callback)
        {
            callback = Utility.ToUiAction(callback);

            using var jTask = m_jEventsClient.JLoad(source == ADS.ReadNetworkOnly);
            jTask.JAddOnSuccessListener(jData => {
                var status = jData.GetResponseStatus();
                var events = null as List<AIE>;
                using (var jEvents = jData.JGet()) {
                    events = Utility.ToAndroidEvent(jEvents).ToList();
                }
                callback(status, events);
            }).JAddOnFailureListener(jException => {
                Debug.Log("FetchAllEvents failed");
                callback(ARS.InternalError, null);
            });
        }

        public void FetchEvent(ADS source, string eventId, Action<ARS, AIE> callback)
        {
            callback = Utility.ToUiAction(callback);

            using var jTask = m_jEventsClient.JLoadByIds(source == ADS.ReadNetworkOnly, new[] { eventId });
            jTask.JAddOnSuccessListener(jData => {
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
                Debug.Log("FetchEvent failed");
                callback.Invoke(ARS.InternalError, null);
            });
        }

        public void IncrementEvent(string id, uint steps) => m_jEventsClient.Increment(id, (int)steps);

        #endregion IEventsClient implementation

    }

}

#endif