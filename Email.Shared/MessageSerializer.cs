using Email.Shared.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Email.Shared
{
    public static class MessageSerializer
    {
		public static byte[] SerializeIntoBinary(object dto)
		{
			using (var memstr = new MemoryStream())
			{
				var binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memstr, dto);
				memstr.Flush();
				memstr.Seek(0, SeekOrigin.Begin);
				return memstr.GetBuffer();
			}
		}

		public static T DeserialiseFromBinary<T>(byte[] messageBody) where T : class
		{
			using (var memstr = new MemoryStream())
			{
				memstr.Write(messageBody, 0, messageBody.Length);
				memstr.Seek(0, SeekOrigin.Begin);
				var binaryFormatter = new BinaryFormatter();
				return binaryFormatter.Deserialize(memstr) as T;
			}
		}
	}
}
