using System;
using System.IO;
using System.Collections.Generic;

namespace TestMapX
{
    class Program
    {

        static string solution = "TestMapX";
        static void Main(string[] args)
        {
            var watch = new System.Diagnostics.Stopwatch();
            try
            {
                watch.Start();

                ConfigurationBuilder bldr = new ConfigurationBuilder(@"/Users/woo/Development/workspaces/TestMapX/TestMapX/myconfig.cfg");
                Configuration config = bldr.getConfiguration();
                Logger.config = config;
                Logger logger = Logger.Instance;
                int fileLogLevel = int.Parse(config.get("FileLogLevel"));
                int consoleLogLevel = int.Parse(config.get("ConsoleLogLevel"));
                FileLogger fileLogger = new FileLogger(@"/Users/woo/tmp/logs/MapX/");
                fileLogger.Init();
                logger.RegisterObserver(fileLogger);
                ConsoleLogger console = new ConsoleLogger();
                console.Init();
                logger.RegisterObserver(console);
                logger.Write(1, $"Ralph = {config.get("Ralph")}");
                string dataSource = config.get("dataSource");
                string dataUser = config.get("dataUser");
                string dataPassword = config.get("dataPassword");
                string connString = "Data Source=" + dataSource + "; user id=" + dataUser + "; password=" + dataPassword + ";";
                // Store codeStatus into config
                CurrentCodeRevision sc = new CurrentCodeRevision();
                string status = sc.getCodeStatus();

                config.add("CodeStatus", status);
                String file = "/Users/woo/Development/workspaces/TestMapX/TestMapX/segments.txt";
                StreamReader dataStream = new StreamReader(file);
                String text;
                Dictionary<int, TestMapX.Point> points = new Dictionary<int, TestMapX.Point>();
                Dictionary<int, Segment> segments = new Dictionary<int, Segment>();
                float y_min = 9999999.0f, x_min = 9999999.0f, y_max = -9999999.0f, x_max = -9999999.0f;
                float fudge = float.Parse(config.get("nearestNeighborFudge"));
                int x_zones = int.Parse(config.get("xZones"));
                int y_zones = int.Parse(config.get("yZones"));
                float delta_x, delta_y;
                int[,] cells; // Will contain index into dictionary of neighbors in this cell
                Dictionary<int, List<Point>> neighbors = new Dictionary<int, List<Point>>();
                Dictionary<int, Block> blocks = new Dictionary<int, Block>();

                static int x_region(float x, float x_min, float delta_x)
                {
                    return (int)((x - x_min) / delta_x); // how many delta_x's is it from the min value
                }
                static int y_region(float y, float y_min, float delta_y)
                {
                    return (int)((y - y_min) / delta_y);
                }
                static float distance(float x1, float y1, float x2, float y2)
                {
                    return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
                }
                static float pointDistance(Point p1, Point p2)
                {
                    return distance(p1.x, p1.y, p2.x, p2.y);
                }
                Point findClosestPointTo(Point nextPoint, int cell_id)
                {
                    List<Point> cellNeighbors = neighbors[cell_id];
                    // loop through cellNeighbors list and look at points that are not nextPoint.
                    // calculate distance between nextPoint and those points and return the closest.
                    Point closest = null;
                    float minDist = 99999999.0f;
                    for (int i = 0; i < cellNeighbors.Count; i++)
                    {
                        Point p = cellNeighbors[i];
                        if (p.id_point != nextPoint.id_point)
                        {
                            float dist = pointDistance(p, nextPoint);
                            if (dist < minDist)
                            {
                                minDist = dist;
                                closest = p;
                            }
                        }
                    }
                    return closest;
                }
                while ((text = dataStream.ReadLine()) != null)
                {
                    if (!text.StartsWith('#'))
                    {
                        string[] bits = text.Split(' ');
                        float x1 = float.Parse(bits[0]);
                        float y1 = float.Parse(bits[1]);
                        float x2 = float.Parse(bits[2]);
                        float y2 = float.Parse(bits[3]);
                        logger.Write(3, String.Format("{0:F}, {1:F}, {2:F}, {3:F}", x1, y1, x2, y2));
                        Point p1 = new Point(x1, y1);
                        Point p2 = new Point(x2, y2);
                        points.Add(p1.id_point, p1);
                        points.Add(p2.id_point, p2);
                        Segment segment = new Segment(p1, p2);
                        segments.Add(segment.id_segment, segment);
                        p1.segment = segment;
                        p2.segment = segment;

                        if (p1.y > y_max)
                        {
                            y_max = p1.y;
                        }
                        if (p1.y < y_min)
                        {
                            y_min = p1.y;
                        }
                        if (p2.y > y_max)
                        {
                            y_max = p2.y;
                        }
                        if (p2.y < y_min)
                        {
                            y_min = p2.y;
                        }

                        if (p1.x > x_max)
                        {
                            x_max = p1.x;
                        }
                        if (p1.x < x_min)
                        {
                            x_min = p1.x;
                        }
                        if (p2.x > x_max)
                        {
                            x_max = p2.x;
                        }
                        if (p2.x < x_min)
                        {
                            x_min = p2.x;
                        }

                    }
                    else
                    {
                        string formattedStr = String.Format("{0}", text);
                        logger.Write(1, formattedStr);
                    }
                }
                List<int> point_ids = new List<int>(points.Keys);

                logger.Write(1, String.Format("Domain bounds are ({0}, {1}) by ({2}, {3})", x_min, y_max, x_max, y_min));

                delta_x = (float)((x_max + fudge - (x_min - fudge)) / x_zones);
                delta_y = (float)((y_max + fudge - (y_min - fudge)) / y_zones);
                fudge = (float)(Math.Max(delta_x, delta_y) / 10.0);
                delta_x = (float)((x_max + fudge - (x_min - fudge)) / x_zones);
                delta_y = (float)((y_max + fudge - (y_min - fudge)) / y_zones);
                logger.Write(1, String.Format("Region divided into {0} x {1} cells", x_zones, y_zones));
                logger.Write(1, String.Format("delta_x, delta_y are: ({0}, {1})", delta_x, delta_y));
                cells = new int[x_zones, y_zones];
                // Initialize cells to -1 as empty
                for (int i = 0; i < x_zones; i++)
                {
                    for (int j = 0; j < y_zones; j++)
                    {
                        cells[i, j] = -1;
                    }
                }
                int next_cell_id = 0;

                foreach (int id in point_ids)
                {
                    Point p = (points[id]);
                    p.x_index = x_region(p.x, x_min, delta_x);
                    p.y_index = y_region(p.y, y_min, delta_y);
                    logger.Write(3, p.ToString());
                    int cell_id = cells[p.x_index, p.y_index];

                    if (cell_id == -1)  // If no index of a list of neighbors, then
                    {
                        cell_id = next_cell_id;
                        List<Point> pointsInCell = new List<Point>();
                        neighbors[cell_id] = pointsInCell;
                        cells[p.x_index, p.y_index] = next_cell_id; // store index of list in cells
                        next_cell_id++;
                    }
                    // We have a cell_id, so we get the list from the dictionary of neightbors and add to it.
                    List<Point> cell_neighbors = neighbors[cell_id];
                    cell_neighbors.Add(p);
                }
                foreach (int id in point_ids)
                {
                    logger.Write(3, "" + points[id]);
                }

                logger.Write(3, "\n--------- cells with their neighborhood ids-----\n");
                for (int i = 0; i < neighbors.Count; i++)
                {
                    List<Point> _cells = neighbors[i];
                    logger.Write(3, String.Format("\ncell_id : {0}) ", i));
                    for (int j = 0; j < _cells.Count; j++)
                        logger.Write(3, String.Format(" {0}, ", _cells[j]));
                    logger.Write(3, "\n");
                }

                logger.Write(2, "\n-------- neighborhoods appear reasonable, now create blocks -----\n");
                int pointCount = point_ids.Count;
                int k = 1;
                while (k < pointCount)
                {
                    Point p = (points[k++]);
                    if (p.in_block == null)
                    { // Let's add it to a block
                        Block block = new Block(p); // Block sets startingPoint when p is added.
                        blocks[block.id_block] = block;
                        Point nextPoint = block.add(p, p.segment); // first point of segment added
                        logger.Write(2, String.Format("Point {0} added to block {1}", p.id_point, block.id_block));
                        while (nextPoint != null)
                        {
                            Point secondPoint = block.add(nextPoint, nextPoint.segment); // adds other end
                            logger.Write(2, String.Format("Point {0} added to block {1}", nextPoint.id_point, block.id_block));
                            nextPoint.segment.block = block;
                            if (secondPoint == null) // end of segment
                            {
                                int cell_id = cells[nextPoint.x_index, nextPoint.y_index]; // what cell it's in

                                Point firstPoint = findClosestPointTo(nextPoint, cell_id);
                                logger.Write(2, String.Format("Found {0} near {1}", firstPoint.id_point, nextPoint.id_point));
                                if (firstPoint.id_point == nextPoint.id_point)
                                {
                                    logger.Write(0, String.Format("findClosestPoint failed for {0} in {1}", nextPoint.id_point, cell_id));
                                    break;
                                }
                                if (firstPoint.id_point == block.startingPoint.id_point)
                                {
                                    block.closed = true;
                                    logger.Write(2, String.Format("Closed block {0}", block.id_block));
                                    break;
                                }
                                nextPoint = block.add(firstPoint, firstPoint.segment);
                                if (nextPoint != null)
                                    logger.Write(2, String.Format("Point {0} added to block {1}", firstPoint.id_point, block.id_block));
                            }
                        } // End while
                        logger.Write(3, String.Format("Block {0} has {1}", block.id_block, block));
                    } // End if
                } // End loop for new point
                logger.Write(2, "\n------------ Points and their cell indices -----------\n");
                foreach (int id in point_ids)
                {
                    Point p = (points[id]);
                    logger.Write(3, p.ToString());
                }
                logger.Write(1, $"There were {blocks.Count} blocks identified");
                logger.Write(2, "\n-------------Block Segment points-----------------------------------\n");
                foreach (int key in blocks.Keys)
                {
                    Block aBlock = blocks[key];
                    List<Segment> segmentsInBlock = aBlock.getSegments();
                    for (int i = 0; i < segmentsInBlock.Count; i++)
                    {
                        Segment seg = segmentsInBlock[i];
                        float x1 = seg.point_leftUpper.x;
                        float y1 = seg.point_leftUpper.y;
                        float x2 = seg.point_rightLower.x;
                        float y2 = seg.point_rightLower.y;
                        logger.Write(3, String.Format("{0} {1} {2} {3}", x1, y1, x2, y2));
                    }
                }

                watch.Stop();
                config.add("ElapsedTime", $"{watch.ElapsedMilliseconds} ms");
                config.addComment();
                config.summarize();

                if (config.get("INTERACTIVE") != "NO")
                {
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                {
                    Logger.Instance.Write(0, $"(ex) - Error in RoadProcessor: ");
                    Logger.Instance.Terminate("RoadProcessor exiting after Exception.");
                }

            }

            //           MapXLogger.Instance.Write(0, "During testing enter key to close terminal results window");

            Logger.Instance.Terminate($"{solution} exited successfully.");
            //           Console.ReadKey();
        } // End of main
    } // End of class
}