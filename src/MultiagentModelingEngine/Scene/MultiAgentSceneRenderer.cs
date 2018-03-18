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
        public MultiAgentSceneRenderer(IDrawWrapper drawContex, MultiAgentSceneDrawingConfig config)
        {
            DrawContext = drawContex ?? throw new ArgumentNullException($"{drawContex}");
            Configuration = config ?? throw new ArgumentNullException($"{config}");            
        }

        public IDrawWrapper DrawContext { get; protected set; }
        public int ContextWidth => DrawContext.Width;
        public int ContextHeigth => DrawContext.Height;
        public MultiAgentSceneDrawingConfig Configuration { get; protected set; }

        public void InitializeContext()
        {
            var halfHeight = DrawContext.Height / 2;
            var width = DrawContext.Width;

            DrawContext.SetBackgroundColor(Configuration.BackgroundColor);
 
            var pen = new Pen(Configuration.RoadBorderColor, Configuration.RoadBorderWidth);
            DrawContext.DrawLine(pen, 0, halfHeight, width, halfHeight);
            DrawRoadWayDelimiterLines();
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
