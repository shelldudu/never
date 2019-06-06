using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Memcached
{
    public enum Command : byte
    {
        get = 0,
        set = 1,
        add = 2,
        replace = 3,
        delete = 4,
        increment = 5,
        decrement = 6,
        quit = 7,
        flush = 8,
        getq = 9,
        noop = 10,
        version = 11,
        getk = 12,
        getkq = 13,
        append = 14,
        prepend = 15,
        stat = 16,
        setq = 17,
        addq = 18,
        replaceq = 19,
        deleteq = 20,
        incrementq = 21,
        decrementq = 22,
        quitq = 23,
        flushq = 24,
        appendq = 25,
        prependq = 26,
        sasllist = 32,
        saslstart = 33,
        saslstep = 34
    }
}
