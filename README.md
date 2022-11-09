# Stable Roommates Problem Generalized
This project tests multiple algorithms for the Stable Roommates Problem with rooms of any size 
without the stable requirement that instead optimizes an objective function that 
measures the quality of a matching.
Partitions n people into k groups that are all the same size and where k divides n
such that some objective function is minimized
given a edge weighted graph where w(u,v) is how u prefers v.

There are two choices for objective functions:
Utilitarian objective which maximizes the satisfaction overall.
Deontological objective which maximizes the satisfaction for the person that is least satisfied.

The algorithms include:
* Simple Hill Climbing
* Stochastic Hill Climbing
* First-Choice Hill Climbing
* Random-Restart Hill Climbing
* Simulated Annealing
* Branch and Bound

Various metrics are reported for each algorithm in 3 different experiments.
To run the experiments, in the terminal execute
```console
dotnet run --configuration Release
``` 

Running the experiments with the current configurations can take several hours.
Branch and bound takes the longest time, after that Simulated Annealing dominates.
Reducing the MaxNumBranchVars constant can reduce the runtime and vice-versa.
Branch and Bound will only be run for problems where n*k <= MaxNumBranchVars,
so Branch and Bound will solve O(2^MaxNumBranchVars) linear programs in the worst case for a combination of n and k.

The program will output a csv file for each combination of metric and value of k for each experiment.
These files will be in the "bin/Release" folder.
Note "k0" does not mean k=0, it means the first value of k for that experiment.

To plot the data in the csv files go to the "Python" folder, place
the csv-files in the "Python/Experiments" folder and in the terminal execute
```console
python plots.py
``` 
Then the plots can be found in the "plots" folder as png images.
