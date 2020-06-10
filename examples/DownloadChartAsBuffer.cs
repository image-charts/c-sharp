using ImageChartsLib;

public class DownloadChartAsBuffer {
    static void Main(string[] args)
    {
        byte[] buffer = new ImageCharts()
            .cht("bvg") // vertical bar chart
            .chs("300x300") // 300px x 300px
            .chd("a:60,40") // 2 data points: 60 and 40
            .toBuffer();

        Console.WriteLine(buffer); // System.Byte[]
    }
}
