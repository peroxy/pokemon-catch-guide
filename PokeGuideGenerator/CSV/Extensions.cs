namespace PokeGuideGenerator.CSV
{
    public static class Extensions
    {
        public static string ToHumanFormat(this bool value)
        {
            return value ? "yes" : "no";
        }
    }
}
