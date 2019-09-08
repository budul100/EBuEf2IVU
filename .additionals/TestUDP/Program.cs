using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Jarloo.Listener
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var multicastAddress = IPAddress.Parse("224.0.0.1");
            var endpoint = new IPEndPoint(
                address: IPAddress.Any,
                port: 4447);

            using (var client = new UdpClient())
            {
                client.Client.SetSocketOption(
                    optionLevel: SocketOptionLevel.Socket,
                    optionName: SocketOptionName.ReuseAddress,
                    optionValue: true);
                client.ExclusiveAddressUse = false;

                client.Client.Bind(endpoint);
                client.JoinMulticastGroup(multicastAddress);

                var receivedResults = client.Receive(ref endpoint);

                var result = Encoding.ASCII.GetString(
                    bytes: receivedResults,
                    index: 0,
                    count: receivedResults.Length)
                    .TrimEnd('\0');

            }
        }
    }
}