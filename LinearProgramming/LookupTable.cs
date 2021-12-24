using System;
using System.Collections.Generic;
using System.Text;

namespace LinearProgramming
{
    public class LookupTable
    {
        /**
   * 
   */
        private const long serialVersionUID = -5236327868793399936L;
        private List<int> theTable;

        public LookupTable()
        {
            theTable = new List<int>();
        }

        public int lookup(int aVariable)
        {
            int theIndex = theTable.IndexOf(aVariable);
            if (theIndex == -1)
            {
                theTable.Add(aVariable);
                theIndex = theTable.Count - 1;
            }

            return theIndex + 1;
        }

        public int reverseLookup(int aVariable)
        {
            if (aVariable <= theTable.Count && aVariable > 0)
            {
                return theTable[aVariable - 1];
            }
            else
            {
                return 0;
            }
        }

        public int getSize()
        {
            return theTable.Count;
        }
    }
}
