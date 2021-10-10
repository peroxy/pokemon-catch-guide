namespace PokeGuideGenerator
{
    internal partial class Program
    {
        public class EvoTrigger
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }


        public class EvoTriggers
        {
            public int Count { get; set; }
            public object Next { get; set; }
            public object Previous { get; set; }
            public EvoTrigger[] Results { get; set; }
        }

    }
}
