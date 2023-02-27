using Betweenness;

Genome<int>.Initialize();

var root = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
root = Directory.GetParent(root).FullName;
root = Directory.GetParent(root).FullName;
root = Directory.GetParent(root).FullName;

HashSet<int> materialFilter = new();
foreach(var line in File.ReadAllLines(Path.Combine(root, "Data", "input.txt")))
{
	var materials = line.Split(' ');

	var material0 = Convert.ToInt32(materials[0]);
	var material1 = Convert.ToInt32(materials[1]);
	var material2 = Convert.ToInt32(materials[2]);

	Genome<int>.Orders.Add((material0, material1, material2));

	materialFilter.UnionWith(new[] { material0, material1, material2 });
}
Genome<int>.Bases.AddRange(materialFilter);


const int populationSize = 100;
var currentGeneration = 1;


var population = Enumerable.Range(0, populationSize).Select(_ => new Entity<int>(new(true))).ToList();

while(true)
{
	foreach(var entity in population)
	{
		if(entity.Fitness == Genome<int>.Orders.Count)
		{
			File.WriteAllText(Path.Combine(root, "Data", "output.txt"), $"Generation {currentGeneration}: [{string.Join(", ", entity.Gen.Dna)}]");
			return;
		}
		else if(currentGeneration % 100 == 0 && new Random().NextDouble() > 0.5)
			entity.Recreate();
	}

	List<Entity<int>> newPopulation = new(populationSize);

	population = population.OrderByDescending(entity => entity.Fitness).ToList();

	var totalFitness = 0;

	List<int> accumulateFitness = new(populationSize);

	population.ForEach(entity =>
	{
		accumulateFitness.Add(totalFitness += entity.Fitness);
		if(newPopulation.Count < populationSize / 10)
			newPopulation.Add(entity);
	});

	while(newPopulation.Count < populationSize)
	{
		int fatherFitness = new Random().Next(totalFitness);
		bool fatherSelected = false;
		int fatherId = -1;

		int motherFitness = (fatherFitness + totalFitness / 2) % totalFitness;
		bool motherSelected = false;
		int motherId = -1;

		for(var id = 0; id < populationSize && !(fatherSelected && motherSelected); id++)
		{
			if(!fatherSelected && accumulateFitness[id] > fatherFitness)
			{
				fatherSelected = true;
				fatherId = id;
			}
			if(!motherSelected && accumulateFitness[id] > motherFitness)
			{
				motherSelected = true;
				motherId = id;
			}
		}
		newPopulation.Add(population[fatherId].Mate(population[motherId]));
	}

	population = newPopulation;

	currentGeneration++;
}