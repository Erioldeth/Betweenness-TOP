namespace Betweenness
{
	internal class Genome<T>
		where T : notnull
	{
		public static List<(T, T, T)> Orders { get; }
		public static List<T> Bases { get; }
		public List<T> Dna { get; } = new();
		public Dictionary<T, int> GeneId { get; } = new();

		public static void Initialize() { }

		static Genome()
		{
			Orders = new();
			Bases = new();
		}

		public Genome(bool isRandomDna)
		{
			if(isRandomDna)
				ConstructDna();
		}

		public void Mutate()
		{
			bool mutated = false;
			Random rand = new();
			while(!mutated)
			{
				var gene = Bases[rand.Next(Bases.Count)];
				if(!GeneId.ContainsKey(gene))
				{
					AddGene(gene);
					mutated = true;
				}
			}
		}

		public void ConstructDna()
		{
			Dna.Clear();
			GeneId.Clear();

			Dna.AddRange(ShuffleBases());
			var pos = 0;
			Dna.ForEach(gene => GeneId.Add(gene, pos++));
		}

		public void AddGene(T gene)
		{
			GeneId.Add(gene, Dna.Count);
			Dna.Add(gene);
		}

		private static List<T> ShuffleBases()
		{
			int n = Bases.Count;
			Random rand = new();
			while(n > 1)
			{
				var k = rand.Next(n--);
				(Bases[k], Bases[n]) = (Bases[n], Bases[k]);
			}
			return Bases.ToList();
		}
	}
}
