using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.InputDefs;
using GameEngine;
using Microsoft.Xna.Framework;

namespace Test
{
    class ChopperControlSystem : IUpdateSystem
    {
        public void Update(GameTime gameTime)
        {
            List<Entity> sceneEntities = SceneManager.Instance.GetActiveScene().GetAllEntities();
            Entity chopper = ComponentManager.Instance.GetEntityWithTag("Chopper",sceneEntities);
            TransformComponent t = ComponentManager.Instance.GetEntityComponent<TransformComponent>(chopper);

            Entity kb = ComponentManager.Instance.GetEntityWithTag("keyboard", sceneEntities);
            if (kb != null)
            {
                KeyBoardComponent k = ComponentManager.Instance.GetEntityComponent<KeyBoardComponent>(kb);
                if (k != null)
                {
                    Vector3 newRot = Vector3.Zero;

                    if (Utilities.CheckKeyboardAction("right", BUTTON_STATE.HELD, k))
                    {
                        newRot = new Vector3(-0.018f, 0f, 0f);
                        t.vRotation = newRot;
                    }
                    else if (Utilities.CheckKeyboardAction("left", BUTTON_STATE.HELD, k))
                    {
                        newRot = new Vector3(0.018f, 0f, 0f);
                        t.vRotation = newRot;
                    }
                    else
                    {
                        t.vRotation = Vector3.Zero;
                    }

                    if (Utilities.CheckKeyboardAction("up", BUTTON_STATE.HELD, k))
                    {
                        t.position += new Vector3(0f, 0.2f, 0f);
                    }
                    if (Utilities.CheckKeyboardAction("down", BUTTON_STATE.HELD, k))
                    {
                        t.position += new Vector3(0f, -0.2f, 0f);
                    }
                    if (Utilities.CheckKeyboardAction("forward", BUTTON_STATE.HELD, k))
                    {

                        t.position += t.forward * 0.3f;
                    }
                    if (Utilities.CheckKeyboardAction("back", BUTTON_STATE.HELD, k))
                    {

                        t.position += t.forward * -0.3f;
                    }

                }
            }
        }
    }
}
