﻿using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Mix.Heart.Enums;
using Mix.Heart.Exceptions;
using Mix.Shared.Models.Configurations;
using Newtonsoft.Json;

namespace Mix.Communicator.Services
{
    // Ref: https://firebase.google.com/docs/cloud-messaging/send-message
    public class FirebaseService
    {
        private readonly GoogleSettingModel _settings = new GoogleSettingModel();
        public FirebaseService(IConfiguration configuration)
        {
            configuration.GetSection(MixAppSettingsSection.Google).Bind(_settings);
            if (!string.IsNullOrEmpty(_settings.ProjectId) && _settings.Firebase.Credential != null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(_settings.Firebase.Credential))
                });
            }
        }

        public async Task<FirebaseToken> VerifyTokenAsync(string idToken)
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            return decodedToken;
        }

        public async Task<string> SendNotificationToDevice(
            string registrationToken,
            Notification? notification = default)
        {
            try
            {

                // This registration token comes from the client FCM SDKs.
                var message = new Message()
                {
                    Notification = notification,
                    Token = registrationToken,
                };

                // Send a message to the device corresponding to the provided
                // registration token.
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);

                // Response is a message ID string.
                Console.WriteLine("Successfully sent message: " + response);

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> SendWebPushToDevice(
            List<string> registrationTokens, WebpushConfig config)
        {
            try
            {

                // This registration token comes from the client FCM SDKs.
                var message = new MulticastMessage()
                {
                    Tokens = registrationTokens,
                    Webpush = config
                };

                // Send a message to the device corresponding to the provided
                // registration token.
                var responses = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

                // Response is a message ID string.
                Console.WriteLine("Successfully sent message: " + responses.SuccessCount);
                Console.WriteLine("Failed sent message: " + responses.FailureCount);
                var failed = responses.Responses.Where(m => !m.IsSuccess).ToList();
                foreach (var item in failed)
                {
                    Console.WriteLine($"{item.MessageId}: {item.Exception}");
                }
                return string.Join(",", responses.Responses.Select(r => r.MessageId));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> SendToMultipleDevices(
            List<string> registrationTokens,
            Notification notification,
            Dictionary<string, string> data)
        {
            // POST a list containing up to 500 registration tokens.
            // These registration tokens come from the client FCM SDKs.
            try
            {
                var message = new MulticastMessage()
                {
                    Tokens = registrationTokens,
                    Data = data,
                    Notification = notification
                };
                var responses = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
                // See the BatchResponse reference documentation
                // for the contents of responses.
                return string.Join(",", responses.Responses.Select(r => r.MessageId));
            }
            catch (Exception ex)
            {
                throw new MixException(MixErrorStatus.Badrequest, ex);
            }
        }

        public async Task SendAll()
        {
            // These registration tokens come from the client FCM SDKs.
            var registrationTokens = new List<string>()
            {
                "YOUR_REGISTRATION_TOKEN_1",
                // ...
                "YOUR_REGISTRATION_TOKEN_n",
            };
            var message = new MulticastMessage()
            {
                Tokens = registrationTokens,
                Data = new Dictionary<string, string>()
                {
                    { "score", "850" },
                    { "time", "2:45" },
                },
            };

            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
            if (response.FailureCount > 0)
            {
                var failedTokens = new List<string>();
                for (var i = 0; i < response.Responses.Count; i++)
                {
                    if (!response.Responses[i].IsSuccess)
                    {
                        // The order of responses corresponds to the order of the registration tokens.
                        failedTokens.Add(registrationTokens[i]);
                    }
                }

                Console.WriteLine($"List of tokens that caused failures: {failedTokens}");
            }
        }

        public async Task SendToTopics(string topic)
        {
            // The topic name can be optionally prefixed with "/topics/".
            // var topic = "highScores";

            // See documentation on defining a message payload.
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { "score", "850" },
                    { "time", "2:45" },
                },
                Topic = topic,
            };

            // Send a message to the devices subscribed to the provided topic.
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            // Response is a message ID string.
            Console.WriteLine("Successfully sent message: " + response);
        }

        public async Task SendWithCondition(string condition)
        {
            // Define a condition which will send to devices which are subscribed
            // to either the Google stock or the tech industry topics.
            // var condition = "'stock-GOOG' in topics || 'industry-tech' in topics";

            // See documentation on defining a message payload.
            var message = new Message()
            {
                Notification = new Notification()
                {
                    Title = "$GOOG up 1.43% on the day",
                    Body = "$GOOG gained 11.80 points to close at 835.67, up 1.43% on the day.",
                },
                Condition = condition,
            };

            // Send a message to devices subscribed to the combination of topics
            // specified by the provided condition.
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            // Response is a message ID string.
            Console.WriteLine("Successfully sent message: " + response);
        }

        public async Task SendBatchMessages(string registrationToken)
        {
            // POST a list containing up to 500 messages.
            var messages = new List<Message>()
{
                new Message()
                {
                    Notification = new Notification()
                    {
                        Title = "Price drop",
                        Body = "5% off all electronics",
                    },
                    Token = registrationToken,
                },
                new Message()
                {
                    Notification = new Notification()
                    {
                        Title = "Price drop",
                        Body = "2% off all books",
                    },
                    Topic = "readers-club",
                },
            };

            var response = await FirebaseMessaging.DefaultInstance.SendAllAsync(messages);
            // See the BatchResponse reference documentation
            // for the contents of responses.
            Console.WriteLine($"{response.SuccessCount} messages were sent successfully");
        }
    }
}
