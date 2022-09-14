// //File: Block.cs
// //
// //  Copyright (c) 2018 ERC LLC
// //
// //Author: woo
// //
// // Since: 2022-9-11
// //
using System;
using System.IO;
using System.Collections.Generic;

namespace TestMapX
{
    /*
     * A Block contains a connected set of segments which forms a closed loop
     * 
     */
    public class Block
    {
        public List<Segment> segments { get; }
        private static int Number { get; set; }
        public bool closed = false;
        public int id_block { get; }
        public Point startingPoint;
        public Point nextPoint { get; set; }

        public Block(Point p)
        {
            this.id_block = Number;
            segments = new List<Segment>();
            Number++;
            startingPoint = p;
        }

        public Point add(Point p_added, Segment s)
        {
            if (p_added.in_block != null)
            {
                return null;
            }
            segments.Add(s);  // The segment s with p_added in added to the block.
            p_added.in_block = this; // The point is marked as in a block
            Point p1 = s.point_leftUpper;
            Point p2 = s.point_rightLower;
            Point returnPoint = p2;
            // set up to return the other point in the segment
            if (p1.id_point != p_added.id_point)
            {
                returnPoint = p1;
            }
            if (returnPoint.id_point == startingPoint.id_point)
            {
                this.closed = true;
                return null;
            }
            returnPoint.in_block = this;
            this.nextPoint = returnPoint; // This shoud be the other end of the segment
            return returnPoint;
        }

        public Point getNextPoint()
        {
            return this.nextPoint;
        }

        public List<Segment> getSegments()
        {
            return segments;
        }

        public override string ToString()
        {
            string returnStr = "\n";
            string closedStr = closed ? "Closed" : "Open";
            returnStr += "\n\tid: " + id_block + "\n\t" + closedStr + "\n]t start: " + startingPoint.id_point + "\n\t";

            for (int i = 0; i < segments.Count; i++)
            {
                returnStr += segments[i].id_segment + "\n\t";
            }
            return returnStr;
        }

    }
}
