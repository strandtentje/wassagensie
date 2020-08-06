using System;
using System.Collections.Generic;

namespace WasSagenSie
{
    [Serializable]
    internal class ZaagContext
    {

        public List<Size> Cuttables = new List<Size>();
        public List<Size> Remainders = new List<Size>();
        public List<Cut> Cuts = new List<Cut>();
        public List<Size> PendingTargets = new List<Size>();
        public List<Size> ReachedTargets = new List<Size>();

        public short Sawcut { get; internal set; }

        internal void AddCuttable(short width, short height, short amount)
        {
            for (int i = 0; i < amount; i++) AddCuttable(new Size(width, height));
        }

        internal void MarkDone(Size target, Size bestRemainder)
        {
            PendingTargets.RemoveByIdentity(target);
            Remainders.RemoveByIdentity(bestRemainder);
            bestRemainder.IsDone = true;
            ReachedTargets.Add(bestRemainder);
        }

        private void AddCuttable(Size size)
        {
            Cuttables.Add(size);
        }

        internal Cut CutHeight(Size bestRemainder, short height)
        {
            short cutHeight = bestRemainder.Height;
            cutHeight-=height;
            cutHeight-=Sawcut;

            var cutRemainder = new Size(bestRemainder.Width, cutHeight);

            if (cutRemainder.Height > 0) Remainders.Add(cutRemainder);

            Remainders.RemoveByIdentity(bestRemainder);
            var newRemainder = bestRemainder.Clone();
            newRemainder.Height = height;
            Remainders.Add(newRemainder);

            var cut = new Cut()
            {
                Position = (short)(height + (Sawcut / 2)),
                Result = newRemainder, 
                Remainder = cutRemainder, 
                Source = bestRemainder, 
                WorH = 'h'
            };
            Cuts.Add(cut);

            return cut;
        }

        internal Cut CutWidth(Size bestRemainder, short width)
        {
            short cutWidth = bestRemainder.Width;
            cutWidth -= width;
            cutWidth -= Sawcut;

            var cutRemainder = new Size(cutWidth, bestRemainder.Height);

            if (cutRemainder.Width > 0) Remainders.Add(cutRemainder);

            Remainders.RemoveByIdentity(bestRemainder);
            var newRemainder = bestRemainder.Clone();
            newRemainder.Width = width;
            Remainders.Add(newRemainder);

            var cut = new Cut()
            {
                Position = (short)(width + (Sawcut / 2)),
                Result = newRemainder,
                Remainder = cutRemainder,
                Source = bestRemainder,
                WorH = 'w'
            };
            Cuts.Add(cut);

            return cut;
        }

        internal void AddPendingTarget(short width, short height, short amount)
        {
            for (int i = 0; i < amount; i++) AddPendingTarget(new Size(width, height));
        }

        private void AddPendingTarget(Size size)
        {
            PendingTargets.Add(size);
        }
    }
}

[Serializable]
public class Size
{
    public Size Clone()
    {
        return new Size(Width, Height, IsDone);
    }

    internal static readonly Size Max = new Size(short.MaxValue, short.MaxValue);
    public short Width, Height; public bool IsDone;
    public static int IdentityCounter = 0;
    public int Identity;

    public Size(short width, short height, bool isDone = false)
    {
        Identity = Size.IdentityCounter++;
        Width = width;
        Height = height;
        IsDone = isDone;
    }

    public bool IsRotated { get; internal set; }

    public bool CanContain(Size inner)
    {
        return (Width >= inner.Width) && (Height >= inner.Height);
    }

    public bool CanContainRotated(Size inner)
    {
        return (Width >= inner.Height) && (Height >= inner.Width);
    }

    internal void SwapDimensions()
    {
        IsRotated = false;
        var ph = Width;
        Width = Height;
        Height = ph;
    }
}

[Serializable]
class Cut
{
    public Size Source, Result, Remainder; public short Position; public char WorH;
}