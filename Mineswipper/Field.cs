namespace Mineswipper
{
    public class Field
    {
        public Cell[,] cells;
        public int N { get; set; } //N - ряды поля
        public int M { get; set; } //M - столбики поля
        
        public Field(int n, int m) //конструктор
        {
            N = n;
            M = m;
            cells = new Cell[N, M];
        }

    }

}
