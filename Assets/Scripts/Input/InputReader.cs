using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Platformer
{


    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/InputReader")]

    public class InputReader : ScriptableObject, CharacterInputActions.IPlayerActions
    {

        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate {};
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };

        // Getting inputactions object from CharacterInputActions scritp
        CharacterInputActions inputActions;

        public Vector3 Direction => (Vector3)inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new CharacterInputActions();
                inputActions.Player.SetCallbacks(instance: this);
            }
            inputActions.Enable();
        }


        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(arg0: context.ReadValue<Vector2>());
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // noop
        }
   
        bool IsDeviceMouse(InputAction.CallbackContext context)
        {
            return context.control.device.name == "Mouse";
        }

    }

}