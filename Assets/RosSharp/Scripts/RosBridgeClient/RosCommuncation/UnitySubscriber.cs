﻿/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Threading;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public abstract class UnitySubscriber<T> : MonoBehaviour where T: Message, new()
    {
        public string Topic;
        public float TimeStep;

        private RosConnector rosConnector;
        private readonly int SecondsTimeout = 1;
        private string _topicID;


        protected virtual void Start()
        {
            rosConnector = GetComponent<RosConnector>();
            //new Thread(Subscribe).Start();
        }

        public void SubscribeStatus()
        {
            if (isActiveAndEnabled)
            {
                new Thread(Subscribe).Start();
            }
            else
            {
                UnSubscribe();
            }
        }

        private void Subscribe()
        {
            try
            {
                if (!rosConnector.IsConnected.WaitOne(SecondsTimeout * 100))
                    Debug.LogWarning("Failed to subscribe: RosConnector not connected");

                _topicID = rosConnector.RosSocket.Subscribe<T>(Topic, ReceiveMessage, (int)(TimeStep * 1000)); // the rate(in ms in between messages) at which to throttle the topics          

            }
            catch
            {
                Subscribe();
            }


            //rosConnector.RosSocket.Subscribe<T>(Topic, ReceiveMessage, (int)(TimeStep * 100)); // the rate(in ms in between messages) at which to throttle the topics
        }

        private void UnSubscribe()
        {
            rosConnector.RosSocket.Unsubscribe(_topicID);
        }

        protected abstract void ReceiveMessage(T message);

    }
}