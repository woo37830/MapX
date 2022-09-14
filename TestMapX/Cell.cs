// //File: Cells.cs
// //
// //  Copyright (c) 2018 ERC LLC
// //
// //Author: woo
// //
// // Since: 2022-9-7
// //
using System;
using System.IO;
using System.Collections.Generic;

namespace TestMapX
{
    public class Cell
    {
        private static int Number { get; set; }
        public int id_cell { get; }
        protected float x_min { get; set; }
        protected float x_max { get; set; }
        protected float y_min { get; set; }
        protected float y_max { get; set; }
        public List<Point> pointsInCell { get; }
        public Cell(float x, float y, float delta_x, float delta_y)
        {
            this.id_cell = Number;
            pointsInCell = new List<Point>();
            Number++;
        }
        public void add(Point p)
        {
            pointsInCell.Add(p);
        }

        public Point findClosestPointTo(Point p)
        {
            return null;
        }
        public override string ToString()
        {
            return "id: " + id_cell + ", x: " + x_min + "->" + x_max + ", y: " + y_min + "->" + y_max;
        }
    }
}