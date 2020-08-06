using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WasSagenSie;

internal class CalculationRunner : Command
{
    public override ResultSet Run(ZaagContext context)
    {
        Task.Run(async () =>
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var result = Finalize(await Recurse(context.DeepClone()));
            //Size.IdentityCounter = 1;
            sw.Stop();
            result.Add(new ResultSet(ResultType.Heading, string.Format("Verstreken tijd: {0} ms", sw.ElapsedMilliseconds)));
            result.Print(0, MainClass.PrintResult);
        }); 
        return new ResultSet(ResultType.Heading, "Geactiveerd...");
    }

    private async Task<ZaagContext> Recurse(ZaagContext context)
    {
        if (context.PendingTargets.Count == 0)
        {
            return context;
        }
        else
        {
            ZaagContext nr = null;
            var jr = EvaluateJustRemainders(context);

            if (jr == null)
            {
                nr = EvaluateNewRemainder(context);
            } if (context.Remainders.Count < context.ReachedTargets.Count * 3 + 10)
            {
                nr = EvaluateNewRemainder(context);
            }
            
            if ((nr == null) && (jr == null))
            {
                return EvaluateNewRemainderFlip(context) ?? context;
            }
            else if ((nr != null) && (jr == null))
            {
                return await Recurse(nr);
            }
            else if ((nr == null) && (jr != null))
            {
                return await Recurse(jr);
            }
            else 
            {
                var newResult = await Recurse(nr);
                var remResult = await Recurse(jr);
                if (newResult == null)
                {
                    if (remResult == null)
                    {
                        return context;
                    }
                    else
                    {
                        return remResult;
                    }
                }
                else if (remResult == null)
                {
                    return newResult;
                }
                if (remResult.Cuttables.Count > newResult.Cuttables.Count)
                {
                    return remResult;
                }
                else if (remResult.Cuttables.Count < newResult.Cuttables.Count)
                {
                    return newResult;
                }
                else if (remResult.Remainders.Count > newResult.Remainders.Count)
                {
                    return newResult;
                }
                else
                {
                    return remResult;
                }
            }
        }
    }

    private ZaagContext EvaluateNewRemainderFlip(ZaagContext concept)
    {
        Size bestFresh = null;
        Size nextTarget = null;
        bool found = false;
        foreach (var t in concept.PendingTargets)
        {
            nextTarget = t;
            if (TryBestPlank(concept.Cuttables, nextTarget, out bestFresh))
            {
                found = true;
                break;
            }
        }
        if (found) if (bestFresh != null)
        {
            concept = concept.DeepClone();
            concept.Cuttables.RemoveByIdentity(bestFresh);
            concept.Remainders.Add(bestFresh);
            if (bestFresh.IsRotated) bestFresh.SwapDimensions();
            if (bestFresh.Height > nextTarget.Height) bestFresh = concept.CutHeight(bestFresh, nextTarget.Height).Result;
            if (bestFresh.Width > nextTarget.Width) bestFresh = concept.CutWidth(bestFresh, nextTarget.Width).Result;
            concept.MarkDone(nextTarget, bestFresh);
            return concept;
        }
        return null;
    }

    private ZaagContext EvaluateNewRemainder(ZaagContext concept)
    {
        Size bestFresh = null;
        Size nextTarget = null;
        bool found = false;
        foreach (var t in concept.PendingTargets)
        {
            nextTarget = t;
            if (TryBestPlank(concept.Cuttables, nextTarget, out bestFresh))
            {
                found = true;
                break;
            }
        }
        if (found) if (bestFresh != null)
        {
            concept = concept.DeepClone();
            concept.Cuttables.RemoveByIdentity(bestFresh);
            concept.Remainders.Add(bestFresh);
            if (bestFresh.IsRotated) bestFresh.SwapDimensions();
            if (bestFresh.Width > nextTarget.Width) bestFresh = concept.CutWidth(bestFresh, nextTarget.Width).Result;
            if (bestFresh.Height > nextTarget.Height) bestFresh = concept.CutHeight(bestFresh, nextTarget.Height).Result;
            concept.MarkDone(nextTarget, bestFresh);
            return concept;
        }
        return null;
    }

    private ZaagContext EvaluateJustRemainders(ZaagContext concept)
    {
        Size bestRemainder = null;
        Size nextTarget = null;
        bool found = false;
        foreach (var t in concept.PendingTargets)
        {
            nextTarget = t;
            if (TryBestPlank(concept.Remainders, nextTarget, out bestRemainder))
            {
                found = true;
                break;
            }
        }

        if (found) if (bestRemainder != null)
        {
            concept = concept.DeepClone();
            if (bestRemainder.IsRotated) bestRemainder.SwapDimensions();
            if (bestRemainder.Width > nextTarget.Width) bestRemainder = concept.CutWidth(bestRemainder, nextTarget.Width).Result;
            if (bestRemainder.Height > nextTarget.Height) bestRemainder = concept.CutHeight(bestRemainder, nextTarget.Height).Result;
            concept.MarkDone(nextTarget, bestRemainder);
            return concept;
        }

        return null;
    }

    private bool TryBestPlank(List<Size> planks, Size target, out Size bestRemainder)
    {
        bestRemainder = Size.Max;

        foreach (var candidate in planks)
        {
            bool aBetterFit = CheckIfBetterFit(target, bestRemainder, candidate);
            if (aBetterFit)
                bestRemainder = candidate;
        }

        return bestRemainder != Size.Max;
    }

    private bool CheckIfBetterFit(Size loneTarget, Size bestRemainder, Size candidate)
    {
        candidate.IsRotated = candidate.CanContainRotated(loneTarget);
        return (candidate.IsRotated || candidate.CanContain(loneTarget)) &&
            (bestRemainder.CanContain(candidate) || bestRemainder.CanContainRotated(candidate));
    }

    private ResultSet Finalize(ZaagContext context)
    {
        int counter = 1;
        foreach (var item in context.Cuts)
        {
            item.Source.Identity = counter++;
            item.Result.Identity = counter++;
            item.Remainder.Identity = counter++;
        }

        var allResults = new ResultSet(ResultType.Heading, "Resultaten: ");
        var cutReport = new ResultSet(ResultType.Heading, "Zaagsnedes: ");
        allResults.Add(cutReport);
        foreach (Cut item in context.Cuts)
        {
            string worh = "Breedte";
            if (item.WorH == 'h') worh = "Diepte";
            cutReport.Add(new ResultSet(ResultType.Dimension, string.Format(
            "Zaag plank {10} van {0}x{1} " +
            "over de {2} op {3} om " +
            "plank {8} van {4}x{5} en " +
            "plank {9} {6}x{7} te krijgen",
            item.Source.Width, item.Source.Height,
            worh, item.Position,
            item.Result.Width, item.Result.Height,
            item.Remainder.Width, item.Remainder.Height,
            item.Result.Identity, item.Remainder.Identity,
            item.Source.Identity)));
        }

        var remainderReport = new ResultSet(ResultType.Heading, "Reststukken: ");
        allResults.Add(remainderReport);
        foreach (var item in context.Remainders)
        {
            remainderReport.Add(new ResultSet(ResultType.Good, string.Format(
                "plank {2} van {0}x{1}",
                item.Width, item.Height, item.Identity)));
        }

        var resultReport = new ResultSet(ResultType.Heading, "Producten: ");
        allResults.Add(resultReport);
        foreach (var item in context.ReachedTargets)
        {
            resultReport.Add(new ResultSet(ResultType.Dimension, string.Format(
                "plank {2} van {0}x{1}",
                item.Width, item.Height, item.Identity)));
        }

        var unableReport = new ResultSet(ResultType.Heading, "Niet te maken: ");
        allResults.Add(unableReport);
        foreach (var item in context.PendingTargets)
        {
            unableReport.Add(new ResultSet(ResultType.Bad, string.Format(
                "plank {2} van {0}x{1}",
                item.Width, item.Height, item.Identity)));

        }

        return allResults;
    }

    public override void SetArgs(string[] commandArgs)
    {
        if (commandArgs.Length > 0)
        {
            throw new CommandSyntaxException("RekenUit verwacht geen parameters.");
        }
    }
}