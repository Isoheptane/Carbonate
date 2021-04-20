using System;
using System.Text;
using JsonSharp;

namespace Carbonate.Standard
{
    public class Packet
    {
        /*
        *   Packet data is stored in a JSON Object.
        *   One packet has a 4 byte packet head that indicates
        *   the length of the JSON data.
        */
        JsonObject packetData;
        /// <summary>
        /// Create a empty packet.
        /// </summary>
        public Packet() => packetData = new JsonObject();

        /// <summary>
        /// Create a packet from a JSON string.
        /// </summary>
        /// <param name="json">JSON string</param>
        public Packet(string json) => packetData = JsonObject.Parse(json);

        /// <summary>
        /// Create a packet from a JSON object.
        /// </summary>
        /// <param name="json">JSON object</param>
        public Packet(JsonObject json) => packetData = json;

        /// <summary>
        /// Create a packet from a byte array.
        /// </summary>
        /// <param name="rawByteData">Binary data</param>
        public Packet(byte[] rawByteData) => packetData = JsonObject.Parse(Encoding.UTF8.GetString(rawByteData));

        /// <summary>
        /// Create a packet from a byte array.
        /// </summary>
        /// <param name="rawByteData">Binary data</param>
        /// <param name="offset">Binary data offset</param>
        /// <param name="length">Binary data length</param>
        public Packet(byte[] rawByteData, int offset, int length) => packetData = JsonObject.Parse(Encoding.UTF8.GetString(rawByteData, offset, length));

        public JsonValue this[string objectName]
        {
            get 
            {
                return packetData[objectName];
            }
            set
            {
                if(packetData.Exist(objectName))
                    packetData[objectName] = value;
                else
                    packetData.Add(objectName, value);
            }
        }

        /// <summary>
        /// Convert packet into a byte array.
        /// </summary>
        /// <returns>The data byte array</returns>
        public byte[] ToByteArray()
        {
            string jsonString = packetData.ToString();
            int packetLength = Encoding.UTF8.GetByteCount(jsonString);
            byte[] byteArray = new byte[packetLength + 4];

            Array.Copy(BitConverter.GetBytes(packetLength), 0, byteArray, 0, 4);
            Array.Copy(Encoding.UTF8.GetBytes(jsonString), 0, byteArray, 4, packetLength);

            return byteArray;
        }

        /// <summary>
        /// Convert packet into a JSON object.
        /// </summary>
        public JsonObject ToJsonObject() => packetData;

        /// <summary>
        /// Convert packet into a JSON string.
        /// </summary>
        /// <returns>The JSON string</returns>
        public override string ToString() => packetData.ToString();

        /// <summary>
        /// Convert packet into a serialized JSON string.
        /// </summary>
        /// <param name="tab">Tab string</param>
        /// <returns>The serialized JSON string</returns>
        public string Serialize(string tab = "    ") => packetData.Serialize("", tab);

    }
}
