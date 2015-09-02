namespace Data.Mining
{
    public static class MiningCompilerExtensions
    {
        public static int GetLevel(this string line)
        {
            var level = default(int);
            for (int c = 0; c <= line.Length - 1; c++) { if (line[c] != ' ') { break; } level++; }
            return level;
        }
    }
}
