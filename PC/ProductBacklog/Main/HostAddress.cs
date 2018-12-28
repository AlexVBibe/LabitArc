using System;

namespace ProductBacklog.Main
{
    public class HostAddress
    {
        public string Address { get; }
        public string Ip { get; }
        public int Port { get; }

        public HostAddress(string fullAddress)
        {
            Address = fullAddress;

            var addressBits = fullAddress.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            Ip = addressBits[0];
            Port = int.Parse(addressBits[1]);
        }
    }
}
