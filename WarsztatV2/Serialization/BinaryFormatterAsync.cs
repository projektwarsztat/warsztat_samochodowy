using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    public class BinaryFormatterAsync
    {
        private BinaryFormatter bF;

        public BinaryFormatterAsync()
        {
            bF = new BinaryFormatter();
        }
        public BinaryFormatterAsync(BinaryFormatter bF)
        {
            this.bF = bF;
        }

        public void Serialize<T>(Stream stream, T data)
        {
            bF.Serialize(stream, data);
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T) bF.Deserialize(stream);
        }

        public Task SerializeAsync<T>(Stream stream, T data)
        {
            return Task.Factory.StartNew(() => Serialize(stream, data));
        }

        public Task<T> DeserializeAsync<T>(Stream stream)
        {
            return Task<T>.Factory.StartNew(() => Deserialize<T>(stream));
        }
    }
}
