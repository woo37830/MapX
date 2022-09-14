// //File: Segment.cs
// //
// //  Copyright (c) 2018 ERC LLC
// //
// //Author: woo
// //
// // Since: 2022-9-5
// //
using System;
namespace TestMapX
{
    public class Segment
    {
        public int id_segment { get; set; }
        public Point point_leftUpper { get; set; }
        public Point point_rightLower { get; set; }
        public Block block { get; set; }
        private static int Number { get; set; }
        public float length { get; }
        public float angle { get; }

        public Segment(Point p1, Point p2)
        {
            Number++;
            this.id_segment = Number;
            this.point_leftUpper = p1;
            this.point_rightLower = p2;
            this.length = (float)Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
            this.angle = (float)Math.Atan2((double)(p2.x - p1.x), (double)(p1.y - p2.y));
        }

        public override string ToString()
        {
            {
                string blkString = (this.block == null) ? "Not in block" : this.block.ToString();
                return "Segment id:  " + this.id_segment + " (" + this.point_leftUpper.id_point + ", " + this.point_rightLower.id_point + ") - " + this.length + " " + this.angle + ", block: " + blkString;
            }
        }

    }
}
