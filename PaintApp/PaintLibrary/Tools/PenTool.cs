﻿using PaintLibrary.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintLibrary.Tools
{
    /// <summary>
    /// Tool to drawing by Pen (100 hardness, straight lines)
    /// </summary>
    [Serializable]
    public class PenTool : Tool, IMouseHandler
    {
        /// <summary>
        /// Style of drawing
        /// </summary>
        public PenStyle PenStyle { get; set; } = new PenStyle();

        //key points of polyline
        private List<PointF> points = new List<PointF>();

        internal override void Render(Graphics gr, Document doc)
        {
            //render Layer
            base.Render(gr, doc);

            //render line
            RenderLine(gr);
        }

        private void RenderLine(Graphics gr)
        {
            //draw line over Leyer
            if (points.Count > 1)
            using (var pen = new Pen(PenStyle.Color, PenStyle.Width))
                gr.DrawLines(pen, points.ToArray());
        }

        internal override void Apply(Document doc)
        {
            //create undo point
            // -- doc.CreateUndoPoint("Pen tool");
            doc.OnStartOperation("Pen tool");

            //draw line on Layer 
            using (var gr = Graphics.FromImage(doc.Layer))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                RenderLine(gr);
            }

            //clear points
            points.Clear();
            doc.OnFinishOperation();
        }

        #region IMouseHandler

        public void OnMouseDown(Document doc, Point point)
        {
            //clear points
            points.Clear();

            //add first point
            points.Add(point);
        }

        public void OnMouseMove(Document doc, Point point)
        {
            //add next point
            points.Add(point);
        }

        public void OnMouseUp(Document doc, Point point)
        {
            //flush previous lines
            if (points.Count > 1)
                Apply(doc);
        }

        #endregion
    }
}
