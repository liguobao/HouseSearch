using System;
using System.Collections.Generic;

namespace JiebaNet.Segmenter.FinalSeg
{
    public interface IFinalSeg
    {
        IEnumerable<string> Cut(string sentence);
    }
}