namespace QuickInput
{
    public class Button
    {
        private ButtonState state = ButtonState.up;

        public enum ButtonState
        {
            up,
            pressed,
            down,
            released
        }

        /// <summary>
        /// Only works with positive_input that always returns true.
        /// Do not use Press or Release actions like GetKeyDown OrGetKeyUp as an argument because they execute more frequently than Engine.Run() does.
        /// </summary>
        /// <param name="positive_input"></param>
        public void CheckForInput(bool positive_input)
        {
            if (positive_input)
            {
                Press();
            }
            else
            {
                Release();
            }
        }

        public void Press()
        {
            if (state != ButtonState.pressed && state != ButtonState.down)
            {
                state = ButtonState.pressed;
            }
            else
            {
                state = ButtonState.down;
            }
        }
        public void Release()
        {
            if (state != ButtonState.released && state != ButtonState.up)
            {
                state = ButtonState.released;
            }
            else
            {
                state = ButtonState.up;
            }
        }

        public bool IsPressed()
        {
            return state == ButtonState.pressed;
        }
        public bool IsReleased()
        {
            return state == ButtonState.released;
        }
        public bool IsUp()
        {
            return state == ButtonState.up;
        }
        public bool IsDown()
        {
            return state == ButtonState.down;
        }
    }
}