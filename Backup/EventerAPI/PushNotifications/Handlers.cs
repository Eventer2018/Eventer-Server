using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PushSharp.Core;
using PushSharp.Apple;
using PushSharp.Google;
using Newtonsoft.Json.Linq;
using EventerAPI.General;

namespace EventerAPI.PushNotifications
{
    public class Handlers
    {
        public static void PushAPNSNotification(string[] device_tokens, string notification_alert, JObject extra_data){

            var apns_config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox, System.Web.Hosting.HostingEnvironment.MapPath("\\PushNotifications\\Cert\\CertificatesEventer.p12"), "12341234");
            var apns_broker = new ApnsServiceBroker(apns_config);

            apns_broker.OnNotificationFailed += (notification, aggregateEx) =>
            {
                aggregateEx.Handle(ex =>
                {

                    // See what kind of exception it was to further diagnose
                    if (ex is ApnsNotificationException)
                    {
                        var notificationException = (ApnsNotificationException)ex;

                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                        //Logger.Write(string.Format("Apple Notification Failed: ID={0}, Code={1}", apnsNotification.Identifier, statusCode));

                    }
                    else
                    {
                        // Inner exception might hold more useful information like an ApnsConnectionException           
                        //Logger.Write(string.Format("Apple Notification Failed for some unknown reason : {0}", ex.InnerException));
                    }

                    // Mark it as handled
                    return true;
                });
            };

            apns_broker.OnNotificationSucceeded += (notification) =>
            {
                Console.WriteLine("Apple Notification Sent!");
            };

            // Start the broker
            apns_broker.Start();

            JObject _payload = new JObject(JObject.FromObject(new { aps = new { alert = notification_alert, sound = "default" } }));
            if(extra_data != null)
                _payload.Add("notification", extra_data);

            //Logger.Write(string.Format("IOS Push attempt: Device token: {0}, Message {1}", device_token, _payload));
                
            foreach(string device_token in device_tokens)
            {
                apns_broker.QueueNotification(new ApnsNotification
                {
                    DeviceToken = device_token,
                    Payload = _payload//JObject.Parse(string.Format("{{'aps':{{'alert':'{0}', 'sound' : 'default'}}, 'notification' : {{'client_id' : '{1}' }}}}", message, client_id))
                });
            }
            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            apns_broker.Stop();
        }

        public static void PushGCMNotification(string[] device_tokens, string notification_alert, JObject extra_data)
        {
            // Configuration
            var config = new GcmConfiguration("AIzaSyA0nnt5_-oZG1xswA-a1D8HcuxZpyi087s");

            // Create a new broker
            var gcmBroker = new GcmServiceBroker(config);

            // Wire up events
            gcmBroker.OnNotificationFailed += (notification, aggregateEx) =>
            {

                aggregateEx.Handle(ex =>
                {

                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException)
                    {
                        var notificationException = (GcmNotificationException)ex;

                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        Logger.Write(string.Format("GCM Notification Failed: ID={0}, Desc={1}", gcmNotification.MessageId, description));
                    }
                    else if (ex is GcmMulticastResultException)
                    {
                        var multicastException = (GcmMulticastResultException)ex;

                        foreach (var succeededNotification in multicastException.Succeeded)
                        {
                            Logger.Write(string.Format("GCM Notification Failed: ID={0}", succeededNotification.MessageId));

                        }

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            Logger.Write(string.Format("GCM Notification Failed: ID={0}, Desc={1}", n.MessageId, n + " " + e));
                        }

                    }
                    else if (ex is DeviceSubscriptionExpiredException)
                    {
                        var expiredException = (DeviceSubscriptionExpiredException)ex;

                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        Logger.Write(string.Format("Device RegistrationId Expired: {0}", oldId));


                        if (!string.IsNullOrWhiteSpace(newId))
                        {
                            // If this value isn't null, our subscription changed and we should update our database
                            Logger.Write(string.Format("Device RegistrationId Changed To {0}", newId));

                        }
                    }
                    else if (ex is RetryAfterException)
                    {
                        var retryException = (RetryAfterException)ex;
                        // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date

                        //Logger.Write(string.Format("GCM Rate Limited, don't send more until after {0}",retryException.RetryAfterUtc));
                    }
                    else
                    {
                        Logger.Write("GCM Notification Failed for some unknown reason");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            gcmBroker.OnNotificationSucceeded += (notification) =>
            {
                Console.WriteLine("GCM Notification Sent!");
            };

            JObject _payload = JObject.FromObject(new { message = notification_alert });
            if (extra_data != null)
                _payload.Add("data", extra_data);

            Logger.Write(string.Format("GCM Push attempt: Device token: {0}, Message {1}", device_tokens[0], _payload));

            // Start the broker
            gcmBroker.Start();
            gcmBroker.QueueNotification(new GcmNotification
            {
                RegistrationIds = new List<string> {
                    device_tokens[0]
                },
                //Notification = _payload
                Data = _payload

            });


            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            gcmBroker.Stop();
        }
    }
}