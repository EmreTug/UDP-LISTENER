using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Models
{
    public class historyModel
    {
        public int Id { get; set; }
        public FixedSizedQueue<byte[]> Data { get; set; }
    }
   
    public class FixedSizedQueue<T>
    {
        private byte[] ObjectToByteArray(object obj)
        {
           return (byte[])obj;
        }
        ConcurrentQueue<T> q = new ConcurrentQueue<T>();
        private object lockObject = new object();

        public int getCount()
        {
            return q.Count;
        }
        public void Enqueue(T obj)
        {
            q.Enqueue(obj);
            lock (lockObject)
            {
                T overflow;
                while (q.Count > 100 && q.TryDequeue(out overflow)) ;
            }
        }
        public List<byte[]> getList()
        {
            List<byte[]> list = new List<byte[]>();
            foreach(var item in q)
            {
                list.Add(ObjectToByteArray(item));
            }
            return list;
        }
    }

}
