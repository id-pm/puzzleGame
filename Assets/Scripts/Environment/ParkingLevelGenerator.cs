using System;
using System.Collections.Generic;

public static class ParkingLevelGenerator
{
    [Serializable]
    public struct CarPlacement
    {
        public int X;
        public int Y;
        public int Rotate;
        public int Size;

        public CarPlacement(int x, int y, int rotate, int size)
        {
            X = x;
            Y = y;
            Rotate = rotate;
            Size = size;
        }
    }

    private struct Point
    {
        public int Row;
        public int Col;
        public Point(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public override bool Equals(object obj)
        {
            if (obj is Point p)
                return Row == p.Row && Col == p.Col;
            return false;
        }
        public override int GetHashCode() => (Row, Col).GetHashCode();
        public static Point operator +(Point a, Point b) => new Point(a.Row + b.Row, a.Col + b.Col);
    }

    private static Point GetDirectionVector(int rotate)
    {
        switch (rotate % 4)
        {
            case 0: return new Point(-1, 0);
            case 1: return new Point(0, 1); 
            case 2: return new Point(1, 0); 
            case 3: return new Point(0, -1);
            default: return new Point(0, 0);
        }
    }

    private static List<Point> GetOccupiedCells(CarPlacement placement)
    {
        List<Point> cells = new List<Point>();
        Point dir = GetDirectionVector(placement.Rotate);
        for (int i = 0; i < placement.Size; i++)
            cells.Add(new Point(placement.Y - dir.Row * i, placement.X - dir.Col * i));
        return cells;
    }

    public static string GetOccupiedCells(int x, int y, int rotate, int size)
    {
        List<Point> cells = new List<Point>();
        Point dir = GetDirectionVector(rotate);
        for (int i = 0; i < size; i++)
            cells.Add(new Point(y - dir.Row * i, x - dir.Col * i));
        string result = "";
        foreach (var cell in cells)
        {
            result += $"({cell.Row}, {cell.Col}) ";
        }
        return result;
    }
    private static Point GetFrontCell(CarPlacement placement)
    {
        return new Point(placement.Y, placement.X);
    }
    private static bool FacesEachOther(CarPlacement a, CarPlacement b)
    {
        Point aDir = GetDirectionVector(a.Rotate);
        Point bDir = GetDirectionVector(b.Rotate);

        if (aDir.Row != -bDir.Row || aDir.Col != -bDir.Col)
            return false;

        if (a.Rotate % 2 == 0 && b.Rotate % 2 == 0)
        {
            if (a.X == b.X)
            {
                if (a.Y < b.Y)
                {
                    if (a.Rotate == 2 && b.Rotate == 0)
                        return true;
                }
                else if (a.Y > b.Y)
                {
                    if (a.Rotate == 0 && b.Rotate == 2)
                        return true;
                }
            }
        }
        else if (a.Rotate % 2 != 0 && b.Rotate % 2 != 0)
        {
            if (a.Y == b.Y)
            {
                if (a.X < b.X)
                {
                    if (a.Rotate == 1 && b.Rotate == 3)
                        return true;
                }
                else if (a.X > b.X)
                {
                    if (a.Rotate == 3 && b.Rotate == 1)
                        return true;
                }
            }
        }
        return false;
    }

    private static bool FacesEachOther3(CarPlacement a, CarPlacement b)
    {
        Point aDir = GetDirectionVector(a.Rotate);
        Point bDir = GetDirectionVector(b.Rotate);

        if (aDir.Row != -bDir.Row || aDir.Col != -bDir.Col)
            return false;

        if (a.Rotate % 2 == 0 && b.Rotate % 2 == 0)
        {
            if(a.Y == b.Y)
            {
                if(a.X < b.X)
                {
                    if(a.Rotate == 2)
                    {
                        return b.Rotate == 0;
                    }
                }
                else
                {
                    if(a.Rotate == 0)
                    {
                        return b.Rotate == 2;
                    }
                }
            }
        }
        else if(a.Rotate % 2 != 0 && b.Rotate % 2 != 0)
        {
            if(a.X == b.X)
            {
                if(a.Y < b.Y)
                {
                    if(a.Rotate == 1)
                    {
                        return b.Rotate == 3;
                    }
                }
                else
                {
                    if(a.Rotate == 3)
                    {
                        return b.Rotate == 1;
                    }
                }
            }
        }
        return false;
    }


    private static bool CheckCollision(CarPlacement placement, List<CarPlacement> placements, int boardRows, int boardCols)
    {
        foreach (Point cell in GetOccupiedCells(placement))
        {
            if (cell.Row < 0 || cell.Row >= boardRows || cell.Col < 0 || cell.Col >= boardCols)
                return true;
        }
        foreach (var other in placements)
        {
            foreach (var cell in GetOccupiedCells(placement))
            {
                foreach (var otherCell in GetOccupiedCells(other))
                {
                    if (cell.Equals(otherCell))
                        return true;
                }
            }
        }
        return false;
    }

    public static List<CarPlacement> GenerateRandomLevel(int vehicleCount, int boardRows, int boardCols)
    {
        int maxAttemptsPerVehicle = 100;
        int attempts = 0;
        List<CarPlacement> placements = new List<CarPlacement>();
        Random random = new Random();

        while (placements.Count < vehicleCount)
        {
            if (attempts > vehicleCount * maxAttemptsPerVehicle)
                break;

            int rotate = random.Next(0, 4);
            int size = random.Next(0, 3) == 0 ? 3 : 2;
            int minX = 0, maxX = boardCols - 1;
            int minY = 0, maxY = boardRows - 1;

            switch (rotate)
            {
                case 0:
                    maxY = boardRows - size;
                    break;
                case 1:
                    minX = size - 1;
                    break;
                case 2:
                    minY = size - 1;
                    break;
                case 3:
                    maxX = boardCols - size;
                    break;
            }
            if (minX > maxX || minY > maxY)
            {
                attempts++;
                continue;
            }
            int x = random.Next(minX, maxX + 1);
            int y = random.Next(minY, maxY + 1);
            CarPlacement placement = new CarPlacement(x, y, rotate, size);

            if (CheckCollision(placement, placements, boardRows, boardCols))
            {
                attempts++;
                continue;
            }

            bool faceConflict = false;
            
            foreach (var other in placements)
            {
                if (FacesEachOther(placement, other))
                {
                    faceConflict = true;
                    break;
                }
            }
            if (faceConflict)
            {
                attempts++;
                continue;
            }

            placements.Add(placement);
            attempts++;
        }
        return placements;
    }

    public static List<CarPlacement> GenerateLevel_Test(int vehicleCount, int boardRows, int boardCols)
    {
        int maxAttemptsPerVehicle = 100;
        int attempts = 0;
        List<CarPlacement> placements = new List<CarPlacement>();
        Random random = new Random();

        while (placements.Count < vehicleCount)
        {
            if (attempts > vehicleCount * maxAttemptsPerVehicle)
                break;

            CarPlacement candidate = new CarPlacement();

            if (placements.Count == 0)
            {
                // Для первого автомобиля – размещаем как обычно
                int rotate = random.Next(0, 4);
                int size = random.Next(0, 3) == 0 ? 3 : 2;
                int minX = 0, maxX = boardCols - 1;
                int minY = 0, maxY = boardRows - 1;
                switch (rotate)
                {
                    case 0:
                        maxY = boardRows - size;
                        break;
                    case 1:
                        minX = size - 1;
                        break;
                    case 2:
                        minY = size - 1;
                        break;
                    case 3:
                        maxX = boardCols - size;
                        break;
                }
                if (minX > maxX || minY > maxY)
                {
                    attempts++;
                    continue;
                }
                int x = random.Next(minX, maxX + 1);
                int y = random.Next(minY, maxY + 1);
                candidate = new CarPlacement(x, y, rotate, size);

                if (CheckCollision(candidate, placements, boardRows, boardCols))
                {
                    attempts++;
                    continue;
                }
                bool faceConflict = false;
                foreach (var other in placements)
                {
                    if (FacesEachOther(candidate, other))
                    {
                        faceConflict = true;
                        break;
                    }
                }
                if (faceConflict)
                {
                    attempts++;
                    continue;
                }
            }
            else
            {
                // Для последующих автомобилей – сначала ищем варианты, где новый автомобиль будет касаться предыдущего
                var last = placements[placements.Count - 1];
                List<CarPlacement> candidateList = new List<CarPlacement>();

                // Перебираем все варианты по вращению и размеру
                for (int rotate = 0; rotate < 4; rotate++)
                {
                    for (int size = 2; size <= 3; size++)
                    {
                        int minX = 0, maxX = boardCols - 1;
                        int minY = 0, maxY = boardRows - 1;
                        switch (rotate)
                        {
                            case 0:
                                maxY = boardRows - size;
                                break;
                            case 1:
                                minX = size - 1;
                                break;
                            case 2:
                                minY = size - 1;
                                break;
                            case 3:
                                maxX = boardCols - size;
                                break;
                        }
                        for (int x = minX; x <= maxX; x++)
                        {
                            for (int y = minY; y <= maxY; y++)
                            {
                                CarPlacement temp = new CarPlacement(x, y, rotate, size);
                                if (CheckCollision(temp, placements, boardRows, boardCols))
                                    continue;
                                bool faceConflict = false;
                                foreach (var other in placements)
                                {
                                    if (FacesEachOther(temp, other))
                                    {
                                        faceConflict = true;
                                        break;
                                    }
                                }
                                if (faceConflict)
                                    continue;
                                // Проверяем, что размещение касается предыдущего автомобиля
                                if (!Touches(temp, last))
                                    continue;

                                candidateList.Add(temp);
                            }
                        }
                    }
                }

                if (candidateList.Count > 0)
                {
                    candidate = candidateList[random.Next(0, candidateList.Count)];
                }
                else
                {
                    // Если вариантов с касанием нет, размещаем автомобиль в любом доступном месте (как в оригинале)
                    int rotate = random.Next(0, 4);
                    int size = random.Next(0, 3) == 0 ? 3 : 2;
                    int minX = 0, maxX = boardCols - 1;
                    int minY = 0, maxY = boardRows - 1;
                    switch (rotate)
                    {
                        case 0:
                            maxY = boardRows - size;
                            break;
                        case 1:
                            minX = size - 1;
                            break;
                        case 2:
                            minY = size - 1;
                            break;
                        case 3:
                            maxX = boardCols - size;
                            break;
                    }
                    if (minX > maxX || minY > maxY)
                    {
                        attempts++;
                        continue;
                    }
                    int x = random.Next(minX, maxX + 1);
                    int y = random.Next(minY, maxY + 1);
                    candidate = new CarPlacement(x, y, rotate, size);
                    if (CheckCollision(candidate, placements, boardRows, boardCols))
                    {
                        attempts++;
                        continue;
                    }
                    bool faceConflict = false;
                    foreach (var other in placements)
                    {
                        if (FacesEachOther(candidate, other))
                        {
                            faceConflict = true;
                            break;
                        }
                    }
                    if (faceConflict)
                    {
                        attempts++;
                        continue;
                    }
                }
            }
            placements.Add(candidate);
            attempts++;
        }
        return placements;
    }

    // Вспомогательный метод, возвращающий список ячеек (x, y), занимаемых автомобилем.
    private static List<(int, int)> GetCells(CarPlacement placement)
    {
        List<(int, int)> cells = new List<(int, int)>();
        int x = placement.X;
        int y = placement.Y;
        int size = placement.Size;
        switch (placement.Rotate)
        {
            case 0: // Вертикально вниз
                for (int i = 0; i < size; i++)
                    cells.Add((x, y + i));
                break;
            case 1: // Горизонтально влево
                for (int i = 0; i < size; i++)
                    cells.Add((x - i, y));
                break;
            case 2: // Вертикально вверх
                for (int i = 0; i < size; i++)
                    cells.Add((x, y - i));
                break;
            case 3: // Горизонтально вправо
                for (int i = 0; i < size; i++)
                    cells.Add((x + i, y));
                break;
        }
        return cells;
    }

    private static bool Touches(CarPlacement a, CarPlacement b)
    {
        var cellsA = GetCells(a);
        var cellsB = GetCells(b);
        foreach (var cellA in cellsA)
        {
            foreach (var cellB in cellsB)
            {
                if (Math.Abs(cellA.Item1 - cellB.Item1) + Math.Abs(cellA.Item2 - cellB.Item2) == 1)
                    return true;
            }
        }
        return false;
    }

}
