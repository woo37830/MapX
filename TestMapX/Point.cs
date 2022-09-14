// //File: Point.cs
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
    public class Point
    {
        public float x { get; }
        public float y { get; }
        public Block in_block { get; set; }
        public Segment segment { get; set; }
        public int x_index { get; set; }
        public int y_index { get; set; }
        private static int Number { get; set; }
        public int id_point { get; }

        public Point(float _x, float _y)
        {
            this.x = _x;
            this.y = _y;
            Number++;
            this.id_point = Number;
        }

        public override string ToString()
        {

            int block_id = -1;
            if (in_block != null)
            {
                block_id = in_block.id_block;
            }
            return "Point id:  " + id_point + " (" + x + ", " + y + ") - segment: " + segment.id_segment + " cell[" + x_index + "," + y_index + "]" + " block: " + block_id;
        }
    }

}