using Never.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Never.TestConsole.Serialization
{
    /// <summary>
    /// 对jsontext分析的读取
    /// </summary>
    public class ThunderReaderTest
    {
        [Xunit.Fact]
        public void TestParse1()
        {
            var json = "1";
            var list = ThunderReader.Load(json);
            var node = list.FirstOrDefault();
            Xunit.Assert.NotNull(node);
            Xunit.Assert.True(node.Escaping == false);
            Xunit.Assert.True(node.Segment.Offset == 0);
            Xunit.Assert.True(node.Segment.Offset + node.Segment.Count == 1);
            Xunit.Assert.True(node.Key == "");
            Xunit.Assert.True(node.Level == 0);
            Xunit.Assert.True(node.Node == null);
            Xunit.Assert.True(node.NodeType == Never.Serialization.Json.Deserialize.ContentNodeType.String);
            Xunit.Assert.True(node.Original == json);
            Xunit.Assert.True(node.Segment != null);
            Xunit.Assert.True(node.Segment.Count == 1);
            Xunit.Assert.True(node.ToString() == json);
        }

        [Xunit.Fact]
        public void TestParse1_1()
        {
            var json = "  1";
            var list = ThunderReader.Load(json);
            var node = list.FirstOrDefault();
            Xunit.Assert.NotNull(node);
            Xunit.Assert.True(node.Escaping == false);
            Xunit.Assert.True(node.Segment.Offset == 2);
            Xunit.Assert.True(node.Segment.Offset + node.Segment.Count == 3);
            Xunit.Assert.True(node.Key == "");
            Xunit.Assert.True(node.Level == 0);
            Xunit.Assert.True(node.Node == null);
            Xunit.Assert.True(node.NodeType == Never.Serialization.Json.Deserialize.ContentNodeType.String);
            Xunit.Assert.True(node.Original == json);
            Xunit.Assert.True(node.Segment != null);
            //Xunit.Assert.True(node.Segment[0] == ' ');
            //Xunit.Assert.True(node.Segment[1] == ' ');
            //Xunit.Assert.True(node.Segment[2] == '1');
            Xunit.Assert.True(node.Segment.Count == 1);
            Xunit.Assert.True(node.ToString() == "1");
        }

        [Xunit.Fact]
        public void TestParse1_2()
        {
            var json = "  1 ";
            var list = ThunderReader.Load(json);
            var node = list.FirstOrDefault();
            Xunit.Assert.NotNull(node);
            Xunit.Assert.True(node.Escaping == false);
            Xunit.Assert.True(node.Segment.Offset == 0);
            Xunit.Assert.True(node.Segment.Offset + node.Segment.Count == 3);
            Xunit.Assert.True(node.Key == "");
            Xunit.Assert.True(node.Level == 0);
            Xunit.Assert.True(node.Node == null);
            Xunit.Assert.True(node.NodeType == Never.Serialization.Json.Deserialize.ContentNodeType.String);
            Xunit.Assert.True(node.Original == json);
            Xunit.Assert.True(node.Segment != null);
            //Xunit.Assert.True(node.Segment[0] == ' ');
            //Xunit.Assert.True(node.Segment[1] == ' ');
            //Xunit.Assert.True(node.Segment[2] == '1');
            //Xunit.Assert.True(node.Segment[3] == ' ');
            Xunit.Assert.True(node.Segment.Count == 4);
            Xunit.Assert.True(node.ToString() == json);
        }

        [Xunit.Fact]
        public void TestParse1_3()
        {
            var json = "  1 b";
            var list = ThunderReader.Load(json);
            var node = list.FirstOrDefault();
            Xunit.Assert.NotNull(node);
            Xunit.Assert.True(node.Escaping == false);
            Xunit.Assert.True(node.Segment.Offset == 0);
            Xunit.Assert.True(node.Segment.Offset + node.Segment.Count == 4);
            Xunit.Assert.True(node.Key == "");
            Xunit.Assert.True(node.Level == 0);
            Xunit.Assert.True(node.Node == null);
            Xunit.Assert.True(node.NodeType == Never.Serialization.Json.Deserialize.ContentNodeType.String);
            Xunit.Assert.True(node.Original == json);
            Xunit.Assert.True(node.Segment != null);
            //Xunit.Assert.True(node.Segment[0] == ' ');
            //Xunit.Assert.True(node.Segment[1] == ' ');
            //Xunit.Assert.True(node.Segment[2] == '1');
            //Xunit.Assert.True(node.Segment[3] == ' ');
            //Xunit.Assert.True(node.Segment[4] == 'b');
            Xunit.Assert.True(node.Segment.Count == 5);
            Xunit.Assert.True(node.ToString() == json);
        }

        [Xunit.Fact]
        public void TestParse1_4()
        {
            var json = "a  1";
            var list = ThunderReader.Load(json);
            var node = list.FirstOrDefault();
            Xunit.Assert.NotNull(node);
            Xunit.Assert.True(node.Escaping == false);
            Xunit.Assert.True(node.Segment.Offset == 0);
            Xunit.Assert.True(node.Segment.Offset + node.Segment.Count == 3);
            Xunit.Assert.True(node.Key == "");
            Xunit.Assert.True(node.Level == 0);
            Xunit.Assert.True(node.Node == null);
            Xunit.Assert.True(node.NodeType == Never.Serialization.Json.Deserialize.ContentNodeType.String);
            Xunit.Assert.True(node.Original == json);
            //Xunit.Assert.True(node.Segment != null);
            //Xunit.Assert.True(node.Segment[0] == 'a');
            //Xunit.Assert.True(node.Segment[1] == ' ');
            //Xunit.Assert.True(node.Segment[2] == ' ');
            //Xunit.Assert.True(node.Segment[3] == '1');
            Xunit.Assert.True(node.Segment.Count == 4);
            Xunit.Assert.True(node.ToString() == json);
        }

        [Xunit.Fact]
        public void TestParse1_5()
        {
            var json = "a  1 ";
            var list = ThunderReader.Load(json);
            var node = list.FirstOrDefault();
            Xunit.Assert.NotNull(node);
            Xunit.Assert.True(node.Escaping == false);
            Xunit.Assert.True(node.Segment.Offset == 0);
            Xunit.Assert.True(node.Segment.Offset + node.Segment.Count == 4);
            Xunit.Assert.True(node.Key == "");
            Xunit.Assert.True(node.Level == 0);
            Xunit.Assert.True(node.Node == null);
            Xunit.Assert.True(node.NodeType == Never.Serialization.Json.Deserialize.ContentNodeType.String);
            Xunit.Assert.True(node.Original == json);
            Xunit.Assert.True(node.Segment != null);
            //Xunit.Assert.True(node.Segment[0] == 'a');
            //Xunit.Assert.True(node.Segment[1] == ' ');
            //Xunit.Assert.True(node.Segment[2] == ' ');
            //Xunit.Assert.True(node.Segment[3] == '1');
            //Xunit.Assert.True(node.Segment[4] == ' ');
            Xunit.Assert.True(node.Segment.Count == 5);
            Xunit.Assert.True(node.ToString() == json);
        }

        [Xunit.Fact]
        public void TestParse1_6()
        {
            var json = " a  1 ";
            var list = ThunderReader.Load(json);
            var node = list.FirstOrDefault();
            Xunit.Assert.NotNull(node);
            Xunit.Assert.True(node.Escaping == false);
            Xunit.Assert.True(node.Segment.Offset == 0);
            Xunit.Assert.True(node.Segment.Offset + node.Segment.Count == 5);
            Xunit.Assert.True(node.Key == "");
            Xunit.Assert.True(node.Level == 0);
            Xunit.Assert.True(node.Node == null);
            Xunit.Assert.True(node.NodeType == Never.Serialization.Json.Deserialize.ContentNodeType.String);
            Xunit.Assert.True(node.Original == json);
            Xunit.Assert.True(node.Segment != null);
            //Xunit.Assert.True(node.Segment[0] == ' ');
            //Xunit.Assert.True(node.Segment[1] == 'a');
            //Xunit.Assert.True(node.Segment[2] == ' ');
            //Xunit.Assert.True(node.Segment[3] == ' ');
            //Xunit.Assert.True(node.Segment[4] == '1');
            //Xunit.Assert.True(node.Segment[5] == ' ');
            Xunit.Assert.True(node.Segment.Count == 6);
            Xunit.Assert.True(node.ToString() == json);
        }

        [Xunit.Fact]
        public void TestParse1_7()
        {
            var json = " a  1 e";
            var list = ThunderReader.Load(json);
            var node = list.FirstOrDefault();
            Xunit.Assert.NotNull(node);
            Xunit.Assert.True(node.Escaping == false);
            Xunit.Assert.True(node.Segment.Offset == 0);
            Xunit.Assert.True(node.Segment.Offset + node.Segment.Count == 6);
            Xunit.Assert.True(node.Key == "");
            Xunit.Assert.True(node.Level == 0);
            Xunit.Assert.True(node.Node == null);
            Xunit.Assert.True(node.NodeType == Never.Serialization.Json.Deserialize.ContentNodeType.String);
            Xunit.Assert.True(node.Original == json);
            Xunit.Assert.True(node.Segment != null);
            //Xunit.Assert.True(node.Segment[0] == ' ');
            //Xunit.Assert.True(node.Segment[1] == 'a');
            //Xunit.Assert.True(node.Segment[2] == ' ');
            //Xunit.Assert.True(node.Segment[3] == ' ');
            //Xunit.Assert.True(node.Segment[4] == '1');
            //Xunit.Assert.True(node.Segment[5] == ' ');
            //Xunit.Assert.True(node.Segment[6] == 'e');
            Xunit.Assert.True(node.Segment.Count == 7);
            Xunit.Assert.True(node.ToString() == json);
        }

        [Xunit.Fact]
        public void TestParse1_7_1()
        {
            var json = " a \\\\ 1 e";
            var list = ThunderReader.Load(json);
            var node = list.FirstOrDefault();
            Xunit.Assert.NotNull(node);
            Xunit.Assert.True(node.Escaping == true);
            Xunit.Assert.True(node.Segment.Offset == 0);
            Xunit.Assert.True(node.Segment.Offset + node.Segment.Count == 8);
            Xunit.Assert.True(node.Key == "");
            Xunit.Assert.True(node.Level == 0);
            Xunit.Assert.True(node.Node == null);
            Xunit.Assert.True(node.NodeType == Never.Serialization.Json.Deserialize.ContentNodeType.String);
            Xunit.Assert.True(node.Original == json);
            Xunit.Assert.True(node.Segment != null);
            //Xunit.Assert.True(node.Segment[0] == ' ');
            //Xunit.Assert.True(node.Segment[1] == 'a');
            //Xunit.Assert.True(node.Segment[2] == ' ');
            //Xunit.Assert.True(node.Segment[3] == '\\');
            //Xunit.Assert.True(node.Segment[4] == '\\');
            //Xunit.Assert.True(node.Segment[5] == ' ');
            //Xunit.Assert.True(node.Segment[6] == '1');
            //Xunit.Assert.True(node.Segment[7] == ' ');
            //Xunit.Assert.True(node.Segment[8] == 'e');
            Xunit.Assert.True(node.Segment.Count == 9);
            Xunit.Assert.True(node.ToString() == json);
        }

        [Xunit.Fact]
        public void TestParse2()
        {
            var json = "[a]";
            var list = ThunderReader.Load(json);
        }

        [Xunit.Fact]
        public void TestParse2_1()
        {
            var json = " [a,b,c] ";
            var list = ThunderReader.Load(json);
        }

        [Xunit.Fact]
        public void TestParse2_3()
        {
            var json = "['a','\\\\b','c']";
            var list = ThunderReader.Load(json);
        }

        [Xunit.Fact]
        public void TestParse3()
        {
            var json = "{a:a,b: 'e',c: c} ";
            var list = ThunderReader.Load(json);
        }
    }
}