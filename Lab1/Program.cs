// Генерація вхідних даних
List<Point> points = GeneratePoints(3);
SavePointsToFile(points, "points.txt");

// Зчитування вхідних даних з файлу
List<Point> loadedPoints = LoadPointsFromFile("points.txt");

// Знаходження пари найближчих точок з використанням багатопотоковості
var startTime = DateTime.Now;
var closestPair = FindClosestPairParallel(loadedPoints);
var endTime = DateTime.Now;

Console.WriteLine($"Closest pair: ({closestPair.Item1.X}, {closestPair.Item1.Y}) and ({closestPair.Item2.X}, {closestPair.Item2.Y})");
Console.WriteLine($"Distance: {Distance(closestPair.Item1, closestPair.Item2)}");
Console.WriteLine($"Time taken: {(endTime - startTime).TotalSeconds} seconds");

static List<Point> GeneratePoints(int count)
{
	Random random = new Random();
	List<Point> points = new List<Point>();
	for (int i = 0; i < count; i++)
	{
		points.Add(new Point(random.NextDouble() * 1000, random.NextDouble() * 1000));
	}
	return points;
}

static void SavePointsToFile(List<Point> points, string filename)
{
	using (StreamWriter writer = new StreamWriter(filename))
	{
		foreach (var point in points)
		{
			writer.WriteLine($"{point.X} {point.Y}");
		}
	}
}

static List<Point> LoadPointsFromFile(string filename)
{
	List<Point> points = new List<Point>();
	using (StreamReader reader = new StreamReader(filename))
	{
		string line;
		while ((line = reader.ReadLine()) != null)
		{
			string[] parts = line.Split(' ');
			points.Add(new Point(double.Parse(parts[0]), double.Parse(parts[1])));
		}
	}
	return points;
}

static Tuple<Point, Point> FindClosestPairParallel(List<Point> points)
{
	double minDistance = double.MaxValue;
	Point closestPoint1 = null;
	Point closestPoint2 = null;

	Parallel.For(0, points.Count, i =>
	{
		for (int j = i + 1; j < points.Count; j++)
		{
			double distance = Distance(points[i], points[j]);
			if (distance < minDistance)
			{
				lock (points)
				{
					if (distance < minDistance)
					{
						minDistance = distance;
						closestPoint1 = points[i];
						closestPoint2 = points[j];
					}
				}
			}
		}
	});

	return Tuple.Create(closestPoint1, closestPoint2);
}

static double Distance(Point p1, Point p2)
{
	return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
}

class Point
{
	public double X { get; }
	public double Y { get; }

	public Point(double x, double y)
	{
		X = x;
		Y = y;
	}
}
