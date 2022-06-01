using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    /// <summary>
    /// Klasa zajmująca się serializacją i deserializacją danych, wykorzystywana do komunikacji sieciowej
    /// </summary>
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

        /// <summary>
        /// Metoda generyczna serializująca synchornicznie
        /// </summary>
        public void Serialize<T>(Stream stream, T data)
        {
            bF.Serialize(stream, data);
        }

        /// <summary>
        /// Metoda generyczna deserializująca synchornicznie
        /// </summary>
        public T Deserialize<T>(Stream stream)
        {
            return (T) bF.Deserialize(stream);
        }

        /// <summary>
        /// Metoda generyczna serializująca asynchornicznie
        /// </summary>
        public Task SerializeAsync<T>(Stream stream, T data)
        {
            return Task.Factory.StartNew(() => Serialize(stream, data));
        }

        /// <summary>
        /// Metoda generyczna deserializująca asynchornicznie
        /// </summary>
        public Task<T> DeserializeAsync<T>(Stream stream)
        {
            return Task<T>.Factory.StartNew(() => Deserialize<T>(stream));
        }
    }
}
