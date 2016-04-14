using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.InputDefs;
using GameEngine;

namespace Test
{
    class Utilities
    {
        public static bool CheckKeyboardAction(string action, BUTTON_STATE state, KeyBoardComponent kbc)
        {
            if (kbc != null)
            {
                if (kbc.GetActionState(action) == state)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
