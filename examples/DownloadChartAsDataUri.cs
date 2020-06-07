using ImageChartsLib;

public class MainClass {
    static void Main(string[] args)
    {
        string chartUrl = new ImageCharts()
            .cht("bvg") // vertical bar chart
            .chs("300x300") // 300px x 300px
            .chd("a:60,40") // 2 data points: 60 and 40
            .toDataURI();

        Console.WriteLine(chartUrl); // "data:image/png;base64,iVBORw0KGgo...
    }
}


