using Microsoft.UI.Xaml.Input;

namespace ExcelPapy.Objects
{
	public sealed partial class SliderCustom : UserControl
	{
        private bool _isDragging;
        private double _dragStartOffsetX; // offset entre le clic et le bord gauche du thumb

        // === Propriétés publiques === 

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(SliderCustom),
                new PropertyMetadata(5d, OnRangeChanged));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(SliderCustom),
                new PropertyMetadata(100d, OnRangeChanged));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(SliderCustom),
                new PropertyMetadata(0d, OnValueChanged));

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, Math.Clamp(value, Minimum, Maximum));
        }

        public event EventHandler<double>? ValueChanged;

        public SliderCustom()
        {
            this.InitializeComponent();
        }

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (SliderCustom)d;
            slider.UpdateThumbFromValue();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (SliderCustom)d;
            slider.UpdateThumbFromValue();
            slider.ValueChanged?.Invoke(slider, (double)e.NewValue);
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateThumbFromValue();
        }

        // === Gestion du drag ===

        private void Thumb_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isDragging = true;
            var pos = e.GetCurrentPoint(RootGrid).Position;
            _dragStartOffsetX = pos.X - Thumb.Margin.Left;

            Thumb.CapturePointer(e.Pointer);
            e.Handled = true;
        }

        private void Thumb_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDragging) return;

            var pos = e.GetCurrentPoint(RootGrid).Position;
            double newLeft = pos.X - _dragStartOffsetX;

            MoveThumbTo(newLeft);
            e.Handled = true;
        }

        private void Thumb_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _isDragging = false;
            Thumb.ReleasePointerCapture(e.Pointer);
        }

        private void Thumb_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            _isDragging = false;
        }

        // === Logique de positionnement ===

        private double MaxThumbLeft => Math.Max(0, RootGrid.ActualWidth - Thumb.Width);

        private void MoveThumbTo(double left)
        {
            left = Math.Clamp(left, 0, MaxThumbLeft);

            Thumb.Margin = new Thickness(left, 0, 0, 0);

            // Le track suit le thumb : on le fait aller jusqu'au centre du thumb
            Track.Width = left + Thumb.Width / 2.0;

            // Calcul de la Value correspondante (sans re-déclencher un recalcul du thumb)
            double ratio = MaxThumbLeft > 0 ? left / MaxThumbLeft : 0;
            double newValue = Minimum + ratio * (Maximum - Minimum);

            SetValue(ValueProperty, newValue); // ne repasse pas par le setter clampé pour éviter une boucle visuelle
            ValueChanged?.Invoke(this, newValue);
        }

        private void UpdateThumbFromValue()
        {
            if (RootGrid.ActualWidth <= 0) return;

            double range = Maximum - Minimum;
            double ratio = range > 0 ? (Value - Minimum) / range : 0;
            double left = ratio * MaxThumbLeft;

            Thumb.Margin = new Thickness(left, 0, 0, 0);
            Track.Width = left + Thumb.Width / 2.0;
        }
    }
}
