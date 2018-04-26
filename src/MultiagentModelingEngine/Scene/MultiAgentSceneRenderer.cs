using SceneRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawWrapperLib;
using Common;
using System.Drawing;

namespace MultiagentModelingEngine.Scene
{
    public class MultiAgentSceneRenderer : ISceneRenderer
    {
        public List<Vector> SmokeCoversKoordinates { get; set; }
        public MultiAgentSceneRenderer(IDrawWrapper drawContex, MultiAgentSceneDrawingConfig config)
        {
            DrawContext = drawContex ?? throw new ArgumentNullException($"{drawContex}");
            Configuration = config ?? throw new ArgumentNullException($"{config}");
            SmokeCoversKoordinates = new List<Vector>();
        }

        public IDrawWrapper DrawContext { get; protected set; }
        public int ContextWidth => DrawContext.Width;
        public int ContextHeigth => DrawContext.Height;
        public MultiAgentSceneDrawingConfig Configuration { get; protected set; }

        public List<Vector> GetKoordinates()
        {
            return SmokeCoversKoordinates;
        }

        public void InitializeContext()
        {
            var halfHeight = DrawContext.Height / 2;
            var width = DrawContext.Width;

            DrawContext.SetBackgroundColor(Configuration.BackgroundColor);
 
            var pen = new Pen(Configuration.RoadBorderColor, Configuration.RoadBorderWidth);
            DrawContext.DrawLine(pen, 0, halfHeight, width, halfHeight);
            DrawRoadWayDelimiterLines();
            DrawSmokeCovers();
        }

        private void DrawSmokeCovers()
        {
            var deltaX = DrawContext.Width / Configuration.SmokeCoversNumber;
            var pen = new Pen(Configuration.SmokeCoverColor, Configuration.RoadwayDelimiterWidth * 4);            
            for (int i = 0; i < Configuration.SmokeCoversNumber; i++)
            {
                var x = deltaX * (i + 1) - Configuration.SmokeCoverWidth / 2 - deltaX / 2;
                DrawContext.DrawLine(pen, x, 0, x + Configuration.SmokeCoverWidth, 0);
                DrawContext.DrawLine(pen, x, DrawContext.Height, x + Configuration.SmokeCoverWidth, DrawContext.Height);
                SmokeCoversKoordinates.Add(new Vector(x + Configuration.SmokeCoverWidth / 2, 0));
                SmokeCoversKoordinates.Add(new Vector(x + Configuration.SmokeCoverWidth / 2, DrawContext.Height));
            }
        }

        private void DrawRoadWayDelimiterLines()
        {
            var lineWidth = DrawContext.Height / Configuration.RoadwaysNumber;
            int linesNumber = Configuration.RoadwaysNumber / 2;
            if (linesNumber % 2 != 0)
                linesNumber += 1;
            var pen = new Pen(Configuration.RoadwayDelimiterColor, Configuration.RoadwayDelimiterWidth);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            DrawRoadWayDelimiterLinesFirstHalf(pen, linesNumber, lineWidth);
            DrawRoadWayDelimiterLinesLastHalf(pen, linesNumber, lineWidth);
        }

        private void DrawRoadWayDelimiterLinesLastHalf(Pen pen, int linesNumber, int lineWidth)
        {
            var halfHeight = DrawContext.Height / 2;
            var width = DrawContext.Width;
            for (int line = 0; line < linesNumber / 2; line++)
            {
                var yKoordinate = lineWidth * (line + 1) + halfHeight;
                DrawContext.DrawLine(pen, 0, yKoordinate, width, yKoordinate);
            }
        }

        private void DrawRoadWayDelimiterLinesFirstHalf(Pen pen, int linesNumber, int lineWidth)
        {
            var width = DrawContext.Width;
            for (int line = 0; line < linesNumber / 2; line++)
            {
                var yKoordinate = lineWidth * (line + 1);
                DrawContext.DrawLine(pen, 0, yKoordinate, width, yKoordinate);
            }
        }

        private void DrawRoadwayDelimiters()
        {
            var linesNumber = Configuration.RoadwaysNumber / 2;
            
        }

        public void Render(IScene scene)
        {
            scene.Objects.ForEach(e =>
            {
                e.Render(DrawContext);
            });
        }
    }
}
