namespace Betweenness
{
	internal class Entity<T>
		where T : notnull
	{
		public Genome<T> Gen { get; }
		public int Fitness { get; private set; }

		public Entity(Genome<T> gen)
		{
			Gen = gen;
			CalculateFitness();
		}

		public Entity<T> Mate(Entity<T> other)
		{
			Genome<T> childGen = new(false);
			Random rand = new();
			for(var geneId = 0; geneId < Genome<T>.Bases.Count; geneId++)
			{
				var rate = rand.NextDouble();

				var fatherGene = Gen.Dna[geneId];
				var fatherSelectable = !childGen.GeneId.ContainsKey(fatherGene);

				var motherGene = other.Gen.Dna[geneId];
				var motherSelectable = !childGen.GeneId.ContainsKey(motherGene);

				var parentSelectable = fatherSelectable || motherSelectable;
				if(!parentSelectable || rate > 0.9)
					childGen.Mutate();
				else
				{
					if(fatherSelectable && motherSelectable)
						childGen.AddGene(rate > 0.45 ? fatherGene : motherGene);
					else
						childGen.AddGene(fatherSelectable ? fatherGene : motherGene);
				}
			}
			return new(childGen);
		}

		public void Recreate()
		{
			Gen.ConstructDna();
			CalculateFitness();
		}

		private void CalculateFitness()
		{
			Fitness = 0;
			var geneId = Gen.GeneId;
			Genome<T>.Orders.ForEach(order =>
			{
				(T gene1, T gene2, T gene3) = order;
				(var id1, var id2, var id3) = (geneId[gene1], geneId[gene2], geneId[gene3]);
				if((id1 < id2 && id2 < id3) || (id1 > id2 && id2 > id3))
					Fitness++;
			});
		}
	}
}
