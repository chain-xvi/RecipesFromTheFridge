using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesFromTheFridge.Helpers
{
    class CreatingDelay
    {
        /// <summary>
        /// Creates a time delay
        /// </summary>
        /// <param name="ms">Holds the number of milliseconds to send to the <see cref="Task.Delay(int)"/></param>
        /// <returns>returns a <see cref="Task"/> which represents an action being completed</returns>
        internal static async Task CreateDelay(int ms) => await Task.Run(async () => { await Task.Delay(ms); });
    }
}
