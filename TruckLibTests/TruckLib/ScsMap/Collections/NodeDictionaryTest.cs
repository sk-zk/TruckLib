using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;
using TruckLib.ScsMap.Collections;
using System.Numerics;

namespace TruckLibTests.TruckLib.ScsMap.Collections
{
    public class NodeDictionaryTest
    {
        [Fact]
        public void Add()
        {
            var dict = new NodeDictionary();
            var node = new Node() { Position = new Vector3(-12, 34, 56) };

            dict.Add(node.Uid, node);

            Assert.Single(dict);
            Assert.True(dict.ContainsKey(node.Uid));
            Assert.Equal(node, dict[node.Uid]);
        }

        [Fact]
        public void Within()
        {
            var dict = new NodeDictionary();
            var node1 = new Node() { Position = new Vector3(10, 0, 10) };
            dict.Add(node1.Uid, node1);
            var node2 = new Node() { Position = new Vector3(-10, 0, -10) };
            dict.Add(node2.Uid, node2);
            var node3 = new Node() { Position = new Vector3(50, 0, 50) };
            dict.Add(node3.Uid, node3);

            var results = dict.Within(0, 0, 20, 20);
            Assert.Single(results);
            Assert.Equal(node1, results[0]);
        }

        [Fact]
        public void Remove()
        {
            var dict = new NodeDictionary();
            var node = new Node() { Position = new Vector3(10, 0, 10) };
            dict.Add(node.Uid, node);

            dict.Remove(node.Uid);

            Assert.Empty(dict);
            Assert.False(dict.ContainsKey(node.Uid));
            Assert.Empty(dict.Within(0, 0, 20, 20));
        }

        [Fact]
        public void IndexerSet()
        {
            var dict = new NodeDictionary();
            var node1 = new Node() 
            { 
                Uid = 42,
                Position = new Vector3(10, 0, 10) 
            };
            dict.Add(node1.Uid, node1);

            var node2 = new Node() 
            { 
                Uid = 42,
                Position = new Vector3(50, 0, 50)
            };
            dict[42] = node2;

            Assert.Empty(dict.Within(0, 0, 20, 20));
            Assert.Equal(node2, dict.Within(40, 40, 60, 60)[0]);

        }
    }
}
