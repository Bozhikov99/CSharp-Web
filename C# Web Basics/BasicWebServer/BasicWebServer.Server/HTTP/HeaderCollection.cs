using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.HTTP
{
    public class HeaderCollection : IEnumerable<Header>
    {
        private readonly Dictionary<string, Header> headers;

        public HeaderCollection() => headers = new Dictionary<string, Header>();

        public int Count => headers.Count;

        public void Add(string name, string value) => headers[name] = new Header(name, value);

        public bool Contains(string name) => headers.ContainsKey(name);

        public string this[string name] => headers[name].Value;

        public IEnumerator<Header> GetEnumerator() => headers.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
