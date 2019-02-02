using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace PrioHelper
{
    public enum FilterMode
    {
        Contains,
        StartsWith
    }

    /*public class FilteredComboBoxBehavior : ManagedBehaviorBase<ComboBox>
    {
        private ICollectionView currentView;
        private string currentFilter;
        private Binding textBinding;
        private TextBox textBox;

        private PropertyInfo HighlightedInfoPropetyInfo { get; set; }

        public static readonly DependencyProperty FilterModeProperty = DependencyProperty.Register("FilterMode", typeof(FilterMode), typeof(FilteredComboBoxBehavior), new PropertyMetadata(default(FilterMode)));

        public FilterMode FilterMode
        {
            get
            {
                return (FilterMode)this.GetValue(FilterModeProperty);
            }
            set
            {
                this.SetValue(FilterModeProperty, value);
            }
        }


        public static readonly DependencyProperty OpenDropDownOnFocusProperty = DependencyProperty.Register("OpenDropDownOnFocus", typeof(bool), typeof(FilteredComboBoxBehavior), new PropertyMetadata(true));

        public bool OpenDropDownOnFocus
        {
            get
            {
                return (bool)this.GetValue(OpenDropDownOnFocusProperty);
            }
            set
            {
                this.SetValue(OpenDropDownOnFocusProperty, value);
            }
        }

        protected override void OnSetup()
        {
            base.OnSetup();
            this.AssociatedObject.KeyUp += this.AssociatedObjectOnKeyUp;
            this.AssociatedObject.IsKeyboardFocusWithinChanged += this.OnIsKeyboardFocusWithinChanged;
            this.textBox = this.AssociatedObject.FindChild<TextBox>();

            this.textBinding = BindingOperations.GetBinding(this.AssociatedObject, ComboBox.TextProperty);
            this.HighlightedInfoPropetyInfo = typeof(ComboBox).GetProperty(
             "HighlightedInfo",
             BindingFlags.Instance | BindingFlags.NonPublic);

            var pd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ComboBox));
            pd.AddValueChanged(this.AssociatedObject, this.OnItemsSourceChanged);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.KeyUp -= this.AssociatedObjectOnKeyUp;
            if (this.currentView != null)
            {
                // ReSharper disable once DelegateSubtraction 
                this.currentView.Filter -= this.TextInputFilter;
            }

            BindingOperations.ClearAllBindings(this);
        }


        private void OnItemsSourceChanged(object sender, EventArgs eventArgs)
        {
            this.currentFilter = this.AssociatedObject.Text;
            if (this.currentView != null)
            {
                // ReSharper disable once DelegateSubtraction 
                this.currentView.Filter -= this.TextInputFilter;
            }

            if (this.AssociatedObject.ItemsSource != null)
            {
                this.currentView = CollectionViewSource.GetDefaultView(this.AssociatedObject.ItemsSource);
                this.currentView.Filter += this.TextInputFilter;
            }

            this.Refresh();
        }

        private void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (this.AssociatedObject.IsKeyboardFocusWithin)
            {
                this.AssociatedObject.IsDropDownOpen = this.AssociatedObject.IsDropDownOpen || this.OpenDropDownOnFocus;
            }
            else
            {
                this.AssociatedObject.IsDropDownOpen = false;
                this.currentFilter = this.AssociatedObject.Text;
                this.Refresh();
            }
        }

        private void AssociatedObjectOnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (!this.IsTextManipulationKey(keyEventArgs)
             || (Keyboard.Modifiers.HasAnyFlag() && Keyboard.Modifiers != ModifierKeys.Shift)
             )
            {
                return;
            }

            if (this.currentFilter != this.AssociatedObject.Text)
            {
                this.currentFilter = this.AssociatedObject.Text;
                this.Refresh();
            }
        }

        private bool TextInputFilter(object obj)
        {
            var stringValue = obj as string;
            if (obj != null && !(obj is string))
            {
                var path = (string)this.GetValue(TextSearch.TextPathProperty);
                if (path != null)
                {
                    stringValue = obj.GetType().GetProperty(path).GetValue(obj) as string;
                }
            }

            if (stringValue == null)
                return false;


            switch (this.FilterMode)
            {
                case FilterMode.Contains:
                    return stringValue.IndexOf(this.currentFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                case FilterMode.StartsWith:
                    return stringValue.StartsWith(this.currentFilter, StringComparison.OrdinalIgnoreCase);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsTextManipulationKey(KeyEventArgs keyEventArgs)
        {
            return keyEventArgs.Key == Key.Back
             || keyEventArgs.Key == Key.Space
             || (keyEventArgs.Key >= Key.D0 && keyEventArgs.Key <= Key.Z)
             || (Keyboard.IsKeyToggled(Key.NumLock) && keyEventArgs.Key >= Key.NumPad0 && keyEventArgs.Key <= Key.NumPad9)
             || (keyEventArgs.Key >= Key.Multiply && keyEventArgs.Key <= Key.Divide)
             || (keyEventArgs.Key >= Key.Oem1 && keyEventArgs.Key <= Key.OemBackslash);

        }

        private void Refresh()
        {
            if (this.currentView != null)
            {
                var tempCurrentFilter = this.AssociatedObject.Text;
                using (new SuspendBinding(this.textBinding, this.AssociatedObject, ComboBox.TextProperty))
                {
                    this.currentView.Refresh();
                    //reset internal highlighted info 
                    this.HighlightedInfoPropetyInfo.SetValue(this.AssociatedObject, null);
                    this.AssociatedObject.SelectedIndex = -1;
                    this.AssociatedObject.Text = tempCurrentFilter;

                }

                if (this.textBox != null && tempCurrentFilter != null)
                {
                    this.textBox.SelectionStart = tempCurrentFilter.Length;
                    this.textBox.SelectionLength = 0;
                }
            }
        }
    }

    /// <summary> 
    /// Temporarely suspend binding on dependency property 
    /// </summary> 
    public class SuspendBinding : IDisposable
    {
        private readonly Binding bindingToSuspend;

        private readonly DependencyObject target;

        private readonly DependencyProperty property;

        public SuspendBinding(Binding bindingToSuspend, DependencyObject target, DependencyProperty property)
        {
            this.bindingToSuspend = bindingToSuspend;
            this.target = target;
            this.property = property;
            BindingOperations.ClearBinding(target, property);
        }

        public void Dispose()
        {
            BindingOperations.SetBinding(this.target, this.property, this.bindingToSuspend);
        }
    }


    public abstract class ManagedBehaviorBase<T> : Behavior<T> where T : FrameworkElement
    {
        private bool isSetup;
        private bool isHookedUp;
        private WeakReference weakTarget;

        protected virtual void OnSetup() { }
        protected virtual void OnCleanup() { }
        protected override void OnChanged()
        {
            var target = this.AssociatedObject;
            if (target != null)
            {
                this.HookupBehavior(target);
            }
            else
            {
                this.UnHookupBehavior();
            }
        }

        private void OnTargetLoaded(object sender, RoutedEventArgs e) { this.SetupBehavior(); }

        private void OnTargetUnloaded(object sender, RoutedEventArgs e) { this.CleanupBehavior(); }

        private void HookupBehavior(T target)
        {
            if (this.isHookedUp) return;
            this.weakTarget = new WeakReference(target);
            this.isHookedUp = true;
            target.Unloaded += this.OnTargetUnloaded;
            target.Loaded += this.OnTargetLoaded;
            if (target.IsLoaded)
            {
                this.SetupBehavior();
            }
        }

        private void UnHookupBehavior()
        {
            if (!this.isHookedUp) return;
            this.isHookedUp = false;
            var target = this.AssociatedObject ?? (T)this.weakTarget.Target;
            if (target != null)
            {
                target.Unloaded -= this.OnTargetUnloaded;
                target.Loaded -= this.OnTargetLoaded;
            }
            this.CleanupBehavior();
        }

        private void SetupBehavior()
        {
            if (this.isSetup) return;
            this.isSetup = true;
            this.OnSetup();
        }

        private void CleanupBehavior()
        {
            if (!this.isSetup) return;
            this.isSetup = false;
            this.OnCleanup();
        }
    }*/
}
