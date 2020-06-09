using ImageChartsLib;

public class MainClass {
    static void Main(string[] args)
    {
        string chartUrl = new ImageCharts()
            .cht("bvg") // vertical bar chart
            .chs("300x300") // 300px x 300px
            .chd("a:60,40") // 2 data points: 60 and 40
            .toURL(); // get the generated URL

        Console.WriteLine(chartUrl); // https://image-charts.com/chart?cht=bvg&chs=300x300&chd=a%3A60%2C40
    }
}
