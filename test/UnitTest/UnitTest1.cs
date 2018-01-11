using AsIKnow.WebHelpers;
using System;
using System.Linq;
using Xunit;

namespace UnitTest
{
    public class UnitTest1
    {
        [Trait("Category", "DataExtensions")]
        [Fact(DisplayName = "IsBase64")]
        public void IsBase64()
        {
            Assert.True(Convert.ToBase64String(new byte[] { 0,1,2 }).IsBase64());
            Assert.False("A*A=".IsBase64());
            Assert.True("/A==".IsBase64());
            Assert.True("+w==".IsBase64());
        }

        [Trait("Category", "DataExtensions")]
        [Fact(DisplayName = "FromToBase64")]
        public void FromToBase64()
        {
            Assert.True(new byte[] { 252 }.ToBase64() == "/A==");
            Assert.Throws<ArgumentException>(()=>"A*A=".FromBase64());
            Assert.Collection("/A==".FromBase64(), p=> Assert.True(p == 252));
        }
        
        [Trait("Category", "DataExtensions")]
        [Fact(DisplayName = "InflateDefalte")]
        public void InflateDefalte()
        {
            byte[] test = Enumerable.Range(0, 1000).Select(p=>(byte)p).ToArray();

            byte[] deflated = test.Deflate();

            Assert.True(test.Length >= deflated.Length);
            Assert.True(test.Length == deflated.Inflate().Length);
        }

        [Trait("Category", "DataExtensions")]
        [Fact(DisplayName = "ZipArchive")]
        public void ZipArchive()
        {
            byte[] test = Enumerable.Range(0, 1000).Select(p => (byte)p).ToArray();
            byte[] zip_data;

            using (IArchive zip = new AsIKnow.WebHelpers.ZipArchive())
            {
                zip.AddOrUpdateItem("/prova/value", test);

                zip_data = zip.ToArray();
                zip_data = zip.ToArray();
            }
            
            using (IArchive zip = new AsIKnow.WebHelpers.ZipArchive(zip_data))
            {
                Assert.True(zip.ItemExists("/prova/value"));
                Assert.True(zip.GetItem("/prova/value").Length == test.Length);

                zip.AddOrUpdateItem("/prova/value2", test);

                zip_data = zip.ToArray();
            }

            using (IArchive zip = new AsIKnow.WebHelpers.ZipArchive(zip_data))
            {
                Assert.True(zip.ListItems().Count() == 2);
                zip.RemoveItem("/prova/value");
                Assert.True(zip.ListItems().Count() == 1);

                zip_data = zip.ToArray();
            }

            using (IArchive zip = new AsIKnow.WebHelpers.ZipArchive(zip_data))
            {
                Assert.True(zip.ListItems().Count() == 1);
                Assert.True(zip.ListItems().First() == "/prova/value2");
            }
        }
    }
}
