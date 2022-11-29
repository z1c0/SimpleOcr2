namespace Newport.Converters
{
    public class BooleanToBrushConverter : BaseConverter
    {
        protected override object OnConvert(object value)
        {
            if (value is bool && (bool)value)
            {
                return TrueBrush;
            }
            return FalseBrush;
        }

        protected override object OnConvertBack(object value)
        {
            throw new NotImplementedException();
        }

        public Brush FalseBrush
        {
            get { return (Brush)GetValue(FalseBrushProperty); }
            set { SetValue(FalseBrushProperty, value); }
        }

        public static readonly BindableProperty FalseBrushProperty =
          BindableProperty.Create("FalseBrush", typeof(Brush), typeof(BooleanToBrushConverter), null);

        public Brush TrueBrush
        {
            get { return (Brush)GetValue(TrueBrushProperty); }
            set { SetValue(TrueBrushProperty, value); }
        }

        public static readonly BindableProperty TrueBrushProperty =
          BindableProperty.Create("TrueBrush", typeof(Brush), typeof(BooleanToBrushConverter), null);
    }
}