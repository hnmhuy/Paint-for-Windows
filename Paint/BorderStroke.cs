namespace Paint
{
    public class BorderStroke
    {
        private bool isDash = false;
        private object data;

        public BorderStroke(bool isDash, object data)
        {
            this.isDash = isDash;
            this.data = data;
        }
    }
}
