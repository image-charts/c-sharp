using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ImageChartsLib;

namespace ImageChartsTest
{
    [TestClass]
    public class ImageChartsTest
    {
        private BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        private ImageCharts defaultBuilder = new ImageCharts(null, null, null, null, null, null);

        // CI user-agent to bypass rate limiting (set in CI environment)
        private static readonly string CI_USER_AGENT = Environment.GetEnvironmentVariable("IMAGE_CHARTS_USER_AGENT");

        // Helper to create ImageCharts with CI user-agent if set
        private ImageCharts CreateImageCharts()
        {
            return new ImageCharts(null, null, null, null, null, null, CI_USER_AGENT);
        }

        [TestInitialize]
        public void WaitBetweenTests()
        {
            // Add 3000ms delay between tests to avoid 429 rate limiting
            System.Threading.Thread.Sleep(3000);
        }

        [TestMethod]
        public void toUrlWorks()
        {
            string url = new ImageCharts().cht("p").chd("t:1,2,3").toURL();
            Assert.AreEqual("https://image-charts.com:443/chart?cht=p&chd=t%3A1%2C2%2C3", url);
        }

        [TestMethod]
        public void toUrlAddSignature()
        {
            string url = new ImageCharts("plop").cht("p")
                    .chd("t:1,2,3")
                    .chs("100x100")
                    .icac("test_fixture")
                    .toURL();
            Assert.AreEqual("https://image-charts.com:443/chart?cht=p&chd=t%3A1%2C2%2C3&chs=100x100&icac=test_fixture&ichm=71bd93758b49ed28fdabd23a0ff366fe7bf877296ea888b9aaf4ede7978bdc8d", url);
        }

        [TestMethod]
        public void toUrlExposesParametersUseThem()
        {
            ImageCharts imageCharts = new ImageCharts();

            string query = string.Join("&", typeof(ImageCharts).GetMethods()
                 .Where(m => m.Name.StartsWith("c") || m.Name.StartsWith("id"))
                 .Select(m =>
                 {
                     m.Invoke(imageCharts, new Object[] { "plop" });
                     return m.Name + "=plop";
                 }));


            string url = imageCharts.toURL();
            string assertQuery = "https://image-charts.com:443/chart?" + query;

            Assert.AreEqual(assertQuery, url);
        }

        [TestMethod]
        [ExpectedException(typeof(ImageChartsException), "\"\\\"chs\\\" is required\"")]
        public void toBufferRejectsIfChsNotDefined()
        {
            CreateImageCharts().cht("p").chd("t:1,2,3").toBuffer();
        }

        [TestMethod]
        [ExpectedException(typeof(ImageChartsException), "IC_MISSING_ENT_PARAMETER")]
        public void toBufferRejectsIfIcacWithoutIchm()
        {

            CreateImageCharts().cht("p").chd("t:1,2,3").chs("100x100").icac("test_fixture").toBuffer();

        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void toBufferRejectsIfTimeoutReached()
        {

            new ImageCharts(null, null, null, null, null, 1)
                    .cht("p").chd("t:1,2,3").chs("100x100").toBuffer();
        }

        [TestMethod]
        public void toBufferWorks()
        {
            CreateImageCharts()
                    .cht("p").chd("t:1,2,3").chs("100x100").toBuffer();
        }

        [TestMethod]
        [ExpectedException(typeof(ImageChartsException), "\"\\\"chs\\\" is required\"")]
        public void toDataURIRejectsIfChsNotDefined()
        {

            CreateImageCharts().cht("p").chd("t:1,2,3").toDataURI();

        }

        [TestMethod]
        public void toDataURIWorks()
        {
            string dataURI = CreateImageCharts().cht("p").chd("t:1,2,3").chs("2x2").toDataURI();

            Assert.AreEqual("data:image/png;base64,iVBORw0K", dataURI.Substring(0, 30));
        }

        [TestMethod]
        public void toDataURISupportGifs()
        {
            string dataURI = CreateImageCharts().cht("p").chd("t:1,2,3").chan("100").chs("2x2").toDataURI();

            Assert.AreEqual("data:image/gif;base64,R0lGODlh", dataURI.Substring(0, 30));
        }

        [TestMethod]
        [ExpectedException(typeof(ImageChartsException), "\"\\\"chs\\\" is required\"")]
        public void toFileRejectsIfError()
        {

            CreateImageCharts().cht("p").chd("t:1,2,3").toFile("/tmp/chart.png");

        }

        [TestMethod]
        [ExpectedException(typeof(ImageChartsException), "\"\\\"chs\\\" is required\"")]
        public void toFileRejectsWhenInvalidPath()
        {

            CreateImageCharts().cht("p").chd("t:1,2,3").toFile("/__invalid_path/chart.png");

        }

        [TestMethod]
        public void toFileWorks()
        {
            string filePath = "/app/plop.png";
            CreateImageCharts().cht("p").chd("t:1,2,3").chan("100").chs("2x2").toFile(filePath);

            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod]
        public void protocolExposeProtocol()
        {
            Assert.AreEqual("https", typeof(ImageCharts).GetField("protocol", bindFlags).GetValue(defaultBuilder).ToString());
        }

        [TestMethod]
        public void protocolUserDefined()
        {
            ImageCharts builder = new ImageCharts("http", null, null, null, null, null);

            Assert.AreEqual("http", typeof(ImageCharts).GetField("protocol", bindFlags).GetValue(builder).ToString());
        }

        [TestMethod]
        public void hostExposeHost()
        {
            Assert.AreEqual("image-charts.com", typeof(ImageCharts).GetField("host", bindFlags).GetValue(defaultBuilder).ToString());
        }

        [TestMethod]
        public void hostUserDefined()
        {
            ImageCharts builder = new ImageCharts(null, "on-premise-image-charts.com", null, null, null, null);

            Assert.AreEqual("on-premise-image-charts.com", typeof(ImageCharts).GetField("host", bindFlags).GetValue(builder).ToString());
        }

        [TestMethod]
        public void pathnameExposePathname()
        {
            Assert.AreEqual("/chart", typeof(ImageCharts).GetField("pathname", bindFlags).GetValue(defaultBuilder).ToString());
        }

        [TestMethod]
        public void pathnameUserDefined()
        {
            ImageCharts builder = new ImageCharts(null, null, null, "/my-charts", null, null);

            Assert.AreEqual("/my-charts", typeof(ImageCharts).GetField("pathname", bindFlags).GetValue(builder).ToString());
        }

        [TestMethod]
        public void portExposePort()
        {
            Assert.AreEqual(443, typeof(ImageCharts).GetField("port", bindFlags).GetValue(defaultBuilder));
        }

        [TestMethod]
        public void portUserDefined()
        {
            ImageCharts builder = new ImageCharts(null, null, 8080, null, null, null);

            Assert.AreEqual(8080, typeof(ImageCharts).GetField("port", bindFlags).GetValue(builder));
        }

        [TestMethod]
        public void queryExposeQuery()
        {
            ImageCharts builder = new ImageCharts();

            FieldInfo protocolField = typeof(ImageCharts).GetField("query", bindFlags);

            Dictionary<string, Object> query = (Dictionary<string, Object>)protocolField.GetValue(builder);
            Assert.AreEqual(query.Count, 0);
        }

        [TestMethod]
        public void queryUserDefined()
        {
            ImageCharts builder = new ImageCharts().cht("p").chd("t:1,2,3").icac("plop");

            FieldInfo protocolField = typeof(ImageCharts).GetField("query", bindFlags);
            Dictionary<string, Object> query = (Dictionary<string, Object>)protocolField.GetValue(builder);

            Assert.AreEqual("p", query["cht"]);
            Assert.AreEqual("t:1,2,3", query["chd"]);
            Assert.AreEqual("plop", query["icac"]);
        }
    }
}
