namespace QuickInput
{
    public class Axis
    {
        public float position = 0f;
        public LastPressed lastPressed = LastPressed.positive;

        public enum LastPressed
        {
            positive,
            negative
        }

        public void CheckForInput(Button positive_input, Button negative_input)
        {
            if (positive_input.IsPressed())
            {
                lastPressed = LastPressed.positive;
            }

            if (negative_input.IsPressed())
            {
                lastPressed = LastPressed.negative;
            }

            bool positive = positive_input.IsDown();
            bool negative = negative_input.IsDown();

            if (positive_input.IsDown() && negative_input.IsDown())
            {
                if (lastPressed == LastPressed.positive)
                {
                    positive = true;
                    negative = false;
                }
                else
                {
                    positive = false;
                    negative = true;
                }
            }

            if (positive)
            {
                position = 1;
            }
            else if (negative)
            {
                position = -1;
            }
            else
            {
                position = 0;
            }
        }
    }
}