using ImageChartsLib;

public class DownloadChartAsDataUri {
    static void Main(string[] args)
    {
        string dataUri = new ImageCharts()
            .cht("bvg") // vertical bar chart
            .chs("300x300") // 300px x 300px
            .chd("a:60,40") // 2 data points: 60 and 40
            .toDataURI();

        Console.WriteLine(dataUri); // "data:image/png;base64,iVBORw0KGgo...
    }
}


