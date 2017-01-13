using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment_3
{
    /// <summary>
    /// Main class for the application
    /// </summary>
    class Program
    {
        private static bool StopExecution;
        private static CancellationTokenSource cts;
        private DateTime StartTime = DateTime.Now;

        /// <summary>
        /// Main method for the application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Create global variables
            List<City> CityList = new List<City>();
            List<State> StateList = new List<State>();
            List<Connection> ConnectionList = new List<Connection>();
            State SelectedState = new State();
            bool verbose = false;
            StopExecution = false;
            cts = new CancellationTokenSource();
            // End of create global variables
            //int[,] Tablero = new int[7, 7] {{0  ,1  ,2  ,3  ,4  ,5  ,6},
            //                                {-1 ,0  ,7  ,8  ,9  ,0  ,1},
            //                                {-1 ,-1 ,0  ,2  ,3  ,4  ,5},
            //                                {-1 ,-1 ,-1 ,0  ,6  ,7  ,8},
            //                                {-1 ,-1 ,-1 ,-1 ,0  ,9  ,0},
            //                                {-1 ,-1 ,-1 ,-1 ,-1 ,0  ,1},
            //                                {-1 ,-1 ,-1 ,-1 ,-1 ,-1 ,0}};

            int[,] Tablero = new int[8, 8] {{0  ,1  ,2  ,3  ,4  ,5  ,6, 7},
                                            {-1 ,0  ,7  ,8  ,9  ,0  ,1, 7},
                                            {-1 ,-1 ,0  ,2  ,3  ,4  ,5, 7},
                                            {-1 ,-1 ,-1 ,0  ,6  ,7  ,8, 7},
                                            {-1 ,-1 ,-1 ,-1 ,0  ,9  ,0, 7},
                                            {-1 ,-1 ,-1 ,-1 ,-1 ,0  ,1, 7},
                                            {-1 ,-1 ,-1 ,-1 ,-1 ,-1 ,0, 7},
                                            {-1 ,-1 ,-1 ,-1 ,-1 ,-1 ,-1, 0}};

            //int[,] Tablero = new int[6, 6] {{0  ,1  ,2  ,3  ,4  ,5},
            //                                {-1 ,0  ,6  ,7  ,8  ,9},
            //                                {-1 ,-1 ,0  ,0  ,1  ,2},
            //                                {-1 ,-1 ,-1 ,0  ,3  ,4},
            //                                {-1 ,-1 ,-1 ,-1 ,0  ,5},
            //                                {-1 ,-1 ,-1 ,-1 ,-1 ,0}};

            //int[,] Tablero = new int[5, 5] {{0  ,1  ,2  ,3  ,4},
            //                                {-1 ,0  ,5  ,6  ,7},
            //                                {-1 ,-1 ,0  ,8  ,9},
            //                                {-1 ,-1 ,-1 ,0  ,0},
            //                                {-1 ,-1 ,-1 ,-1 ,0}};

            int cityCount = Tablero.GetLength(0);
            InsertInputInSystem(CityList, ConnectionList, Tablero, cityCount);

            // verbose option input from user
            Console.WriteLine("Do you want the verbose version (y/n):");
            string v = Console.ReadLine();
            if (v == "y")
            {
                verbose = true;
            }// End of verbose option input from user

            // End input-graph input

            // Create base class and calculate its minimum threshold
            if (verbose)
            {
                Console.WriteLine("Creating Base class of the graph.\n");
            }
            // Call priorityList implementation to insert the base state
            InsertInOrder(StateList, new State
            {
                StateName = "BaseState",
                Cities = CityList,
                ConsideredList = ConnectionList,
                ExcludeList = new List<Connection>(),
                IncludeList = new List<Connection>(),
                MinimumThreshold = 0,
                Status = true,
                Considered = false
            });
            // Call functionality to calculate minimum threshold of base state
            StateList.First().MinimumThreshold = CalculateMinimumThreshold(StateList.First(), StateList, verbose);
            if (verbose)
            {
                Console.WriteLine("Minimum threshold of base state ={0}\n", StateList.First().MinimumThreshold);
            }//End base state initialization

            // Call functionality to get next parent by Best first Search
            SelectedState = GetNextStates(StateList).First();
            if (verbose)
            {
                Console.WriteLine("Generating children nodes for parent: {0}\n", SelectedState.StateName);
            }
            // Call functionality to generate children of the parent state
            GenerateChildState(CityList, StateList, SelectedState.ConsideredList, SelectedState, verbose, null);
            Console.ReadLine();
        }// End of main method

        private static void InsertInputInSystem(List<City> CityList, List<Connection> ConnectionList, int[,] Tablero, int cityCount)
        {
            Console.WriteLine("The input Adjacency matrix is:");
            for (int i = 0; i < cityCount; i++)
            {
                // Call functionality to enter city name from the graph
                CityList.Add(new City { CityName = i.ToString() });
            }
            for (int i = 0; i < cityCount; i++)
            {
                for (int j = 0; j < cityCount; j++)
                {
                    Console.Write(string.Format("{0}\t", Tablero[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
                // Consider all combinations between all cities
                for (int j = i + 1; j < cityCount; j++)
                {
                    int pathCost = Tablero[i, j];
                    // Create instance of connection class
                    Connection path = new Connection
                    {
                        ConnectionName = CityList.ElementAt(i).CityName + CityList.ElementAt(j).CityName,
                        Source = CityList.ElementAt(i),
                        Destination = CityList.ElementAt(j),
                        PathCost = pathCost
                    };
                    // If the path is valid
                    if (path.PathCost > 0)
                    {
                        // Insert it into the priority list of source city
                        InsertInOrder(CityList.ElementAt(i).Connections, path);
                        // Insert it into the priority list of destination city
                        InsertInOrder(CityList.ElementAt(j).Connections, path);
                        // Avoid duplicates
                        if (!ConnectionList.Contains(path))
                        {
                            // Insert it into the priority list of all connections
                            InsertInOrder(ConnectionList, path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Functionality to generate children of the Parent state and continue
        /// </summary>
        /// <param name="CityList"></param>
        /// <param name="StateList"></param>
        /// <param name="ConnectionList"></param>
        /// <param name="SelectedState"></param>
        /// <param name="verbose"></param>
        private static void GenerateChildState(List<City> CityList, List<State> StateList, List<Connection> ConnectionList, State SelectedState, bool verbose, ParallelLoopState loopstate)
        {
            // Finding out the next connection to be considered for branching
            Connection ConcerenedConnection = new Connection();
            // Choosing from parent's list of connections handed down by the parent
            if (ConnectionList.Any(x=>x.Considered==false))
            {
                // Selecting connection that has not been considered before
                ConcerenedConnection = ConnectionList.Where(x => x.Considered == false).First();
            }
            else
            {
                // Else return with null if all connections have been considered
                return;
            }
            // Mark the selected connection as considered for future reference
            ConcerenedConnection.Considered = true;

            if (verbose)
            {
                Console.WriteLine("Considering connection: {0}\n", ConcerenedConnection.ConnectionName);
            }

            // Creating child state that includes the concerend connection and calculating its minimum threshold
            // Creating seperate copy of parent state
            State IState = SelectedState.CreateDeepCopy(SelectedState);
            // Adding connection to be considered in its include list data structure
            IState.IncludeList.Add(ConcerenedConnection);
            // Call generate name functionality to name the child state
            IState.StateName = GenerateStatename(IState.IncludeList, IState.ExcludeList);
            // Initialize the state as active
            IState.Status = true;
            IState.Considered = false;
            // Call functionality to calculate the state's minimum threshold
            IState.MinimumThreshold = CalculateMinimumThreshold(IState, StateList, verbose);
            // If the state is active
            if (IState.Status)
            {
                // Insert it in the state prioritylist
                InsertInOrder(StateList, IState);
            }            
            // If the state has a valid minimum threshold
            if (IState.MinimumThreshold > 0)
            {
                if (verbose)
                {
                    Console.WriteLine("Minimum threshold for {0} = {1}", IState.StateName, IState.MinimumThreshold);
                }
                // And if it id the minimum among all state after considering all connections
                if ((IState.MinimumThreshold<=SelectedState.MinimumThreshold)&&(IState.IncludeList.Count +IState.ExcludeList.Count==IState.ConsideredList.Count))
                {
                    StopExecution = true;
                    // Call print functionality to print optimal path as the state is the optimal state
                    PrintFinalState(IState, loopstate);
                }// Else continue
            }// Else continue
            // End of creating child state that includes the concerend connection

            // Creating child state that excludes the concerend connection and calculating its minimum threshold
            // Creating seperate copy of parent state
            State EState = SelectedState.CreateDeepCopy(SelectedState);
            // Adding connection to be considered in its exclude list data structure
            EState.ExcludeList.Add(ConcerenedConnection);
            // Call generate name functionality to name the child state
            EState.StateName = GenerateStatename(EState.IncludeList, EState.ExcludeList);
            // Initialize the state as active
            EState.Status = true;
            EState.Considered = false;
            // Call functionality to calculate the state's minimum threshold
            EState.MinimumThreshold = CalculateMinimumThreshold(EState, StateList, verbose);
            // If the state is active
            if (EState.Status)
            {
                // Insert it in the state prioritylist
                InsertInOrder(StateList, EState);
            }
            // If the state has a valid minimum threshold
            if (EState.MinimumThreshold > 0)
            {
                if (verbose)
                {
                    Console.WriteLine("Minimum threshold for {0} = {1}\n", EState.StateName, EState.MinimumThreshold);
                }
                // And if it id the minimum among all state after considering all connections
                if ((EState.MinimumThreshold <= SelectedState.MinimumThreshold) && (EState.IncludeList.Count + EState.ExcludeList.Count == EState.ConsideredList.Count))
                {
                    StopExecution = true;
                    // Call print functionality to print optimal path as the state is the optimal state
                    PrintFinalState(EState, loopstate);
                }// Else continue
            }// Else continue
            // End of creating child state that excludes the concerend connection

            if (verbose)
            {
                Console.WriteLine("Deleting Parent: {0}\n", SelectedState.StateName);
            }
            // Delete existing parent from state priority list
            StateList.RemoveAll(x => x.StateName == SelectedState.StateName);
            Console.WriteLine("Total threads at the moment: {0}", Process.GetCurrentProcess().Threads.Count.ToString());
            Process.GetCurrentProcess().Close();

            // Call functionality to get next parent by Best First Search
            if (!StopExecution)
            {
                List<State> ValidSelectedStates = GetNextStates(StateList);
                if (ValidSelectedStates.Count>0)
                {
                    Parallel.ForEach(ValidSelectedStates, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = cts.Token }, (selectedState, loopState) =>
                    {
                        if (verbose)
                        {
                            Console.WriteLine("The next state to be parent: {0}\n", selectedState.StateName);
                        }
                        if (verbose)
                        {
                            Console.WriteLine("Generating children nodes for parent: {0}\n", selectedState.StateName);
                        }
                        // Call functionality to generate children of the new parent state
                        GenerateChildState(CityList, StateList, selectedState.ConsideredList, selectedState, verbose, loopState);
                    });
                }
            }
            else
            {

            }
        }// End of GenerateChildState method

        /// <summary>
        /// Functionality to print the optimal path of the TSP
        /// </summary>
        /// <param name="state"></param>
        private static void PrintFinalState(State state, ParallelLoopState loopState)
        {
            // Initialize string to display optimal path
            string FinalAnswer = "";
            // Deleting all connections from the final state that are in its exclude list data structure in parallel
            Parallel.ForEach(state.ExcludeList, (delCon) =>
            {
                // Consider each city in the final state in parallel
                Parallel.ForEach(state.Cities, (delCity) =>
                {
                    if (delCity.Connections.Exists(x => x.ConnectionName == delCon.ConnectionName))
                    {
                        delCity.Connections.RemoveAll(x => x.ConnectionName == delCon.ConnectionName);
                    }
                });
            });// End of excluding connections from final state in parallel
            // Print filtered connection for the optimal path in parallel
            Parallel.ForEach(state.Cities, (finalCity) =>
            {
                // Consider all cities of the final state in parallel
                Parallel.ForEach(finalCity.Connections, (finalConnection) =>
                {
                    // Avoid duplicate connections
                    if (!FinalAnswer.Contains(finalConnection.ConnectionName))
                    {
                        FinalAnswer += " " + finalConnection.ConnectionName + ";";
                    }
                });
            });
            // Print the optimal path
            Console.WriteLine("The optimal path for the TSP is :{0} with a path cost of {1}.", FinalAnswer, state.MinimumThreshold);
            cts.Cancel();
            Console.ReadLine();
            // Exit application
            Environment.Exit(0);
        }// End of PrintFinalState method

        /// <summary>
        /// Functionality to get next parent state
        /// </summary>
        /// <param name="StateList"></param>
        /// <returns></returns>
        private static List<State> GetNextStates(List<State> StateList)
        {
            // Initialize local variable to hold the result
            State MinState;
            List<State> SelectedStates;
            // Retrieve best state to be considered
            MinState = StateList.Where(x => x.Status == true).First();
            SelectedStates = StateList.Where(x => (x.Status == true) && (x.Considered==false) && (x.MinimumThreshold==MinState.MinimumThreshold)).ToList();
            Parallel.ForEach (SelectedStates, (state) =>
                state.Considered = true);
            // Return result
            return SelectedStates;
        }// End of GetNextState method

        /// <summary>
        /// Functionality to generate name of children states
        /// </summary>
        /// <param name="Include"></param>
        /// <param name="Exclude"></param>
        /// <returns></returns>
        private static string GenerateStatename(List<Connection> Include, List<Connection> Exclude)
        {
            string include = "";
            string exclude = "";
            string stateName = "State (";

            if (Include != null)
            {
                // Include all connections in the include part of the state name in parallel
                Parallel.ForEach(Include, (item) =>
                {
                    include += item.ConnectionName + "; ";
                });
            }
            else
            {
                // Else give - if no connections are present in the include data structure
                include = "-";
            }
            if (Exclude != null)
            {
                // Include all connections in the exclude part of the state name in parallel
                Parallel.ForEach(Exclude, (item) =>
                {
                    exclude += item.ConnectionName + "; ";
                });
            }
            else
            {
                // Else give - if no connections are present in the exclude data structure
                exclude = "-";
            }
            // Generate state name by concatenating all parts
            stateName = stateName + "Including: " + include + "Excluding: " + exclude +")";
            // Return the state name
            return stateName;
        }// End of GenerateStatename method

        /// <summary>
        /// Functionality to calculate the minimum threshold of the given state
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="stateList"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        private static double CalculateMinimumThreshold(State currentState, List<State> stateList, bool verbose)
        {
            // Initiate local variable to hold the minimum threshold of the class
            double minimumthreshold = 0;
            // Create copy of given state for manipulation
            State MinConState = currentState.CreateDeepCopy(currentState);
            // Delete all connections in state that are present in its exclude data structure in parallel
            //Parallel.ForEach(MinConState.ExcludeList, (delCon, loopState) =>
            foreach (Connection delCon in MinConState.ExcludeList)
            {
                //Parallel.ForEach(MinConState.Cities, (delCity) =>
                foreach(City delCity in MinConState.Cities)
                {
                    if (delCity.Connections.Exists(x => x.ConnectionName == delCon.ConnectionName))
                    {
                        delCity.Connections.RemoveAll(x => x.ConnectionName == delCon.ConnectionName);
                        // If a city in the given state is left with only one connection
                        if (delCity.Connections.Count < 2)
                        {
                            // Prune the given state
                            if (verbose && currentState.Status)
                            {
                                Console.WriteLine(currentState.StateName + " pruned because city '" + delCity.CityName + "' needs at least two connections.");
                            }
                            currentState.Status = false;
                            //loopState.Stop();
                            //return;
                        }
                    }
                }
            }
            // Prioritize all connections in state that are present in its include data structure in parallel
            //Parallel.ForEach(MinConState.IncludeList, (inCon, loopState) =>
            foreach(Connection inCon in MinConState.IncludeList)
            {
                //Parallel.ForEach(MinConState.Cities, (inCity) =>
                foreach(City inCity in MinConState.Cities)
                {
                    var result = inCity.Connections.Where(p => MinConState.IncludeList.Any(p2 => p2.ConnectionName == p.ConnectionName));
                    // If a city in the given state has more than two connection in the include data structure
                    if (result.Count() > 2)
                    {
                        // Prune the given state
                        if (verbose && currentState.Status)
                        {
                            Console.WriteLine(currentState.StateName + " pruned because city " + inCity.CityName + " already has two of its connections included.");
                        }
                        currentState.Status = false;
                        //loopState.Stop();
                        //return;
                    }
                    // Otherwise prioritize the include data structure connections in every city of the given state
                    if (inCity.Connections.Exists(x => x.ConnectionName == inCon.ConnectionName))
                    {
                        inCity.Connections.RemoveAll(x => x.ConnectionName == inCon.ConnectionName);
                        inCity.Connections.Insert(0, inCon);
                    }
                }
            }
            // Calculate the minimum threshold for the given state in parallel
            if (currentState.Status)
            {
                Parallel.ForEach(MinConState.Cities, (city) =>
                {
                    minimumthreshold += (city.Connections.ElementAt(0).PathCost + city.Connections.ElementAt(1).PathCost);
                });
                // Return minimum threshold of the given state
                return minimumthreshold /= 2.00;
            }
            else
            {
                return 0;
            }
        }// End of CalculateMinimumThreshold method

        /// <summary>
        /// Priority list implementation for connections
        /// </summary>
        /// <param name="inputConList"></param>
        /// <param name="inputCon"></param>
        private static void InsertInOrder(List<Connection> inputConList, Connection inputCon)
        {
            // If existing list is not empty
            if (inputConList.Count>0)
            {
                // If existing list has an element with minimum threshold higher than the considered connection
                if (inputConList.Exists(x=>x.PathCost>=inputCon.PathCost))
                {
                    // Insert the considered connection before the found element
                    inputConList.Insert(inputConList.FindIndex(x => x.PathCost >= inputCon.PathCost), inputCon);
                }
                else
                {
                    // Else add the element in the end of the list
                    inputConList.Add(inputCon);
                }
            }
            else
            {
                // If existing list is empty, insert the element in the first position in the list
                inputConList.Insert(0, inputCon);
            }
        }// End of InsertInOrder method

        /// <summary>
        /// Priority list implementation for states
        /// </summary>
        /// <param name="inputStateList"></param>
        /// <param name="inputState"></param>
        private static void InsertInOrder(List<State> inputStateList, State inputState)
        {
            // If existing list is not empty
            if (inputStateList.Count > 0)
            {
                // If existing list has an element with minimum threshold higher than the considered connection
                if (inputStateList.Exists(x => x.MinimumThreshold >= inputState.MinimumThreshold))
                {
                    // Insert the considered connection before the found element
                    inputStateList.Insert(inputStateList.FindIndex(x => x.MinimumThreshold >= inputState.MinimumThreshold), inputState);
                }
                else
                {
                    // Else add the element in the end of the list
                    inputStateList.Add(inputState);
                }
            }
            else
            {
                // If existing list is empty, insert the element in the first position in the list
                inputStateList.Insert(0, inputState);
            }
        }// End of InsertInOrder method
    }// End of main program
}