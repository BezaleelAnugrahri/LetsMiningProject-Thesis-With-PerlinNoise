using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace PerlinNoiseTutorial
{

    public class ThreadedDataRequester : MonoBehaviour
    {
        static ThreadedDataRequester instance;
        Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

        void Awake()
        {
            instance = FindObjectOfType<ThreadedDataRequester>();
        }

        public static void RequestDataMap(Func<object> generateData, Action<object> callback)//object here is from System.object; Func is representing a deligate with the return type of object and with no parameters, so we can use that for generating data object
        {
            //to represent our heightMapThread with the callback parameter
            ThreadStart threadStart = delegate
            {
                instance.DataThread(generateData, callback);
            };

            new Thread(threadStart).Start();
        }

        void DataThread(Func<object> generateData, Action<object> callback)
        {
            //if we call the method from inside of the thread, that method will be executed inside of the same thread as well
            //HeightMap heightMap = HeightMapGenerator.GeneratingHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, estimatedDiv, generateData);
            object data = generateData();
            //we want to add this heightMap along with callback to queue. so, we need the struct which can hold the heightMap and callback variable.
            lock (dataQueue)//look = it's mean when 1 thread reaches this point while executing this code no other thread can be executed and waiting until its done
            {
                dataQueue.Enqueue(new ThreadInfo(callback, data));
            }
        }


        
        void Update()
        {
            if (dataQueue.Count > 0)
            {
                for (int i = 0; i < dataQueue.Count; i++)
                {
                    ThreadInfo threadInfo = dataQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }

        }
                
        struct ThreadInfo
        {
            public readonly Action<object> callback;
            public readonly object parameter;

            public ThreadInfo(Action<object> callback, object parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }

        }

    }

}