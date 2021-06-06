using Common.Entities;
using Common.Enums;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;


namespace BusinessLogicLayer.PDFCreation
{
    /// <summary>
    /// SummaryReport defines the generation process of Tour Summary Reports as PDF
    /// </summary>
    class SummaryReport : IDocument
    {
        /// <value>
        /// Tours included in the summary report
        /// </value>
        public List<Tour> Tours { get; }
        /// <summary>
        /// Default constructor of SummaryReport
        /// </summary>
        /// <param name="tours">Tours that should be included in the summary report</param>
        public SummaryReport(List<Tour> tours)
        {
            Tours = tours;
        }
        /// <summary>
        /// Compose defines the base structure of SummaryReport
        /// </summary>
        /// <param name="container">Container in which the content will be placed</param>
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
        /// <summary>
        /// Document Metadata of SummaryReport
        /// </summary>
        /// <returns>Metadata of the PDF file</returns>
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        /// <summary>
        /// ComposedHeader defines the composition of SummaryReport headers
        /// </summary>
        /// <param name="container">Container in which the header will be placed</param>
        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeColumn().Stack(stack =>
                {
                    stack.Item().Text("Tour Summary Report", TextStyle.Default.Size(20).Bold());
                    stack.Item().Text($"Analyzed Tours: {Tours.Count}");
                });
            });
        }
        /// <summary>
        /// ComposeContent assembles ComposeTourStats, ComposeManeuverStats and ComposeTourLogStats into the main section of SummaryReport
        /// </summary>
        /// <param name="container">Container in which the main content will be placed</param>
        void ComposeContent(IContainer container)
        {
            var headingStyle = TextStyle.Default.Size(16).Bold();

            container.PaddingVertical(10).Stack(stack =>
            {
                if (Tours.Any())
                {
                    stack.Item().Text("Tour stats", headingStyle);
                    stack.Item().PaddingTop(5).Element(ComposeTourStats);
                    if (Tours.Any(t => t.Maneuvers.Any()))
                    {
                        stack.Item().Text("Maneuver stats", headingStyle);
                        stack.Item().PaddingTop(5).Element(ComposeManeuverStats);
                    }

                    if (Tours.Any(t => t.TourLogs.Any()))
                    {
                        stack.Item().Text("Tour log stats", headingStyle);
                        stack.Item().PaddingTop(5).Element(ComposeTourLogStats);
                    }
                }
                else
                    stack.Item().Text("Because there are no tours, a summary is not possible!");
               
                
            });
        }
        /// <summary>
        /// ComposeTourStats defines how stats about Tour entities will be presented in the report
        /// </summary>
        /// <param name="container">Container in which stats about Tour entities are placed</param>
        void ComposeTourStats(IContainer container)
        {

            var headingStyle = TextStyle.Default.Size(14).SemiBold();
            container.Stack(stack =>
            {
                if (Tours.Any(t => t.TourLogs.Any()))
                {
                    double bestRating = Tours.Where(t => t.TourLogs.Any()).GroupBy(t => t.TourLogs.Average(tl => tl.Rating)).Max(g => g.Key);
                    List<Tour> bestTours = Tours.Where(t => t.TourLogs.Any()).Where(t => t.TourLogs.Average(tl => tl.Rating) == bestRating).ToList();
                    double worstRating = Tours.Where(t => t.TourLogs.Any()).GroupBy(t => t.TourLogs.Average(tl => tl.Rating)).Min(g => g.Key);
                    List<Tour> worstTours = Tours.Where(t => t.TourLogs.Any()).Where(t => t.TourLogs.Average(tl => tl.Rating) == worstRating).ToList();
                    double longestTotalTime = Tours.Where(t => t.TourLogs.Any()).GroupBy(t => t.TourLogs.Sum(tl => tl.TotalTime)).Max(g => g.Key);
                    List<Tour> longestTotalTimeTours = Tours.Where(t => t.TourLogs.Any()).Where(t => t.TourLogs.Sum(tl => tl.TotalTime) == longestTotalTime).ToList();
                    double shortestTotalTime = Tours.Where(t => t.TourLogs.Any()).GroupBy(t => t.TourLogs.Sum(tl => tl.TotalTime)).Min(g => g.Key);
                    List<Tour> shortestTotalTimeTours = Tours.Where(t => t.TourLogs.Any()).Where(t => t.TourLogs.Sum(tl => tl.TotalTime) == shortestTotalTime).ToList();
                    int highestTourLogCount = Tours.Max(t => t.TourLogs.Count);
                    List<Tour> mostTourLogsTours = Tours.Where(t => t.TourLogs.Count == highestTourLogCount).ToList();
                    int lowestTourLogCount = Tours.Min(t => t.TourLogs.Count);
                    List<Tour> leastTourLogsTours = Tours.Where(t => t.TourLogs.Count == lowestTourLogCount).ToList();

                    stack.Item().Text($"Best rated tours - {bestRating}", headingStyle);
                    stack.Item().Text(string.Join('\n', bestTours.Select(t => t.Name)));
                    stack.Item().Text($"Worst rated tours - {worstRating}", headingStyle);
                    stack.Item().Text(string.Join('\n', worstTours.Select(t => t.Name)));
                    stack.Item().Text($"Longest tours (total time from tour logs) - {longestTotalTime} h", headingStyle);
                    stack.Item().Text(string.Join('\n', longestTotalTimeTours.Select(t => t.Name)));
                    stack.Item().Text($"Shortest tours (total time from tour logs) - {shortestTotalTime} h", headingStyle);
                    stack.Item().Text(string.Join('\n', shortestTotalTimeTours.Select(t => t.Name)));
                    stack.Item().Text($"Tours with most tour logs - {highestTourLogCount}", headingStyle);
                    stack.Item().Text(string.Join('\n', mostTourLogsTours.Select(t => t.Name)));
                    stack.Item().Text($"Tours with least amount of tour logs - {lowestTourLogCount}", headingStyle);
                    stack.Item().Text(string.Join('\n', leastTourLogsTours.Select(t => t.Name)));
                }

                double longestDistance = Tours.Max(t => t.Distance);
                List<Tour> longestDistanceTours = Tours.Where(t => t.Distance == longestDistance).ToList();
                double shortestDistance = Tours.Min(t => t.Distance);
                List<Tour> shortestDistanceTours = Tours.Where(t => t.Distance == shortestDistance).ToList();
                int highestManeuverCount = Tours.Max(t => t.Maneuvers.Count);
                List<Tour> mostManeuversTours = Tours.Where(t => t.Maneuvers.Count == highestManeuverCount).ToList();
                int lowestManeuverCount = Tours.Min(t => t.Maneuvers.Count);
                List<Tour> leastManeuversTours = Tours.Where(t => t.Maneuvers.Count == lowestManeuverCount).ToList();
                int highestRouteTypeCount = Tours.GroupBy(t => t.RouteType).Max(g => g.Count());
                List<ERouteType> mostCommonRouteTypes = Tours.GroupBy(t => t.RouteType).Where(g => g.Count() == highestRouteTypeCount).Select(g => g.Key).ToList();
                int lowestRouteTypeCount = Tours.GroupBy(t => t.RouteType).Min(g => g.Count());
                List<ERouteType> leastCommonRouteTypes = Tours.GroupBy(t => t.RouteType).Where(g => g.Count() == lowestRouteTypeCount).Select(g => g.Key).ToList();

                stack.Item().Text($"Longest tours (distance) - {longestDistance} km", headingStyle);
                stack.Item().Text(string.Join('\n', longestDistanceTours.Select(t => t.Name)));
                stack.Item().Text($"Shortest tours (distance) - {shortestDistance} km", headingStyle);
                stack.Item().Text(string.Join('\n', shortestDistanceTours.Select(t => t.Name)));
                stack.Item().Text($"Tours with most maneuvers - {highestManeuverCount}", headingStyle);
                stack.Item().Text(string.Join('\n', mostManeuversTours.Select(t => t.Name)));
                stack.Item().Text($"Tours with least amount of maneuvers - {lowestManeuverCount}", headingStyle);
                stack.Item().Text(string.Join('\n', leastManeuversTours.Select(t => t.Name)));
                stack.Item().Text($"Most common route type - {highestRouteTypeCount}", headingStyle);
                stack.Item().Text(string.Join('\n', mostCommonRouteTypes));
                stack.Item().Text($"Least common route type - {lowestRouteTypeCount}", headingStyle);
                stack.Item().Text(string.Join('\n', leastCommonRouteTypes));
            });
        }
        /// <summary>
        /// ComposerManeuverStats defines the way how stats about Maneuver entities will be presented
        /// </summary>
        /// <param name="container">Container in which stats about Maneuver entities will be placed</param>
        void ComposeManeuverStats(IContainer container)
        {
            int overallManeuverCount = Tours.Sum(t => t.Maneuvers.Count);
            double longestManeuverDistance = Tours.SelectMany(t => t.Maneuvers).Max(m=>m.Distance);
            List<Maneuver> longestManeuvers = Tours.SelectMany(t => t.Maneuvers).Where(m => m.Distance == longestManeuverDistance).ToList();
            double shortestManeuverDistance = Tours.SelectMany(t => t.Maneuvers).Min(m=>m.Distance);
            List<Maneuver> shortestManeuvers = Tours.SelectMany(t => t.Maneuvers).Where(m => m.Distance == shortestManeuverDistance).ToList();

            var headingStyle = TextStyle.Default.Size(14).SemiBold();

            container.Stack(stack =>
            {
                stack.Item().Text($"Maneuver count - {overallManeuverCount}", headingStyle);
                stack.Item().Text($"Longest maneuvers - {longestManeuverDistance} km", headingStyle);
                stack.Item().Text(string.Join('\n', longestManeuvers.Select(t => t.Narrative)));
                stack.Item().Text($"Shortest maneuvers - {shortestManeuverDistance} km", headingStyle);
                stack.Item().Text(string.Join('\n', shortestManeuvers.Select(t => t.Narrative)));
            });
        }
        /// <summary>
        /// ComposeTourLogStats defines the way stats about TourLog entities of the passed Tours will look like
        /// </summary>
        /// <param name="container">Container in which stats about TourLog entities will be placed</param>
        void ComposeTourLogStats(IContainer container)
        {
            int overallTourLogCount = Tours.Sum(t => t.TourLogs.Count);
            double longestTotalTime = Tours.SelectMany(t => t.TourLogs).Max(tl => tl.TotalTime);
            double shortestTotalTime = Tours.SelectMany(t => t.TourLogs).Min(tl => tl.TotalTime);
            double longestTourLogDistance = Tours.SelectMany(t => t.TourLogs).Max(tl => tl.Distance);
            double shortestTourLogDistance = Tours.SelectMany(t => t.TourLogs).Min(tl => tl.Distance);
            double highestRating = Tours.SelectMany(t => t.TourLogs).Max(tl => tl.Rating);
            double lowestRating = Tours.SelectMany(t => t.TourLogs).Min(tl => tl.Rating);
            double averageSpeed = Tours.SelectMany(t => t.TourLogs).Average(tl => tl.AverageSpeed);
            List<IGrouping<EWeather,TourLog>> weatherGroups = Tours.SelectMany(t => t.TourLogs).GroupBy(tl => tl.Weather).ToList();
            List<IGrouping<ETravelMethod,TourLog>> travelMethodGroups = Tours.SelectMany(t => t.TourLogs).GroupBy(tl => tl.TravelMethod).ToList();

            var headingStyle = TextStyle.Default.Size(14).SemiBold();

            container.Stack(stack =>
            {
                stack.Item().Text($"Tour log count - {overallTourLogCount}");
                stack.Item().Text($"Time range: {shortestTourLogDistance} - {longestTotalTime} h");
                stack.Item().Text($"Distance range: {shortestTourLogDistance} - {longestTourLogDistance} km");
                stack.Item().Text($"Rating range: {lowestRating} - {highestRating}");
                stack.Item().Text($"Average speed - {averageSpeed} km/h");
                stack.Item().Text($"Weather stats", headingStyle);
                stack.Item().Text(string.Join('\n', weatherGroups.Select(g => $"{g.Key} - {g.Count()}")));
                stack.Item().Text($"Travel methods stats", headingStyle);
                stack.Item().Text(string.Join('\n', travelMethodGroups.Select(g => $"{g.Key} - {g.Count()}")));
            });
        }
    }
}
