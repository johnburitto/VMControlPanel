using Core.Dtos;
using ScottPlot;

namespace Infrastructure.Services.Impls
{
    public static class GraphsService
    {
        public static string CreateGraph(GraphType type, GraphDto dto)
        {
            switch (type)
            {
                case GraphType.StackedBar:
                    {
                        var plot = CreateStackedBarPlot(dto);

                        return SavedGraphToLocal(plot, dto);
                    }
                case GraphType.Pie:
                    {
                        var plot = CreatePieGraph(dto);

                        return SavedGraphToLocal(plot, dto);
                    }
                case GraphType.Donut:
                    {
                        var plot = CreateDonutGraph(dto);

                        return SavedGraphToLocal(plot, dto);
                    }
                default: return string.Empty;
            }
        }

        public static string SavedGraphToLocal(Plot plot, GraphDto dto)
        {
            if (!Directory.Exists($"{FileManager.FileDirectory}/{dto.UserId}"))
            {
                Directory.CreateDirectory($"{FileManager.FileDirectory}/{dto.UserId}");
            }

            var path = $"{FileManager.FileDirectory}/{dto.UserId}/{dto.Name}.png";

            plot.SavePng(path, dto.Width, dto.Height);

            return path;
        }

        public static void DeleteGraphFromLocal(string path)
        {
            FileManager.DeleteFile(path);
        }

        private static Plot CreateDonutGraph(GraphDto dto)
        {
            var plot = new Plot();
            var pies = new List<PieSlice>();
            var random = new Random();

            for (var i = 0; i < Math.Min(dto.Labels!.Count, dto.Values!.Count); i++)
            {
                pies.Add(new()
                {
                    Value = dto.Values[i],
                    FillColor = Color.FromARGB((uint)(random.NextDouble() * uint.MaxValue)),
                    Label = $"{dto.Labels[i]} - {dto.Values[i]}"
                });
            }

            plot.Add.Pie(pies).DonutFraction = 0.5;
            plot.ShowLegend();
            plot.HideGrid();

            return plot;
        }

        private static Plot CreatePieGraph(GraphDto dto)
        {
            var plot = new Plot();
            var pies = new List<PieSlice>();
            var random = new Random();

            for (var i = 0; i < Math.Min(dto.Labels!.Count, dto.Values!.Count); i++)
            {
                pies.Add(new()
                {
                    Value = dto.Values[i],
                    FillColor = Color.FromARGB((uint)(random.NextDouble() * uint.MaxValue)),
                    Label = $"{dto.Labels[i]} - {dto.Values[i]}"
                });
            }

            plot.Add.Pie(pies);
            plot.ShowLegend();
            plot.HideGrid();

            return plot;
        }

        private static Plot CreateStackedBarPlot(GraphDto dto)
        {
            var plot = new Plot();

            for (var i = 0; i < Math.Min(dto.Labels!.Count, dto.Values!.Count); i++)
            {
                plot.Add.Bars([i + 1], [dto.Values[i]]).LegendText = $"{dto.Labels[i]} - {dto.Values[i]}";
            }

            plot.Axes.Margins(bottom: 0);
            plot.ShowLegend(Alignment.UpperLeft);
            plot.HideGrid();

            return plot;
        }
    }

    public enum GraphType
    {
        StackedBar,
        Pie,
        Donut
    }
}
