using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Test.Init;

namespace NAJ_Lab1
{
    class Engine : ECSEngine
    {
        public override void InitialiseContent()
        {
        }

        public override void Initialise()
        {
            new InitGame(this);
            SceneManager.Instance.SetActiveScene("Game");
        }
    }
}
