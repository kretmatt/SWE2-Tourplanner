using Common.Entities;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.IO;

namespace BusinessLogicLayer.PDFCreation
{
    public class TourReport : IDocument
    {
        public Tour Tour { get; }

        public TourReport(Tour tour)
        {
            Tour = tour;
        }
        public void Compose(IContainer container)
        {
            container
                .PaddingHorizontal(50)
                .PaddingVertical(50)
                .Page(page =>
                {
                    page.Header().BorderBottom(0.5f).Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().PageNumber("Page {number}");

                });
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    
        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeColumn().Stack(stack =>
                {
                    stack.Item().Text($"{Tour.Name}-Tour",TextStyle.Default.Size(20).Bold());
                    stack.Item().Text($"From: {Tour.StartLocation}");
                    stack.Item().Text($"To: {Tour.EndLocation}");
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            var headingStyle = TextStyle.Default.Size(16).Bold();

            container.PaddingVertical(10).Stack(stack =>
            {
                stack.Item().Text("Route", headingStyle);
                stack.Item().PaddingTop(5).Image(File.ReadAllBytes(Tour.RouteInfo), ImageScaling.FitArea);
                stack.Item().Text("Details", headingStyle);
                stack.Item().PaddingTop(5).Element(ComposeDetails);
                if (Tour.Maneuvers.Count > 0)
                {
                    stack.Item().Text("Maneuvers", headingStyle);
                    stack.Item().PaddingTop(5).Element(ComposeManeuvers);
                }
                if (Tour.TourLogs.Count >= 0)
                {
                    stack.Item().Text("Tour Logs", headingStyle);
                    stack.Item().PaddingTop(5).Element(ComposeTourLogs);
                    stack.Item().Text("Tour Logs reports", headingStyle);
                    stack.Item().PaddingTop(5).Element(ComposeReports);
                }
            });
        }
        void ComposeManeuvers(IContainer container)
        {
            int stepCount = 1;
            container.Stack(stack =>
            {
                foreach (Maneuver m in Tour.Maneuvers)
                {
                    stack.Item().Text($"Step {stepCount} - {m.Distance} km", TextStyle.Default.SemiBold());
                    stack.Item().Text(m.Narrative);
                    stepCount++;
                }
            });
        }

        void ComposeTourLogs(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold();

            container.Decoration(decoration =>
            {
                decoration.Header().BorderBottom(1).Padding(5).Row(row =>
                {
                    row.RelativeColumn().Text("Start datetime", headerStyle);
                    row.RelativeColumn().Text("End datetime", headerStyle);
                    row.RelativeColumn().Text("Distance", headerStyle);
                    row.RelativeColumn().Text("Totaltime", headerStyle);
                    row.RelativeColumn().Text("Temperature", headerStyle);
                    row.RelativeColumn().Text("Rating", headerStyle);
                    row.RelativeColumn().Text("Average speed", headerStyle);
                    row.RelativeColumn().Text("Weather", headerStyle);
                    row.RelativeColumn().Text("Travel method", headerStyle);
                });

                decoration.Content().Stack(stack =>
                {
                    var contentStyle = TextStyle.Default.Size(9);
                    foreach(TourLog tl in Tour.TourLogs)
                    {
                        stack.Item().BorderBottom(1).BorderColor("CCC").Padding(5).Row(row =>
                        {
                            row.RelativeColumn().Text(tl.StartDate,contentStyle);
                            row.RelativeColumn().Text(tl.EndDate, contentStyle);
                            row.RelativeColumn().Text($"{tl.Distance} km", contentStyle);
                            row.RelativeColumn().Text($"{tl.TotalTime} h", contentStyle);
                            row.RelativeColumn().Text($"{tl.Temperature} °C", contentStyle);
                            row.RelativeColumn().Text(tl.Rating, contentStyle);
                            row.RelativeColumn().Text($"{tl.AverageSpeed} km/h", contentStyle);
                            row.RelativeColumn().Text(tl.Weather, contentStyle);
                            row.RelativeColumn().Text(tl.TravelMethod, contentStyle);
                        });
                    }
                });

            });
        }

        void ComposeDetails(IContainer container)
        {
            container.Stack(stack => {
                stack.Item().Text($"Distance - {Tour.Distance} km", TextStyle.Default.SemiBold());
                stack.Item().Text($"Route type - {Tour.RouteType}", TextStyle.Default.SemiBold());
                stack.Item().Text($"Description:", TextStyle.Default.SemiBold());
                stack.Item().Text(Tour.Description);
            });
        }

        void ComposeReports(IContainer container)
        {
            container.Stack(stack =>
            {
                foreach(TourLog tl in Tour.TourLogs)
                {
                    stack.Item().PaddingTop(2).Text(tl.Report);
                }
            });
        }
    }
}
