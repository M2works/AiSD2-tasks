﻿using System;
using ASD.Graphs;

namespace ASD
{

class FindCycleTestCase : TestCase
    {

    private Graph g;
    private Graph gc;
    private bool expectedResult;
    private bool result;
    private Edge[] cycle;

    public static bool ResultOnly;

    public FindCycleTestCase(double timeLimit, Exception expectedException, Graph gg, bool res) : base(timeLimit,expectedException)
        {
        g = gg.Clone();
        gc = gg.Clone();
        expectedResult = res;
        }

    public override void PerformTestCase()
        {
        result = gc.FindCycle(out cycle);
        }

    public override void VerifyTestCase(out Result resultCode, out string message)
        {
        if ( result!=expectedResult )
            {
            resultCode = Result.BadResult;
            message = $"incorrect result: {result} (expected: {expectedResult})";
            return;
            }
        if ( !expectedResult && cycle!=null )
            {
            resultCode = Result.BadResult;
            message = $"icorrect cycle: {cycle.ElementsToString()} (expected: null)";
            return;
            }
        if ( !g.IsEqual(gc) )
            {
            resultCode = Result.BadResult;
            message = "graph was destroyed";
            return;
            }
        if ( ResultOnly )
            {
            resultCode = Result.Success;
            message = "OK";
            return;
            }

        if ( expectedResult && cycle==null )
            {
            resultCode = Result.BadResult;
            message = $"icorrect cycle: unexpected null";
            return;
            }
        if ( expectedResult )
            TestCycle(out resultCode, out message);
        else
            {
            resultCode = Result.Success;
            message = "OK";
            }
        }

    private void TestCycle(out Result resultCode, out string message)
        {
        if ( cycle.Length < (g.Directed?2:3) )
            {
            resultCode = Result.BadResult;
            message = $"cycle to short: {cycle.Length} edge, {cycle.ElementsToString()}";
            return;
            }
        foreach ( Edge e in cycle )
            if ( g.GetEdgeWeight(e.From,e.To)!=e.Weight )
                {
                resultCode = Result.BadResult;
                message = $"incorrect edge in cycle: {e}, {cycle.ElementsToString()}";
                return;
                }
        bool[] used = new bool[g.VerticesCount];
        used[cycle[0].From]=true;
        for ( int i=1 ; i<cycle.Length ; ++i )
            {
            if ( cycle[i].From!=cycle[i-1].To || used[cycle[i].From] )
                {
                resultCode = Result.BadResult;
                message = "incorrect vertices sequence in cycle, {cycle.ElementsToString()}";
                return;
                }
            used[cycle[i].From]=true;
            }
        if ( cycle[0].From!=cycle[cycle.Length-1].To )
            {
            resultCode = Result.BadResult;
            message = "incorrect vertices sequence in cycle, {cycle.ElementsToString()}";
            return;
            }
        resultCode = Result.Success;
        message = "OK";
        }

    }

class TreeCenterTestCase : TestCase
    {

    private Graph g;
    private Graph gc;
    private bool expectedResult;
    private bool result;
    private int[] expectedCenter;
    private int[] center;

    public static bool ResultOnly;

    public TreeCenterTestCase(double timeLimit, Exception expectedException, Graph gg, bool res, int[] ec) : base(timeLimit,expectedException)
        {
        g = gg.Clone();
        gc = gg.Clone();
        expectedResult = res;
        expectedCenter = ec;
        }

    public override void PerformTestCase()
        {
        result = gc.TreeCenter(out center);
        }

    public override void VerifyTestCase(out Result resultCode, out string message)
        {
        if ( result!=expectedResult )
            {
            resultCode = Result.BadResult;
            message = $"incorrect result: {result} (expected: {expectedResult})";
            return;
            }
        if ( !expectedResult && center!=null )
            {
            resultCode = Result.BadResult;
            message = $"icorrect center: {center.ElementsToString()} (expected: null)";
            return;
            }
        if ( !g.IsEqual(gc) )
            {
            resultCode = Result.BadResult;
            message = "graph was destroyed";
            return;
            }
        if ( ResultOnly )
            {
            resultCode = Result.Success;
            message = "OK";
            return;
            }

        if ( expectedResult && center==null )
            {
            resultCode = Result.BadResult;
            message = $"icorrect center: unexpected null";
            return;
            }
        bool ok = true;
        if ( expectedResult )
            {
            switch ( center.Length )
                {
                case 1:
                    ok = expectedCenter.Length==1 && center[0]==expectedCenter[0] ;
                    break;
                case 2:
                    ok = expectedCenter.Length==2 && ( ( center[0]==expectedCenter[0] && center[1]==expectedCenter[1] ) || ( center[0]==expectedCenter[1] && center[1]==expectedCenter[0] ) ) ;
                    break;
                default:
                    ok=false;
                    break;
                }
            }
        if ( !ok )
            {
            resultCode = Result.BadResult;
            message = $"incorrect center: {center.ElementsToString()} (expected: {expectedCenter.ElementsToString()}";
            }
        else
            {
            resultCode = Result.Success;
            message = "OK";
            }
        }

    }

class Lab03
    {

    static void Main(string[] args)
        {
        GraphExport ge = new GraphExport();
        RandomGraphGenerator rgg = new RandomGraphGenerator();

        Graph[] directedGraphs = new Graph[8];
        directedGraphs[0] = new AdjacencyListsGraph<AVLAdjacencyList>(true,3) { new Edge(0,1), new Edge(1,2), new Edge(2,0) };
        directedGraphs[1] = new AdjacencyListsGraph<SimpleAdjacencyList>(true,3) { new Edge(0,1), new Edge(1,2), new Edge(2,1) };
        directedGraphs[2] = new AdjacencyMatrixGraph(true,4) { new Edge(0,1), new Edge(0,2), new Edge(1,3), new Edge(2,3) };
        directedGraphs[3] = new AdjacencyListsGraph<HashTableAdjacencyList>(true,10);
        directedGraphs[4] = new AdjacencyMatrixGraph(true,10) { new Edge(0,1), new Edge(0,2), new Edge(0,3), new Edge(2,4), new Edge(2,5), new Edge(2,6),
                                                                new Edge(5,7), new Edge(5,8), new Edge(5,9), new Edge(6,5), new Edge(7,8), new Edge(8,2) };
        rgg.SetSeed(111);
        directedGraphs[5] = rgg.DirectedGraph(typeof(AdjacencyMatrixGraph),100,0.2);
        rgg.SetSeed(222);
        directedGraphs[6] = rgg.DirectedCycle(typeof(AdjacencyListsGraph<SimpleAdjacencyList>),1000);
        rgg.SetSeed(333);
        directedGraphs[7] = rgg.DAG(typeof(AdjacencyMatrixGraph),200,0.2,1,1);

        TestSet findCycleDirected = new TestSet();
        findCycleDirected.TestCases.Add(new FindCycleTestCase(5, null, directedGraphs[0], true));
        findCycleDirected.TestCases.Add(new FindCycleTestCase(5, null, directedGraphs[1], true));
        findCycleDirected.TestCases.Add(new FindCycleTestCase(5, null, directedGraphs[2], false));
        findCycleDirected.TestCases.Add(new FindCycleTestCase(5, null, directedGraphs[3], false));
        findCycleDirected.TestCases.Add(new FindCycleTestCase(5, null, directedGraphs[4], true));
        findCycleDirected.TestCases.Add(new FindCycleTestCase(5, null, directedGraphs[5], true));
        findCycleDirected.TestCases.Add(new FindCycleTestCase(5, null, directedGraphs[6], true));
        findCycleDirected.TestCases.Add(new FindCycleTestCase(5, null, directedGraphs[7], false));

        Graph[] undirectedGraphs = new Graph[6];
        undirectedGraphs[0] = new AdjacencyListsGraph<AVLAdjacencyList>(false,3) { new Edge(0,1), new Edge(1,2), new Edge(2,0) };
        undirectedGraphs[1] = new AdjacencyListsGraph<SimpleAdjacencyList>(false,4) { new Edge(0,1), new Edge(1,2), new Edge(2,3), new Edge(3,1) };
        undirectedGraphs[2] = new AdjacencyListsGraph<HashTableAdjacencyList>(false,10);
        undirectedGraphs[3] = new AdjacencyMatrixGraph(false,10) { new Edge(0,1), new Edge(0,2), new Edge(0,3), new Edge(2,4), new Edge(2,5), new Edge(2,6),
                                                                   new Edge(5,7), new Edge(5,8), new Edge(5,9), new Edge(8,2) };
        rgg.SetSeed(444);
        undirectedGraphs[4] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph),100,0.2);
        rgg.SetSeed(555);
        undirectedGraphs[5] = rgg.TreeGraph(typeof(AdjacencyListsGraph<SimpleAdjacencyList>),1000,1.0);

        TestSet findCycleUndirected = new TestSet();
        findCycleUndirected.TestCases.Add(new FindCycleTestCase(5, null, undirectedGraphs[0], true));
        findCycleUndirected.TestCases.Add(new FindCycleTestCase(5, null, undirectedGraphs[1], true));
        findCycleUndirected.TestCases.Add(new FindCycleTestCase(5, null, undirectedGraphs[2], false));
        findCycleUndirected.TestCases.Add(new FindCycleTestCase(5, null, undirectedGraphs[3], true));
        findCycleUndirected.TestCases.Add(new FindCycleTestCase(5, null, undirectedGraphs[4], true));
        findCycleUndirected.TestCases.Add(new FindCycleTestCase(5, null, undirectedGraphs[5], false));

        Graph[] trees = new Graph[10];
        trees[0] = new AdjacencyListsGraph<AVLAdjacencyList>(false,3) { new Edge(0,1), new Edge(1,2) };
        trees[1] = new AdjacencyListsGraph<SimpleAdjacencyList>(false,4) { new Edge(0,1), new Edge(1,2), new Edge(2,3) };
        trees[2] = new AdjacencyListsGraph<HashTableAdjacencyList>(false,1);
        trees[3] = new AdjacencyListsGraph<HashTableAdjacencyList>(false,2) { new Edge(0,1) };
        trees[4] = new AdjacencyMatrixGraph(false,5) { new Edge(1,3), new Edge(2,4) };
        trees[5] = new AdjacencyMatrixGraph(true,3) { new Edge(0,1), new Edge(0,2) };
        rgg.SetSeed(777);
        trees[6] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph),100,0.2);
        rgg.SetSeed(888);
        trees[7] = rgg.TreeGraph(typeof(AdjacencyListsGraph<SimpleAdjacencyList>),1000,1.0);
        rgg.SetSeed(999);
        trees[8] = rgg.TreeGraph(typeof(AdjacencyListsGraph<SimpleAdjacencyList>),1001,1.0);
        trees[9] = new AdjacencyListsGraph<SimpleAdjacencyList>(false,10);
        for ( int i=1 ; i<10 ; ++i )
            trees[9].AddEdge(i-1,i);

        TestSet treeCenter = new TestSet();
        treeCenter.TestCases.Add(new TreeCenterTestCase(5, null, trees[0], true, new int[] {1} ));
        treeCenter.TestCases.Add(new TreeCenterTestCase(5, null, trees[1], true, new int[] {1,2} ));
        treeCenter.TestCases.Add(new TreeCenterTestCase(5, null, trees[2], true, new int[] {0} ));
        treeCenter.TestCases.Add(new TreeCenterTestCase(5, null, trees[3], true, new int[] {0,1} ));
        treeCenter.TestCases.Add(new TreeCenterTestCase(5, null, trees[4], false, null ));
        treeCenter.TestCases.Add(new TreeCenterTestCase(5, new ArgumentException(), trees[5], false, null ));
        treeCenter.TestCases.Add(new TreeCenterTestCase(5, null, trees[6], false, null ));
        treeCenter.TestCases.Add(new TreeCenterTestCase(5, null, trees[7], true, new int[] {305,786} ));
        treeCenter.TestCases.Add(new TreeCenterTestCase(5, null, trees[8], true, new int[] {60} ));
        treeCenter.TestCases.Add(new TreeCenterTestCase(5, null, trees[9], true, new int[] {4,5} ));

        //
        // Odkomentuj odpowiednią linię aby zobaczyć wybrany graf
        // Pamiętaj, że przykłady numerowane są od 1
        //
//        ge.Export(directedGraphs[0]);
//        ge.Export(directedGraphs[1]);
//        ge.Export(directedGraphs[2]);
//        ge.Export(directedGraphs[3]);
//        ge.Export(directedGraphs[4]);
//        ge.Export(directedGraphs[5]);
//        ge.Export(directedGraphs[6]);
//        ge.Export(directedGraphs[7]);
//        ge.Export(undirectedGraphs[0]);
//        ge.Export(undirectedGraphs[1]);
//        ge.Export(undirectedGraphs[2]);
//        ge.Export(undirectedGraphs[3]);
//        ge.Export(undirectedGraphs[4]);
//        ge.Export(undirectedGraphs[5]);
//        ge.Export(trees[0]);
//        ge.Export(trees[1]);
//        ge.Export(trees[2]);
//        ge.Export(trees[3]);
//        ge.Export(trees[4]);
//        ge.Export(trees[5]);
//        ge.Export(trees[6]);
//        ge.Export(trees[7]);
//        ge.Export(trees[8]);

        Console.WriteLine("\nCycle Finding\n");

        FindCycleTestCase.ResultOnly=true;
        Console.WriteLine("\nDirected Graphs - result only");
        findCycleDirected.PreformTests(verbose:true, checkTimeLimit:false);
        Console.WriteLine("\nUndirected Graphs - result only");
        findCycleUndirected.PreformTests(verbose:true, checkTimeLimit:false);

        FindCycleTestCase.ResultOnly=false;
        Console.WriteLine("\nDirected Graphs - full funcionality");
        findCycleDirected.PreformTests(verbose:true, checkTimeLimit:false,forceExecution:false);
        Console.WriteLine("\nUndirected Graphs - full funcionality");
        findCycleUndirected.PreformTests(verbose:true, checkTimeLimit:false,forceExecution:false);

        Console.WriteLine("\nTree Center\n");
        TreeCenterTestCase.ResultOnly=true;
        Console.WriteLine("\nResult only");
        treeCenter.PreformTests(verbose:true, checkTimeLimit:false);
        Console.WriteLine("\nFull funcionality");
        TreeCenterTestCase.ResultOnly=false;
        treeCenter.PreformTests(verbose:true, checkTimeLimit:false,forceExecution:false);

        }

    }  // class Lab03

}
