using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Base
{
    public class SequenceSync<T1, T2>
    {
        Func<List<T1>> GetSequence1;
        Func<List<T2>> GetSequence2;

        public List<T1> Sequence1;
    }
}
